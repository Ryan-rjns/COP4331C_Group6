using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This supporting class defines the movement for all helicopter units.
// Helicopter units can move vertically and can yaw in place.
// Helicopter units can also move longitudinally while pitching, and can move laterally while rolling.
// When no input is given a helicopter will rest if on the ground or hover if in the air
public abstract class Helicopter : Unit
{
    // The max speed (in rotations/second) of the helicopter's propeller
    const float PROPELLER_SPEED = 4.0f;
    // The max speed (in rotations/second) of the helicopter's backrotor
    const float BACKROTOR_SPEED = 4.0f;
    // The time (in seconds) it takes the helicopter's propellers to go from 0 to max speed
    const float PROPELLER_ACCEL_TIME = 0.5f;
    // The maximum pitch (in degrees) when the helicopter is moving longitudinally
    const float MAX_PITCH = 20.0f;
    // The maximum roll (in degrees) when the helicopter is moving laterally
    const float MAX_ROLL = 20.0f;
    // The time (in seconds) it takes the helicopter to go from 0 to max pitch/roll
    const float TILT_TIME = 1.0f;


    bool grounded;
    Vector3 InputVelocity = Vector3.zero;
    // 
    float propellerSpeed = 0;
    float backrotorSpeed = 0;

    protected override void Start()
    {
        base.Start();
        // The default speed and accelration for a helicopter:
        MaxSpeed = 3.0f;
        AccelTime = 3.0f;
    }

    protected override void Update()
    {
        base.Update();

        // >>> Important Functionality:

        // Determine if the helicopter is grounded:
        grounded = IsGrounded() ?? false;
        // Move the helicopter towards the input directions (magnitude is automatically clamped to 1)
        TargetVelocity = InputVelocity;


        // >>> Visual Animations:

        // Accelerate the propeller and backrotor to reach their target speeds
        float propellerTarget = grounded && TargetVelocity.y <= 0 ? 0 : PROPELLER_SPEED;
        float backrotorTarget = grounded && TargetVelocity.y <= 0 ? 0 : BACKROTOR_SPEED;
        propellerSpeed = StarLib.PID(propellerSpeed.ToVecX(), propellerTarget.ToVecX(), PROPELLER_SPEED / PROPELLER_ACCEL_TIME).x;
        backrotorSpeed = StarLib.PID(backrotorSpeed.ToVecY(), backrotorTarget.ToVecY(), BACKROTOR_SPEED / PROPELLER_ACCEL_TIME).y;

        // Yaw any children whose name includes "Propeller", and Pitch any children whose name includes "Backrotor"
        foreach (GameObject propeller in gameObject.FindChildren("Propeller"))
        {
            propeller.transform.Rotate(Vector3.up * propellerSpeed);
        }
        foreach (GameObject backrotor in gameObject.FindChildren("Backrotor"))
        {
            backrotor.transform.Rotate(Vector3.right * backrotorSpeed);
        }

        // Pitch the helicopter when it is moving longitudinally and roll it when it is moving laterally (unless grounded
        Vector3 currRot = transform.rotation.eulerAngles.Euler180();
        float pitchTarget = grounded ? 0 : MAX_PITCH * TargetVelocity.normalized.z;
        float rollTarget = grounded ? 0 : MAX_ROLL * TargetVelocity.normalized.x * -1;
        currRot.x = StarLib.PID(currRot.x.ToVecX(), pitchTarget.ToVecX(), MAX_PITCH / TILT_TIME).x;
        currRot.z = StarLib.PID(currRot.z.ToVecZ(), rollTarget.ToVecZ(), MAX_ROLL / TILT_TIME).z;
        transform.rotation = Quaternion.Euler(currRot);
        EntityDebug($"pitchTarget={pitchTarget}, rollTarget={rollTarget}, currPitch={currRot.x},currRoll={currRot.z},currYaw={currRot.y}");
    }



    // Move the helicopter vertically
    // speed - how fast the helicopter will move up (positive) or down (negative)
    // exact - if false, speed is a percentage of MaxSpeed. If true, speed is an exact number (m/s)
    protected virtual void FlyUp(float speed, bool exact = false)
    {
        // When on the ground, the helicopter can only fly up
        if (grounded && speed <= 0) return;
        if (!exact) speed = ScaleVelocity(speed);
        InputVelocity.y = speed;
    }

    // Move the helicopter longitudinally
    // speed - how fast the helicopter will move forwards (positive) or backwards (negative)
    // exact - if false, speed is a percentage of MaxSpeed. If true, speed is an exact number (m/s)
    protected virtual void FlyForward(float speed, bool exact = false)
    {
        // When on the ground, the helicopter can only fly up
        if (grounded) return;
        if (!exact) speed = ScaleVelocity(speed);
        InputVelocity.z = speed;
    }

    // Move the helicopter laterally
    // speed - how fast the helicopter will move right (positive) or left (negative)
    // exact - if false, speed is a percentage of MaxSpeed. If true, speed is an exact number (m/s)
    protected virtual void FlyRight(float speed, bool exact = false)
    {
        // When on the ground, the helicopter can only fly up
        if (grounded) return;
        if (!exact) speed = ScaleVelocity(speed);
        InputVelocity.x = speed;
    }

    // Pivot the helicopter in place
    // speed - how fast the helicopter will move right (positive) or left (negative)
    // exact - if false, speed is a percentage of MaxSpeed. If true, speed is an exact number (m/s)
    protected virtual void FlyPivot(float speed, bool exact = false)
    {
        // When on the ground, the helicopter can only fly up
        if (grounded) return;
        if (!exact) speed = ScaleVelocity(speed);
        // TODO: Implement Pivot
    }
}
