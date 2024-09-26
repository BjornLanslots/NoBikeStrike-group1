using UnityEngine;

public class RotateWithSpeed : MonoBehaviour
{
    public float speed = 5f; // Speed of movement
    public float rotationFactor = 100f; // Factor to determine how much to rotate based on speed

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get the forward movement
        float moveInput = Input.GetAxis("Vertical");
        
        // Calculate the movement vector
        Vector3 moveDirection = transform.forward * moveInput * speed * Time.deltaTime;

        // Move the object
        rb.MovePosition(rb.position + moveDirection);

        // Rotate based on the speed (the magnitude of the movement vector)
        float currentSpeed = moveDirection.magnitude / Time.deltaTime; // Get the speed in units per second
        float rotationAmount = currentSpeed * rotationFactor * Time.deltaTime;

        // Rotate around the X-axis
        transform.Rotate(rotationAmount, 0, 0);
    }
}
