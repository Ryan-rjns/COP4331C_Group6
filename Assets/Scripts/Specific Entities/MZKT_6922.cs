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
        transform.LookAt(target);
        //transform.rotation *= Quaternion.FromToRotation(Vector3.left, -Vector3.forward);
    }
}
