using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingSystemFlat : Unit
{
    public float fireRate;
    public int damage;
    public float fieldOfView;
    public bool beam;
    public GameObject projectile;
    public GameObject target;
    public List<GameObject> projectileSpawns;
    List<GameObject> m_lastProjectiles = new List<GameObject>();
    float m_fireTimer = 0.0f;
    // Update is called once per frame
    protected override void Update(){
            base.Update();
            m_fireTimer += Time.deltaTime;
            if(m_fireTimer >= fireRate && target != null){
                //Vector3( target.transform.position.x, this.transform.position.y, target.transform.position.z );
                float angle = Quaternion.Angle(transform.rotation,Quaternion.LookRotation(new Vector3( target.transform.position.x, this.transform.position.y, target.transform.position.z ) - transform.position));
                if(angle < fieldOfView){
                    //SpawnProjectiles();

                    m_fireTimer = 0.0f;
                }
            }
    }

    void SpawnProjectiles(){
        for(int i = 0; i < projectileSpawns.Count;i++){
            if(projectileSpawns[i]){
                GameObject proj = GameObject.Instantiate(projectile, projectileSpawns[i].transform.position, Quaternion.Euler(projectileSpawns[i].transform.forward)) as GameObject;
                proj.GetComponent<BaseProjectile>().owner = this;
                proj.GetComponent<BaseProjectile>().power = damage;
                proj.GetComponent<BaseProjectile>().FireProjectile(projectileSpawns[i], target, damage);

                m_lastProjectiles.Add(proj);
            }
        }
    }
}
