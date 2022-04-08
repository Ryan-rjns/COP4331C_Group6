using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : BaseProjectile
{
    Vector3 m_direction;
    bool m_fired;
    


    protected override void Start(){
        base.Start();
        MaxSpeed = 5.0f;
        AccelTime = 3.0f;
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
            this.transform.rotation = laucher.transform.rotation;
            TargetVelocity = ScaleVelocity(this.transform.rotation * Vector3.forward);
        }
    }
}
