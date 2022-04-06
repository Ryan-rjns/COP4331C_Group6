using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : BaseProjectile
{
    GameObject m_launcher;
    GameObject m_target;
    protected override void Update(){
        base.Update();
        if(m_launcher){
            //GetComponent<LineRender>().SetPosition(0,m_target.transform.position);
        }
    }

    public override void FireProjectile(GameObject launcher, GameObject target, int damage){
        if(launcher){
            m_launcher = launcher;
            m_target = target;
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(launcher.transform.rotation.eulerAngles), out hit, Mathf.Infinity)){
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
        }
    }
}
