using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebugType
{
    LOG,
    WARNING,
    ERROR
}

// An entity is any "thing" in the world that is dynamic and has some function.
// All Units, Projectiles, etc. inherit from this class, which provides support for common operations, like movement
// All Entities should have a Rigidbody component.
public abstract class Entity : Signal
{
    #region Constants

    // No Entity's hitbox shall exceed this radius (in meters). Used for raycast logic.
    public const float MAX_ENTITY_RADIUS = 20;
    // How close (in meters) the bottom of the Entity's hotbox (directly below origin)
    // has to be to the ground (any non-entity hitbox) in order for an Entity to be considered "Grounded"
    public const float GROUND_TOLERANCE = 0.05f;

    #endregion



    #region Unity Functions

    // All child classes should use base.Start() to make sure that every superclass gets the opportunity to use this function
    protected virtual void Start() 
    {
        if(!TryGetComponent(out rb))
        {
            EntityDebug("No Rigidbody was found on this Entity!"
                + "All Entities should have a Rigidbody, movement will not work without it!", DebugType.ERROR);
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.drag = 0;
            rb.useGravity = false;
        }
    }
    // All child classes should use base.Update() to make sure that every superclass gets the opportunity to use this function
    protected override void Update() => base.Update();
    // All child classes should use base.FixedUpdate() to make sure that every superclass gets the opportunity to use this function
    protected virtual void FixedUpdate() 
    {
        // Increment the current velocity towards the goal of reaching TargetVelocity
        if (Velocity != TargetVelocity)
        {
            // The vector that the current velocity needs to travel along to reach the target velocity
            Vector3 velocityDelta = TargetVelocity - Velocity;
            // The amount that the current velocity will change in this update frame. Make sure it doesn't overshoot the target.
            float velocityStep = Mathf.Min(Acceleration * Time.deltaTime, velocityDelta.magnitude);
            // Increment the current velocity
            Velocity += velocityStep * velocityDelta.normalized;
        }
    }

    #endregion



    #region Movement

    // Child classes should not do anything to the rigidbody! Please use properties like TargetVelocity instead.
    private Rigidbody rb = null;

    // The maximum speed (in m/s) of this Entity
    float _maxSpeed = 10.0f;
    public float MaxSpeed
    {
        get => _maxSpeed;
        protected set
        {
            _maxSpeed = Mathf.Max(value, 0);
            // MaxSpeed could have been reduced, so re-clamp velocities to make sure they doen't exceed the new MaxSpeed.
            Velocity = Velocity;
            TargetVelocity = TargetVelocity;
        }

    }
    
    // The time (in seconds) it takes this Entity to accelerate from 0 to MaxSpeed
    float _accelTime = 0.1f;
    public float AccelTime
    {
        get => _accelTime;
        protected set => _accelTime = Mathf.Max(value, 0);
    }
    
    // The acceleration (in m/s^2) of this Entity, calculated from AccelTime and MaxSpeed (Read-Only).
    public float Acceleration
    {
        get => MaxSpeed / AccelTime;
    }
    
    // The target velocity (in m/s) vector that this entity is attempting to move along.
    // Over time the actual velocity of the entity will accelerate in an attempt to reach this target.
    // The magnitude of this vector is clamped so that it cannot exceed MaxSpeed.
    Vector3 _targetVelocity = Vector3.zero;
    public Vector3 TargetVelocity
    {
        get => _targetVelocity;
        protected set => _targetVelocity = Vector3.ClampMagnitude(value, MaxSpeed);
    }

    // The actual velocity (in m/s) of this Entity (Read-Only).
    public Vector3 Velocity
    {
        get
        {
            if (rb == null) return Vector3.zero;
            return rb.velocity;
        }
        private set 
        {
            if (rb == null) return;
            rb.velocity = value;
        }
    }

    // The turning (yaw) speed (in deg/s) of this Entity
    float _turnSpeed = 25.0f;
    public float TurnSpeed
    {
        get => _turnSpeed;
        protected set
        {
            _turnSpeed = Mathf.Max(value, 0);
        }

    }

    #endregion



    #region Utility

    // Takes an input float percentage in range [-1,1] and scales it by the MaxSpeed.
    public float ScaleVelocity(float percentage)
    {
        return Mathf.Clamp(percentage, -1.0f, 1.0f) * MaxSpeed;
    }
    // Takes an input Vector3 percentage with a magnitude in range [0,1] and scales it by the MaxSpeed.
    public Vector3 ScaleVelocity(Vector3 percentage)
    {
        return Vector3.ClampMagnitude(percentage, 1.0f) * MaxSpeed;
    }
    // Returns true if this entity is on the ground, false if it is in the air, and null if this entity has no colliders.
    // tolerance specifies how close the ground must be. If tolerance <= 0, this fails and returns null.
    public bool? IsGrounded(float tolerance = GROUND_TOLERANCE)
    {
        if (tolerance <= 0) return null;

        // Try to find the bottom of the entity's collision
        Vector3 bottom;
        var resBottom = StarLib.RaycastSearch(transform.position + (Vector3.down * MAX_ENTITY_RADIUS), Vector3.up, MAX_ENTITY_RADIUS * 2,
            (hit, hitObject) => hitObject.Equals(this.gameObject)
            , false/*Stop at the first result*/, true/*Whitelist: Return any parents that match the filter*/);

        // If a bottom was found, record its location
        if (resBottom.Count >= 1) bottom = resBottom[0].hit.point;
        // Otherwise, no bottom was found, then this entity has no collider and this function fails
        else return null;

        var resGround = StarLib.RaycastSearch(bottom + (Vector3.up * tolerance), Vector3.down, tolerance * 2,
            (hit, hitObject) =>
            {
                // If the hit is on a trigger volume, don't count it (blacklist=true)
                if (hit.collider.isTrigger) return true;

                // Try to determine if this GameObject is an Entity
                Entity hitEntity;
                hitObject.TryGetComponent(out hitEntity);
                // If no Entity was found, don't blacklist the hit
                if (hitEntity == null) return false;
                // If an Entity is found in the parent hierarchy, blacklist the entire hit
                return true;
            }
            , false/*Stop at the first result*/, false/*Blacklist: Only accept a hit if none of its parents match the filter*/);
        
        // If a valid ground collider was found, return true. Else, return false
        return resGround.Count >= 1;
    }

    // All child classes should display debug using this method, for more consistent formatting
    protected void EntityDebug(string message, DebugType type = DebugType.LOG)
    {
        // Prepend the class and object names to make it easier to find where info and problems originalte from
        message = $"{GetType()?.Name}: {gameObject?.name}: {message}";
        // Determine what type of debug should be displayed
        if (type == DebugType.LOG) Debug.Log(message);
        else if (type == DebugType.WARNING) Debug.LogWarning(message);
        else if (type == DebugType.ERROR) Debug.LogError(message);
    }

    #endregion


    protected bool destroyed = false;
    public override bool IsSignaled() => destroyed;
}
