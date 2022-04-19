using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : BaseProjectile
{
    Vector3 m_direction;
    bool m_fired;
    
    [HideInInspector]
    public Unit owner;
    [HideInInspector]
    public float power;
    [HideInInspector]
    public GameObject explosion;
    public float ExplosionRadius = 0.5f;


    protected override void Start(){
        base.Start();
    }

    // Update is called once per frame

     protected override void Update(){
        base.Update();
        if(Time.deltaTime >= 10){
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider hitObject) {
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

    public override void FireProjectile(GameObject laucher, GameObject target, int damage)
    {
        if(laucher && target){
            m_direction = Vector3.down;
            TargetVelocity = ScaleVelocity(Vector3.down);
        }
    }
}
