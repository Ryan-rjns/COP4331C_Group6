using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeChecking : MonoBehaviour{


    GameObject target;
    void OnTriggerEnter(Collider other){
        bool invalid = true;
        if(other.CompareTag("Player")){
            invalid = false;
        }
        if(invalid){
            return;
        }
        target = other.gameObject;
    }
    void OnTriggerExit(Collider other){
        if(other.CompareTag("Player")){
            target = null;
        }
    }

    public bool inRange(GameObject obj){
        if(obj == target){
            return true;
        }
        return false;
    }
}
