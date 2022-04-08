using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Entity
{
    [Tooltip("Relative to the projectile's rotation")]
    public Vector3 StartingVelocity = Vector3.zero;
    [Tooltip("How long (in seconds) it takes this projectile to reach its StartingVelocity")]
    public float StartingAccelTime = 1;
    public float ExplosionRadius = 0.5f;

    [HideInInspector]
    public Unit owner;
    [HideInInspector]
    public float power;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        TargetVelocity = StartingVelocity;
        AccelTime = StartingAccelTime;
    }

    private void OnCollisionEnter(Collision ignored)
    {
        // TODO: Spawn explosion particle

        // Damage any units in the explosion radius
        // (The Unit class will prevent friendly fire)
        Collider[] hits = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach(Collider c in hits)
        {
            if(c == null || c.gameObject == null) {return;}
            Unit u = c.gameObject.GetComponentInParent<Unit>();
            if(u != null) u.Damaged(owner, power);
        }
        // Destroy this missile
        Destroy(gameObject);
    }
}
