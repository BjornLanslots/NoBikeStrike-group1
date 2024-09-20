using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bicycle : MonoBehaviour
{
    public float speed = 10f;           // Speed of the bicycle
    public float turnSpeed = 50f;       // Speed of turning
    public float brakeForce = 20f;      // Braking force

    private Rigidbody rb;               // Rigidbody component for physics

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the bicycle
    }

    // Update is called once per frame
    void Update()
    {
        // Handle forward and backward movement
        float move = Input.GetAxis("Vertical");  // Up/Down Arrow or W/S keys for forward/backward

        if (move > 0) // If pressing the up arrow, move forward
        {
            rb.AddForce(transform.forward * move * speed);
        }
        else if (move < 0) // If pressing the down arrow, apply brake
        {
            rb.AddForce(-transform.forward * brakeForce); // Apply brake force in the opposite direction
        }

        // Handle left and right turning
        float turn = Input.GetAxis("Horizontal");  // Left/Right Arrow or A/D keys for turning

        if (turn != 0) // If pressing the left or right arrow, rotate the bicycle
        {
            Vector3 rotation = Vector3.up * turn * turnSpeed * Time.deltaTime;
            transform.Rotate(rotation);
        }
    }
}
