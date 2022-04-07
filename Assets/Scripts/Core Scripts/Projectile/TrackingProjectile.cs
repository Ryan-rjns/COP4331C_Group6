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

    // Update is called once per frame
    protected override void Update(){
        base.Update();
        if(m_target){
            this.transform.LookAt(m_target.transform.position);
            TargetVelocity = ScaleVelocity(this.transform.rotation * Vector3.forward);
        }
    }
    void OnCollisionEnter(){
        Debug.Log("Hit");
        Destroy(this.gameObject);
    }

    public override void FireProjectile(GameObject laucher, GameObject target, int damage)
    {
        m_target = target;
        if(laucher && target){
            this.transform.rotation = laucher.transform.rotation;
            TargetVelocity = ScaleVelocity(this.transform.rotation * Vector3.forward);
        }
    }
}
