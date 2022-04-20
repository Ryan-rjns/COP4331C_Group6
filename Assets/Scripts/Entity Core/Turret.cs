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
    public float rateOfFire = 4.0f;
    public float bulletSpeed = 5.0f;
    public float bulletLifetime = 5.0f;

    private GameObject player;
    private Transform bulletSpawn;
    private float cooldown = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Update();

        // Hard difficulty gives increased health, fire rate, and projectile speed
        if (GameManager.playerData != null && GameManager.playerData.difficultyHard)
        {
            health *= 2.0f;
            rateOfFire *= 2.0f;
            bulletSpeed *= 3.0f;
        }

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
        float aggroDist = bulletSpeed * bulletLifetime * 0.9f;
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
        else if (playerDist <= aggroDist)
        {
            cooldown = rateOfFire;
            GameObject spawned = Instantiate(projectile, bulletSpawn.position, bulletSpawn.rotation);
            spawned.transform.localScale = transform.localScale;
            Projectile spawnedProjectile = spawned.GetComponent<Projectile>();
            if(spawnedProjectile != null)
            {
                spawnedProjectile.owner = this;
                spawnedProjectile.power = attackPower;
                spawnedProjectile.explosion = explosionPrefab;
                spawnedProjectile.SetProjectileVelocity(Vector3.forward * bulletSpeed);
                spawnedProjectile.Lifetime = bulletLifetime;
            }
        }
    }
}

