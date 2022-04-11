using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : BaseProjectile
{
    Vector3 m_direction;
    bool m_fired;
    


    protected override void Start(){
        base.Start();
    }

    private void OnTriggerEnter(Collider c){
        if(c == null || c.gameObject == null) return;
        Unit u = c.GetComponentInParent<Unit>();
        if(u == null || u == this.owner) return;
        u.Damaged(owner, power);
        // Destroy this missile
        Debug.Log("hit....");
        Destroy(gameObject);
    }
    // Update is called once per frame


    public override void FireProjectile(GameObject laucher, GameObject target, int damage)
    {
        if(laucher && target){
            m_direction = Vector3.down;
            TargetVelocity = ScaleVelocity(Vector3.down);
        }
    }
}
