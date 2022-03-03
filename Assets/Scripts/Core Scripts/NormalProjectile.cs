using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : BaseProjectile
{
    Vector3 m_direction;
    bool m_fired;

    // Update is called once per frame
    void Update(){
        if(m_fired){
            transform.position += m_direction * (speed * Time.deltaTime);
        }
    }

    public override void FireProjectile(GameObject laucher, GameObject target, int damage)
    {
        if(laucher && target){
            m_direction = (target.transform.position - laucher.transform.position).normalized;
            m_fired = true;
        }
    }
}
