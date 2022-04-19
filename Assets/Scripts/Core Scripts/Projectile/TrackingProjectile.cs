using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingProjectile : BaseProjectile
{
    GameObject m_launcher;
    GameObject m_target;
    Vector3 m_direction;
    int m_damage;

    [HideInInspector]
    public Unit owner;
    [HideInInspector]
    public float power;
    [HideInInspector]
    public GameObject explosion;
    public float ExplosionRadius = 0.5f;

    protected override void Start(){
        base.Start();
        MaxSpeed = 5.0f;
        AccelTime = 3.0f;
    }

    // Update is called once per frame
    protected override void Update(){
        base.Update();
        if(m_target){
            this.transform.LookAt(m_target.transform.position);
            TargetVelocity = ScaleVelocity(this.transform.rotation * Vector3.forward);
        }
    }
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
        m_target = target;
        m_damage = damage;
        m_launcher = laucher;
        if(laucher && target){
            this.transform.rotation = laucher.transform.rotation;
            TargetVelocity = ScaleVelocity(this.transform.rotation * Vector3.forward);
        }
    }
}
