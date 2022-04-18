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

    // Update is called once per frame

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
            m_direction = Vector3.down;
            TargetVelocity = ScaleVelocity(Vector3.down);
        }
    }
}
