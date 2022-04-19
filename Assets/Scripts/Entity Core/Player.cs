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
    private static Vector3 CAM1_POS = new Vector3(0, -0.35f, 0.7f);
    // If an object is blocking the camera, this is how far (in meters) the camera has to be in front of that object
    public const float CAM_BLOCKING_TOLERANCE = 0.05f;
    // Targeting max distance
    public const float TARGET_MAX_DIST = 75.0f;


    // Inspector items
    public GameObject weapon1Prefab;
    public GameObject weapon2Prefab;
    public GameObject weapon3Prefab;


    // A ref the this player's camera
    Camera cam;
    // The euler rotational offset of the camera due to player inputs
    Vector3 camRot;
    // Current camera
    [HideInInspector]
    public int currentCam = 0;
    // How far (in meters) away from the player the camera is
    float cam0Dist = 3.0f;
    float cam1Dist = 4.0f;
    // A scalar for how fast the camera moves relative to the player's mouse
    float camSpeed = 2000.0f;
    // A scalar for how fast the camera zooms in and out
    float camZoomSpeed = 1000.0f;
    // Count of remaining weapons
    [HideInInspector]
    public int weapon1;
    [HideInInspector]
    public int weapon2;
    [HideInInspector]
    public int weapon3;
    // Current weapon
    [HideInInspector]
    public int currWeapon = 1;
    // Weapon colldown
    private float[] weaponCooldowns = new float[3];

    // Targeting refs
    [HideInInspector]
    public GameObject[] enemies = null;
    [HideInInspector]
    public List<Vector3> targets = null;

    private Transform bulletSpawn;

    protected override void Start()
    {
        base.Start();

        // Init vars
        currWeapon = 1;
        
        // Initialize teams
        team = Team.Get("Player").SetRel("Enemy", Relationship.HOSTILE);
        Team.Get("Enemy").SetRel("Player", Relationship.HOSTILE);

        // Lock the mouse cursor
        GameManager.LockCursor(true);

        // Register the camera
        cam = GetComponentInChildren<Camera>();
        weapon1 = weapon2 = weapon3 = 5;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(cam != null)
        {
            camRot = Quaternion.LookRotation(cam.transform.position, transform.position).eulerAngles.Euler180();
            cam0Dist = (cam.transform.position - transform.position).magnitude;
        }
        else EntityDebug("Could not find a Camera component in a child GameObject!", DebugType.ERROR);

        // Register bulletSpawn
        var bulletSpawns = gameObject.FindChildren("bulletSpawn", false, 1);
        if (bulletSpawns.Count > 0 && bulletSpawns[0] != null)
        {
            bulletSpawn = bulletSpawns[0].transform;
        }
        else Debug.LogError("Player cannot find bulletSpawn!");
    }

    protected override void Update()
    {
        base.Update();
        
        // Movement Input:
        FlyUp(Input.GetAxis("Jump"));
        FlyForward(Input.GetAxis("Vertical"));
        FlyRight(Input.GetAxis("Horizontal"));
        FlyPivot(Input.GetAxis("Strafe"));

        // Firing input:
        if(Input.GetAxis("Fire1") > 0.1)
        {
            PlayerFire();
        }
        // Weapon cooldowns
        for(int i = 0; i < weaponCooldowns.Length; i++)
        {
            if(weaponCooldowns[i] > 0) weaponCooldowns[i] -= Time.deltaTime;
        }

        // Pause Key:
        if (Input.GetKeyUp(KeyCode.P))
        {
            GameManager.Pause();
        }

        // Cam Toggle Key:
        if (Input.GetKeyUp(KeyCode.Tab)) {
            // %2 disables Cam2, %3 enables it.
            currentCam = (currentCam + 1) % 2;
        }

        // Change Weapons
        if(GameManager.playerData != null) {
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                if(GameManager.playerData.weapon1[0]) currWeapon = 1;
            }
            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                if(GameManager.playerData.weapon2[0]) currWeapon = 2;
            }
            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                if(GameManager.playerData.weapon3[0]) currWeapon = 3;
            }
        }

        // Cam 0 and 1: Accept rotation input
        if (currentCam == 0 || currentCam == 1)
        {
            Vector3 mouseInput = new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            camRot += mouseInput * camSpeed * Time.deltaTime;
            camRot = camRot.ClampX(-80.0f, 80.0f);
            // Calculate the camera angle according to helicopter yaw and accumulated player input
            Quaternion camAngle = Quaternion.Euler(camRot + transform.rotation.eulerAngles.y.ToVecY());

            // Cam 0: Third Person: indepently rotate around player and zoom in/out
            if (currentCam == 0)
            {
                cam0Dist += -1 * Input.GetAxis("Mouse ScrollWheel") * camZoomSpeed * Time.deltaTime;
                cam0Dist = Mathf.Clamp(cam0Dist, CAM0_MIN_DIST, CAM0_MAX_DIST);
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
            // Cam 1: First Person: Fixed in place, follows mouse control
            else if (currentCam == 1)
            {
                if(bulletSpawn != null) cam.transform.position = bulletSpawn.transform.position;
                else cam.transform.position = transform.position + (transform.rotation * CAM1_POS);
                cam.transform.rotation = Quaternion.Euler(new Vector3(camAngle.eulerAngles.x + 180.0f, camAngle.eulerAngles.y, camAngle.eulerAngles.z));
                targets = visibleEnemies();
            }
        }
        // Cam 2: Top-Down: Follow player's yaw, zoom in/out (DISABLED)
        if(currentCam == 2) {
            cam.transform.position = transform.position + (Vector3.up * cam1Dist);
            cam1Dist += -1 * Input.GetAxis("Mouse ScrollWheel") * camZoomSpeed * Time.deltaTime;
            cam1Dist = Mathf.Clamp(cam1Dist, CAM1_MIN_DIST, CAM1_MAX_DIST);
            cam.transform.LookAt(transform.position);
            cam.transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90, cam.transform.rotation.eulerAngles.x);
        }
    }

    private void OnCollisionEnter(Collision col) {
        SafeCollision safe = col.gameObject?.GetComponent<SafeCollision>();
        // If the collision is tagged as "safe", don't crash the helicopter
        if (safe != null) return;
        this.Damaged(null, this.Health);
    } 

    public List<Vector3> visibleEnemies() {
        List<Vector3> visible = new List<Vector3>();
        
        foreach(GameObject e in enemies) {
            if(e==null) continue;
            Vector3 screenPoint = cam.WorldToViewportPoint(e.transform.position);
            float enemyDistance = (transform.position - e.transform.position).magnitude;

            if(enemyDistance <= TARGET_MAX_DIST && screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 
                                                && screenPoint.y > 0 && screenPoint.y < 1) {
                visible.Add(cam.WorldToScreenPoint(e.transform.position));
            }
        }

        // TEMP DEBUG: Prevent the "lock-on" effect, and just leave the crosshairs in the middle of the screen
        visible.Clear();

        return visible;
    }

    public override void DeathAnimation(bool destroySelf = true)
    {
        base.DeathAnimation(false);
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            // Ignore the camrea
            if (obj.GetComponentInChildren<Camera>() != null) continue;
            // Disable everything else
            obj.SetActive(false);
        }
        GameManager.Lose();
    }

    private void PlayerFire()
    {
        // Check for errors
        if (currWeapon < 1 || currWeapon > 3)
        {
            Debug.LogError($"currWeapon {currWeapon} is outside of range [1,3]!");
            return;
        }
        if (bulletSpawn == null) return;
        GameObject weaponPrefab = currWeapon == 2 ? weapon2Prefab : (currWeapon == 3 ? weapon3Prefab : weapon1Prefab);
        if(weaponPrefab == null)
        {
            Debug.LogError($"Player missing weapon prefab {currWeapon}");
            return;
        }

        // Check cooldown
        if (weaponCooldowns[currWeapon-1] > 0) return;

        
        float attackPower = 5.0f;
        Vector3 spawnPoint = bulletSpawn.position;
        Quaternion spawnRotation = bulletSpawn.rotation;

        if(currWeapon == 1)
        {
            // TODO: Determine cooldown and attack power from specific weapon's upgrades
            weaponCooldowns[currWeapon-1] = 1.0f;
            attackPower = 5.0f;
            if(currentCam == 1) spawnRotation = cam.transform.rotation;
        }
        

        GameObject spawned = Instantiate(weaponPrefab, spawnPoint, spawnRotation);
        Projectile spawnedProjectile = spawned.GetComponent<Projectile>();
        if (spawnedProjectile != null)
        {
            spawnedProjectile.owner = this;
            spawnedProjectile.power = attackPower;
            spawnedProjectile.explosion = explosionPrefab;
            spawnedProjectile.StartingVelocity = Vector3.forward * 15;
        }
    }
}
