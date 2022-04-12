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


    public override void FireProjectile(GameObject laucher, GameObject target, int damage)
    {
        if(laucher && target){
            m_direction = Vector3.down;
            TargetVelocity = ScaleVelocity(Vector3.down);
        }
    }
}
