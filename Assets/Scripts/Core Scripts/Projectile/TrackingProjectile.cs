using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingProjectile : BaseProjectile
{
    GameObject m_target;
    Vector3 m_direction;


    protected override void Start(){
        base.Start();
        MaxSpeed = 5.0f;
        AccelTime = 3.0f;
    }
    
    private void OnCollisionEnter(Collision c){
        if(c == null || c.gameObject == null) return;
        Unit u = c.gameObject.GetComponentInParent<Unit>();
        if(u != null) u.Damaged(owner, power);
        // Destroy this missile
        Debug.Log("hit....");
        Destroy(gameObject);
    }

    // Update is called once per frame
    protected override void Update(){
        base.Update();
        if(m_target){
            transform.LookAt(m_target.transform.position);
            TargetVelocity = ScaleVelocity(this.transform.rotation * Vector3.forward);
        }
    }

    public override void FireProjectile(GameObject laucher, GameObject target, int damage)
    {
        if(target){
            m_target = target;
            this.transform.rotation = laucher.transform.rotation;
            TargetVelocity = ScaleVelocity(this.transform.rotation * Vector3.forward);
        }
    }
}
