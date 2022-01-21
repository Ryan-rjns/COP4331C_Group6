using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter_Propeller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Time.deltaTime * 360 * 2, 0);
    }
}
