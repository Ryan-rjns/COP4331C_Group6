using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A helicopter with a camera that is controlled by player inputs.
public class Player : Helicopter
{
    // The max and min distances (in meters) that the camera can be from the player
    public const float CAM1_MAX_DIST = 5.0f;
    public const float CAM1_MIN_DIST = 2.0f;
    public const float CAM2_MAX_DIST = 6.0f;
    public const float CAM2_MIN_DIST = 2.0f;

    // If an object is blocking the camera, this is how far (in meters) the camera has to be in front of that object
    public const float CAM_BLOCKING_TOLERANCE = 0.05f;


    // The gameobjects for the different cameras
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;
    // The euler rotational offset of the camera due to player inputs
    Vector3 camRot;
    // Current camera
    private int currentCam = 1;
    // How far (in meters) away from the player the camera is
    float cam1Dist = 1.0f;
    float cam2Dist = 4.0f;
    // A scalar for how fast the camera moves relative to the player's mouse
    float camSpeed = 4.0f;

    protected override void Start()
    {
        base.Start();

        if(cam1 != null && cam2 != null && cam3 != null)
        {
            cam2.enabled = false;
            cam3.enabled = false;
            camRot = Quaternion.LookRotation(cam1.transform.position, transform.position).eulerAngles.Euler180();
            cam1Dist = (cam1.transform.position - transform.position).magnitude;
        }
        else EntityDebug("Could not find a gameobject named \"Camera\"!", DebugType.ERROR);
    }

    protected override void Update()
    {
        base.Update();

        // Debug keys:
        if(Input.GetKeyUp(KeyCode.Z))
        {
            GameManager.Win();
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            GameManager.Loose();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            GameManager.PauseMenu();
        }

        // Accept movement input
        FlyUp(Input.GetAxis("Jump"));
        FlyForward(Input.GetAxis("Vertical"));
        FlyRight(Input.GetAxis("Horizontal"));
        FlyPivot(Input.GetAxis("Strafe"));

        if (Input.GetKeyUp(KeyCode.Tab)) {
            Debug.Log(currentCam);
            if(currentCam == 1) {
                cam1.enabled = false;
                cam2.enabled = true;
                currentCam = 2;
            } else if(currentCam == 2) {
                cam2.enabled = false;
                cam3.enabled = true;
                currentCam = 3;
            } else {
                cam3.enabled = false;
                cam1.enabled = true;
                currentCam = 1;
            }
        }

        if (currentCam == 1)
        {
            // Accept camera input
            camRot += camSpeed * new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            cam1Dist += -1 * Input.GetAxis("Mouse ScrollWheel");
            // Constrain camera input
            camRot = camRot.ClampX(-80.0f, 80.0f);
            cam1Dist = Mathf.Clamp(cam1Dist, CAM1_MIN_DIST, CAM1_MAX_DIST);
            // Calculate the camera angle according to helicopter yaw and accumulated player input
            Quaternion camAngle = Quaternion.Euler(camRot + transform.rotation.eulerAngles.y.ToVecY());
            cam1.transform.position = transform.position + (camAngle * (Vector3.forward * cam1Dist));
            cam1.transform.LookAt(transform);

            // Spring arm the camera so that it doesn't clip through objects (It's not perfect, but it mostly works)
            var blockingHit = StarLib.RaycastSearch(transform.position, cam1.transform.position).GetV(0);
            if (blockingHit != null)
            {
                Vector3 springArmDir = (cam1.transform.position - transform.position).normalized;
                cam1.transform.position = blockingHit.Value.hit.point + (blockingHit.Value.hit.normal * CAM_BLOCKING_TOLERANCE);
            }
        }
        else if(currentCam == 2) {
            cam2.transform.position = transform.position + (Vector3.up * cam2Dist);
            cam2Dist += -1 * Input.GetAxis("Mouse ScrollWheel");
            cam2Dist = Mathf.Clamp(cam2Dist, CAM2_MIN_DIST, CAM2_MAX_DIST);
            cam2.transform.LookAt(transform.position);
            cam2.transform.rotation = Quaternion.Euler(cam2.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90, cam2.transform.rotation.eulerAngles.x);
        }
    }
}
