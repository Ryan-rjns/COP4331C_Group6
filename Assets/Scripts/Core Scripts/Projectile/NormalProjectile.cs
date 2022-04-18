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

    protected override void Update(){
        base.Update();
        if(Time.deltaTime >= 10){
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other) {
        Destroy(this.gameObject);
    }

    public override void FireProjectile(GameObject laucher, GameObject target, int damage)
    {
        if(laucher && target){
            this.transform.rotation = laucher.transform.rotation;
            TargetVelocity = ScaleVelocity(this.transform.rotation * Vector3.forward);
        }
    }
}
