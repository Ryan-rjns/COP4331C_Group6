using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    // Boundary floats
    private const float BOUNDARY_MAX_Z = 250.0f;
    private const float BOUNDARY_MAX_X = 250.0f;
    // Maximum time that the helicopter can be out-of-bounds
    private const float TIMEOUT = 5.0f;
    // Out-of-bounds counter
    private float timer = 0.0f;
    // Player Helicopter
    private GameObject player;
    // Player's x location
    private float x;
    // Player's z location
    private float z;
    // Start is called before the first frame update
    void Start()
    {
        // Find player GameObject
        player = GameObject.FindWithTag("Player");
        // Check if GameObject is valid
        if(player != null) {
            // Initialize x and z
            x = player.transform.position.x;
            z = player.transform.position.z;
        }  
    }
    // Update is called once per frame
    void Update()
    {  
        // Check for timer timeout
        if(timer > TIMEOUT) {
            // Kill player GameObject and halt execution
            Destroy(player);
            Debug.Log("GAMEOVER!");
            return;
        }
        // Update x and z from player's location
        x = player.transform.position.x;
        z = player.transform.position.z;
        // Check if player is out-of-bounds
        if (x < -BOUNDARY_MAX_X || x > BOUNDARY_MAX_X || z < -BOUNDARY_MAX_Z || z > BOUNDARY_MAX_Z) {
            // Advance timer
            timer += Time.deltaTime;
        } else {
            // Reset timer
            timer = 0.0f;
        } 
    }
}
