using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MZKT_6922 : MonoBehaviour
{
    // Start is called before the first frame update
    Transform target = null;

    public int turnRate = 20;

    private void Start(){
        target = (GameObject.FindWithTag("Player")).GetComponent<Transform>();
    }

    private void FixedUpdate(){
        Vector3 oldRot = transform.rotation.eulerAngles;
        transform.LookAt(target);
        Vector3 newRot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(oldRot.x, newRot.y, oldRot.z));
    }
}
