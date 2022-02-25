using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZPU : MonoBehaviour{

    public int health;
    public int damage; 
    
    private GameObject Helicopter = GameObject.FindGameObjectsWithTag("Player")[0];
    private bool targetLocked;
    //This is the top of the ZPU turret, it consisted of a ton of objects, so I added them to an array
    public static GameObject[] turretTop = GameObject.FindGameObjectsWithTag("zputop");
    int turretToplen = turretTop.Length;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate(){
        if(targetLocked){
            //turretTop.transform.LookAt(target.transform);
        }
        
    }
}
