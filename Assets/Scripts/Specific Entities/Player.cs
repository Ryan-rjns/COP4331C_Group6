using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A helicopter with a camera that is controlled by player inputs.
public class Player : Helicopter
{
    // The max and min distances (in meters) that the camera can be from the player
    public const float CAM_MAX_DIST = 5.0f;
    public const float CAM_MIN_DIST = 2.0f;

    // If an object is blocking the camera, this is how far (in meters) the camera has to be in front of that object
    public const float CAM_BLOCKING_TOLERANCE = 0.05f;


    // The gameobject that contains the player's camera
    GameObject cam;
    // The euler rotational offset of the camera due to player inputs
    Vector3 camRot;
    // How far (in meters) away from the player the camera is
    float camDist = 1.0f;
    // A scalar for how fast the camera moves relative to the player's mouse
    float camSpeed = 4.0f;

    protected override void Start()
    {
        base.Start();
        // Find the camera
        cam = gameObject.FindChildren("Camera", false, 1).GetC(0);
        if(cam != null)
        {
            camRot = Quaternion.LookRotation(cam.transform.position, transform.position).eulerAngles.Euler180();
            camDist = (cam.transform.position - transform.position).magnitude;
        }
        else EntityDebug("Could not find a gameobject named \"Camera\"!", DebugType.ERROR);
    }

    protected override void Update()
    {
        base.Update();
        
        // Accept movement input
        FlyUp(Input.GetAxis("Jump"));
        FlyForward(Input.GetAxis("Vertical"));
        FlyRight(Input.GetAxis("Strafe"));

        if (cam != null)
        {
            // Accept camera input
            camRot += camSpeed * new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            camDist += -1 * Input.GetAxis("Mouse ScrollWheel");
            // Constrain camera input
            camRot = camRot.ClampX(-80.0f, 80.0f);
            camDist = Mathf.Clamp(camDist, CAM_MIN_DIST, CAM_MAX_DIST);
            // Calculate the camera angle according to helicopter yaw and accumulated player input
            Quaternion camAngle = Quaternion.Euler(camRot + transform.rotation.eulerAngles.y.ToVecY());
            cam.transform.position = transform.position + (camAngle * (Vector3.forward * camDist));
            cam.transform.LookAt(transform);

            // Spring arm the camera so that it doesn't clip through objects (It's not perfect, but it mostly works)
            var blockingHit = StarLib.RaycastSearch(transform.position, cam.transform.position).GetV(0);
            if (blockingHit != null)
            {
                Vector3 springArmDir = (cam.transform.position - transform.position).normalized;
                cam.transform.position = blockingHit.Value.hit.point + (blockingHit.Value.hit.normal * CAM_BLOCKING_TOLERANCE);
            }
        }
    }
}
