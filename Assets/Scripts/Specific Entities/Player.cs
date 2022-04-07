using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A helicopter with a camera that is controlled by player inputs.
public class Player : Helicopter
{
    // The max and min distances (in meters) that the camera can be from the player
    public const float CAM0_MAX_DIST = 5.0f;
    public const float CAM0_MIN_DIST = 2.0f;
    public const float CAM1_MAX_DIST = 6.0f;
    public const float CAM1_MIN_DIST = 2.0f;
    private static Vector3 CAM2_POS = new Vector3(0, -0.6f, 0.7f);
    // If an object is blocking the camera, this is how far (in meters) the camera has to be in front of that object
    public const float CAM_BLOCKING_TOLERANCE = 0.05f;
    // A ref the this player's camera
    Camera cam;
    // The euler rotational offset of the camera due to player inputs
    Vector3 camRot;
    // Current camera
    private int currentCam = 0;
    // How far (in meters) away from the player the camera is
    float cam0Dist = 3.0f;
    float cam1Dist = 4.0f;
    // A scalar for how fast the camera moves relative to the player's mouse
    float camSpeed = 2000.0f;
    // A scalar for how fast the camera zooms in and out
    float camZoomSpeed = 1000.0f;
    // Count of remaining weapons
    public int weapon1;
    public int weapon2;
    public int weapon3;

    protected override void Start()
    {
        base.Start();
        cam = GetComponentInChildren<Camera>();
        weapon1 = weapon2 = weapon3 = 5;

        if(cam != null)
        {
            camRot = Quaternion.LookRotation(cam.transform.position, transform.position).eulerAngles.Euler180();
            cam0Dist = (cam.transform.position - transform.position).magnitude;
        }
        else EntityDebug("Could not find a Camera component in a child GameObject!", DebugType.ERROR);
    }

    protected override void Update()
    {
        base.Update();

        // Debug commands:
        if (Input.GetKeyUp(KeyCode.Keypad0))
        {
            Debug.Log("Player: Pressed 0, Damaging player");
            Damaged(null,10);
        }
         
        // Movement Input:
        FlyUp(Input.GetAxis("Jump"));
        FlyForward(Input.GetAxis("Vertical"));
        FlyRight(Input.GetAxis("Horizontal"));
        FlyPivot(Input.GetAxis("Strafe"));

        // Pause Key:
        if (Input.GetKeyUp(KeyCode.P))
        {
            GameManager.Pause();
        }

        // Cam Toggle Key:
        if (Input.GetKeyUp(KeyCode.Tab)) {
            currentCam = (currentCam + 1) % 3;
        }

        // Cam 0: Third Person: indepently rotate around player and zoom in/out
        if (currentCam == 0)
        {
            // Accept camera input
            camRot += new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * camSpeed * Time.deltaTime;
            cam0Dist += -1 * Input.GetAxis("Mouse ScrollWheel") * camZoomSpeed * Time.deltaTime;
            // Constrain camera input
            camRot = camRot.ClampX(-80.0f, 80.0f);
            cam0Dist = Mathf.Clamp(cam0Dist, CAM0_MIN_DIST, CAM0_MAX_DIST);
            // Calculate the camera angle according to helicopter yaw and accumulated player input
            Quaternion camAngle = Quaternion.Euler(camRot + transform.rotation.eulerAngles.y.ToVecY());
            cam.transform.position = transform.position + (camAngle * (Vector3.forward * cam0Dist));
            cam.transform.LookAt(transform);

            // Spring arm the camera so that it doesn't clip through objects (It's not perfect, but it mostly works)
            var blockingHit = StarLib.RaycastSearch(transform.position, cam.transform.position).GetV(0);
            if (blockingHit != null)
            {
                Vector3 springArmDir = (cam.transform.position - transform.position).normalized;
                cam.transform.position = blockingHit.Value.hit.point + (blockingHit.Value.hit.normal * CAM_BLOCKING_TOLERANCE);
            }
        }
        // Cam 1: Top-Down: Follow player's yaw, zomm in/out
        else if(currentCam == 1) {
            cam.transform.position = transform.position + (Vector3.up * cam1Dist);
            cam1Dist += -1 * Input.GetAxis("Mouse ScrollWheel") * camZoomSpeed * Time.deltaTime;
            cam1Dist = Mathf.Clamp(cam1Dist, CAM1_MIN_DIST, CAM1_MAX_DIST);
            cam.transform.LookAt(transform.position);
            cam.transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90, cam.transform.rotation.eulerAngles.x);
        }
        // Cam 2: First Person: Fixed in place, follows helicopter's movements
        else if (currentCam == 2)
        {
            cam.transform.position = transform.position + (transform.rotation * CAM2_POS);
            cam.transform.rotation = transform.rotation;
        }
    }
}
