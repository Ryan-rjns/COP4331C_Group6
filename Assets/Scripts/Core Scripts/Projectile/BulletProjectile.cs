using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : BaseProjectile
{
    GameObject m_launcher;
    GameObject m_target;
    void Update(){
        if(m_launcher){
            GetComponent<LineRender>().SetPosition(0,m_target.transform.position);
        }
    }

    public override void FireProjectile(GameObject launcher, GameObject target, int damage){
        if(launcher){
            m_launcher = launcher;
            m_target = target;
        }
    }
}
