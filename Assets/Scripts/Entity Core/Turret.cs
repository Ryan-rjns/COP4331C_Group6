using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Unit
{

    public GameObject projectile;
    public GameObject trackingObject;
    public bool trackingPitch = true;
    public float attackPower = 4.0f;
    public float health = 20.0f;
    public float aggroDist = 15.0f;
    public float rateOfFire = 4.0f;

    private GameObject player;
    private Transform bulletSpawn;
    private float cooldown = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Update();

        // Init vars
        MaxHealth = health;
        cooldown = rateOfFire;

        // Register player
        player = GameObject.Find("Player");
        if (player == null) Debug.LogError("Turret: Cannot find player!");

        // Register bulletSpawn
        var bulletSpawns = gameObject.FindChildren("bulletSpawn", false, 1);
        if(bulletSpawns.Count <= 0 || bulletSpawns[0] == null) Debug.LogError("Turret: Cannot find bulletSpawn!");
        else bulletSpawn = bulletSpawns[0].transform;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (player == null) return;

        // Rotation
        if (trackingObject != null)
        {
            Vector3 baseRot = trackingObject.transform.rotation.eulerAngles;
            trackingObject.transform.LookAt(player.transform);
            Vector3 newRot = trackingObject.transform.rotation.eulerAngles;
            trackingObject.transform.rotation = Quaternion.Euler(new Vector3((trackingPitch ? newRot : baseRot).x, newRot.y, (trackingPitch ? newRot : baseRot).z));
        }

        // Firing
        float playerDist = (player.transform.position - transform.position).magnitude;
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
        else if (playerDist <= aggroDist)
        {
            cooldown = rateOfFire;
            GameObject spawned = Instantiate(projectile, bulletSpawn.position, bulletSpawn.rotation);
            Projectile spawnedProjectile = spawned.GetComponent<Projectile>();
            if(spawnedProjectile != null)
            {
                spawnedProjectile.owner = this;
                spawnedProjectile.power = attackPower;
                spawnedProjectile.explosion = explosionPrefab;
            }
        }
    }
}

