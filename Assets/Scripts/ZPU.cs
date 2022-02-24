using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZPU : MonoBehaviour{

    public int health;
    public int damage; 
    
    private GameObject target;
    private bool targetLocked;
    //This is the top of the ZPU turret, it consisted of a ton of objects, so I added them to an array
    public GameObject[] turretTop = {Box01,Box02,Box03,Box06,Box07,Box08,Box09,Box10,Box11,Box12,Box13,Box14,Box15,Box16,Box17,Box18,Box19,Box20,Box21,Box22,Box23,Box24,Box25, Cylinder01,Cylinder02,Cylinder03,Cylinder04,Cylinder05,Cylinder06,Cylinder07,Cylinder08,Cylinder09,Tube10,Tube11,Tube15,Tube16,Tube17,Tube18,Tube19,Tube20,Tube21,Tube22,Tube23,Tube24};
    int turretToplen = turretTop.Length;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        if(targetLocked){
            turretTop.transform.LookAt(target.transform);
        }
        
    }
}
