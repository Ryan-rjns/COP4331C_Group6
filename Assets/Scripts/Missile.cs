using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // The axial force exerted on the missile 
    private const float MISSILE_FORCE = 5000;
    // Missile Rigidbody
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        // Set missile's rigidbody component
        rb = transform.GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        // Apply missile force
        rb.AddRelativeForce(Vector3.forward * MISSILE_FORCE);
    }
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
