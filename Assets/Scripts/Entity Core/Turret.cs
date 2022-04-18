using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Unit
{
    public const float FIRE_RATE = 4.0f;

    public GameObject projectile;
    public GameObject trackingObject;
    public bool trackingPitch = true;
    public float attackPower = 4.0f;
    public float health = 20.0f;

    private GameObject player;
    private Transform bulletSpawn;
    private float cooldown = FIRE_RATE;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Update();

        // Unit vars
        MaxHealth = health;

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

        // Rotation
        if (trackingObject != null)
        {
            Vector3 baseRot = trackingObject.transform.rotation.eulerAngles;
            trackingObject.transform.LookAt(player.transform);
            Vector3 newRot = trackingObject.transform.rotation.eulerAngles;
            trackingObject.transform.rotation = Quaternion.Euler(new Vector3((trackingPitch ? newRot : baseRot).x, newRot.y, (trackingPitch ? newRot : baseRot).z));
        }

        // Firing
        if(cooldown <= 0)
        {
            cooldown = FIRE_RATE;
            GameObject spawned = Instantiate(projectile, bulletSpawn.position, bulletSpawn.rotation);
            Projectile spawnedProjectile = spawned.GetComponent<Projectile>();
            if(spawnedProjectile != null)
            {
                spawnedProjectile.owner = this;
                spawnedProjectile.power = attackPower;
                spawnedProjectile.explosion = explosionPrefab;
            }
        }
        else
        {
            cooldown -= Time.deltaTime;
        }
    }
}

