using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Entity
{
    [Tooltip("Relative to the projectile's rotation")]
    public Vector3 StartingVelocity = Vector3.forward;
    public bool relativeVelocity = true;
    [Tooltip("How long (in seconds) it takes this projectile to reach its StartingVelocity")]
    public float StartingAccelTime = 1;
    public float ExplosionRadius = 0.5f;
    public float Lifetime = 5.0f;
    public GameObject explosionPrefab;

    [HideInInspector]
    public Unit owner;
    [HideInInspector]
    public float power;
    

    private bool exploded = false;

    protected override void Start()
    {
        base.Start();
        SetProjectileVelocity(StartingVelocity);
        AccelTime = StartingAccelTime;
    }

    public void SetProjectileVelocity(Vector3 v)
    {
        MaxSpeed = v.magnitude;
        TargetVelocity = StartingVelocity = v;
        if (relativeVelocity) TargetVelocity = transform.rotation * TargetVelocity;
    }

    protected override void Update()
    {
        base.Update();
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0) Collision(null);

        //Debug.Log($"Projectile: Target: {TargetVelocity.magnitude}, Current: {Velocity.magnitude}, Max: {MaxSpeed}, AccelTime: {AccelTime}, Accel: {Acceleration}");
    }

    public void OnTriggerEnter(Collider other)
    {
        Collision(other.gameObject);
    }

    public void Collision(GameObject hitObject)
    {
        if (hitObject == null)
        {
            //Debug.Log($"Projectile {gameObject.name}: Colliding with null");
        }
        else
        {
            Unit hitUnit = hitObject.GetComponentInParent<Unit>();
            if (hitUnit != null)
            {
                if (hitUnit == owner) return;
                //Debug.Log($"Projectile {gameObject.name}: Colliding with non-owner unit {hitUnit.gameObject.name}");
            }
            else
            {
                //Debug.Log($"Projectile {gameObject.name}: Colliding with other gameObject {hitObject.name}");
            }
        }

        // Prevents glitch where damage is dealt multiple times
        if (exploded) return;
        exploded = true;

        ExplosionRadius *= (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3.0f;
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation).transform.localScale = Vector3.one * ExplosionRadius / 2.0f;
        }

        // Damage any units in the explosion radius
        // (The Unit class will prevent friendly fire)
        List<Unit> hitUnits = new List<Unit>();
        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach(Collider c in hits)
        {
            if(c == null || c.gameObject == null) continue;
            Unit u = c.gameObject.GetComponentInParent<Unit>();
            // Make sure that each unit is not hit more than once
            if (u != null && !hitUnits.Contains(u))
            {
                u.Damaged(owner, power);
                hitUnits.Add(u);
            }
        }
        // Destroy this missile
        Destroy(gameObject);
    }
}
