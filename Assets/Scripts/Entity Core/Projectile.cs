using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Entity
{
    [Tooltip("Relative to the projectile's rotation")]
    public Vector3 StartingVelocity = Vector3.forward;
    [Tooltip("How long (in seconds) it takes this projectile to reach its StartingVelocity")]
    public float StartingAccelTime = 1;
    public float ExplosionRadius = 0.5f;
    public float Lifetime = 5.0f;

    [HideInInspector]
    public Unit owner;
    [HideInInspector]
    public float power;
    [HideInInspector]
    public GameObject explosion;

    protected override void Start()
    {
        base.Start();
        TargetVelocity = transform.rotation * StartingVelocity;
        AccelTime = StartingAccelTime;
    }

    protected override void Update()
    {
        base.Update();
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0) Collision(null);
    }

    public void OnTriggerEnter(Collider other)
    {
        Collision(other.gameObject);
    }

    public void Collision(GameObject hitObject)
    {
        if (hitObject == null)
        {
            // Colliding with null, typically due to Lifetime reaching 0
        }
        else
        {
            Unit hitUnit = hitObject.GetComponentInParent<Unit>();
            if (hitUnit != null)
            {
                if (hitUnit == owner) return;
                // Colliding with other unit
            }
            else
            {
                // Colliding with other gameObject (like the ground)
            }
        }

        
        
        if(explosion != null) Instantiate(explosion, transform.position, transform.rotation);

        // Damage any units in the explosion radius
        // (The Unit class will prevent friendly fire)
        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach(Collider c in hits)
        {
            if(c == null || c.gameObject == null) continue;
            Unit u = c.gameObject.GetComponentInParent<Unit>();
            if(u != null) u.Damaged(owner, power);
        }
        // Destroy this missile
        Destroy(gameObject);
    }
}