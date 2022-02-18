using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    // The offset of the thrid person camera from the helicopter
    private static Vector3 CAM_TP_OFFSET = new Vector3(0,1.5f,-3);
    // The forwards pitch of the thrid person camera (degrees)
    private static float CAM_TP_PITCH = 20;

    // The force required to keep the helicopter hovering in place when no inputs are given
    private const float HOVER_FORCE = 835;
    // The force exerted on the helicopter due to up/down inputs (upwards force)
    private const float UP_FORCE = 400;
    // The torque exerted on the helicopter due to forward/backward inputs (forward tilt)
    private const float FORWARD_FORCE = 10;
    // The force exerted on the helicopter backrotor due to left/right inputs (horizontal spin)
    private const float RIGHT_FORCE = 10;
    // The maximum pitch of the helicopter before it starts trying to correct itself
    private const float MAX_PITCH = 20;

    // The speed of the helicopter's propellor and backrotor (rotations/second)
    private const float ROTOR_SPEED = 4;
    // The change in propellor speed due to player inputs (rotations/second)
    private const float PROPELLOR_SPEED_BONUS = 1;


    // Helicopter's children
    private Rigidbody rb;
    private Transform body;
    private Transform camTP;
    // Body's children
    private Transform propellor;
    private Transform backrotor;

    private float inputUp;
    private float inputForward;
    private float inputRight;

    // Start is called before the first frame update
    void Start()
    {
        // Register Helcopter's children
        rb = transform.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Helicopter: Can't find Rigidbody component!");
        }
        body = transform.Find("Body");
        if (body == null)
        {
            Debug.LogError("Helicopter: Can't find child \"Body\"!");
        }
        camTP = transform.Find("CamTP");
        if (camTP == null)
        {
            Debug.LogError("Helicopter: Can't find child \"CamTP\"!");
        }
        // Register Body's children
        propellor = body.Find("Propeller");
        if (propellor == null)
        {
            Debug.LogError("Helicopter: Can't find child \"Propeller\"!");
        }
        backrotor = body.Find("Backrotor");
        if (backrotor == null)
        {
            Debug.LogError("Helicopter: Can't find child \"Backrotor\"!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // >>>>> Read Inputs:

        inputUp = Input.GetAxis("Jump");
        inputForward = Input.GetAxis("Vertical");
        inputRight = Input.GetAxis("Horizontal");


        // >>>>> Apply Physics Forces:

        // Read rotations
        float pitch = transform.rotation.eulerAngles.x;
        if (pitch > 180) pitch -= 360;
        float yaw = transform.rotation.eulerAngles.y;
        float roll = transform.rotation.eulerAngles.z;
        // Aopply input / hover forces
        rb.AddRelativeForce(Vector3.up * (HOVER_FORCE + UP_FORCE * inputUp));
        rb.AddTorque(Vector3.right * (FORWARD_FORCE * inputForward));
        rb.AddTorque(Vector3.up * (RIGHT_FORCE * inputRight));
        // Clamp rotations
        transform.rotation = Quaternion.Euler(Mathf.Clamp(pitch,-20,20), yaw, 0);


        // >>>>> Enforce Camera Position:

        // TODO: Implement a spring arm by raytracing from helicopter to the target position
        camTP.position = transform.position + (Quaternion.Euler(0, yaw, 0) * CAM_TP_OFFSET);
        camTP.rotation = Quaternion.Euler(CAM_TP_PITCH, yaw, 0);


        // >>>>> Animate spinning blades:

        float propellorSpeed = Time.deltaTime * 360 * (ROTOR_SPEED + PROPELLOR_SPEED_BONUS * inputUp);
        float backrotorSpeed = Time.deltaTime * 360 * (inputRight == 0 ? ROTOR_SPEED / 2 : ROTOR_SPEED * inputRight);
        propellor.Rotate(0, propellorSpeed, 0);
        backrotor.Rotate(backrotorSpeed, 0, 0);
    }
}
