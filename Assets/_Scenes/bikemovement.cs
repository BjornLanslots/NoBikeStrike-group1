using UnityEngine;

public class BikeMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the bike
    public float turnSpeed = 200f; // Speed of turning
    public Transform wheelB; // Assign Wheel_B transform in the inspector
    public Transform wheelF; // Assign Wheel_F transform in the inspector
    public Transform steerTransform; // Assign the steer transform in the inspector
    public float rotationFactor = 360f; // Factor for wheel rotation speed
    public float steerRotationAmount = 30f; // Maximum steering angle
    private Quaternion steerOffset; // Store the offset rotation for steering

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Create a quaternion representing the -30.542 degrees rotation around the X-axis
        steerOffset = Quaternion.Euler(-30.542f, 0f, 0f);
    }

    void Update()
    {
        // Get input from W, A, S, D keys
        float moveVertical = Input.GetAxis("Vertical"); // W/S keys
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D keys

        // Calculate movement direction
        Vector3 forwardMovement = transform.forward * moveVertical * moveSpeed * Time.deltaTime;

        // Move the bike
        rb.MovePosition(rb.position + forwardMovement);

        // Rotate the bike
        if (moveHorizontal != 0)
        {
            float turnAmount = moveHorizontal * turnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // Rotate both wheels based on the speed
        if (moveVertical > 0)
        {
            RotateWheels(forwardMovement.magnitude / Time.deltaTime); // Pass the current speed
        }

        // Rotate the steering object based on horizontal input
        RotateSteer(moveHorizontal);
    }

    void RotateWheels(float currentSpeed)
    {
        // Calculate rotation amount based on current speed
        float rotationAmount = currentSpeed * rotationFactor * Time.deltaTime;

        // Rotate Wheel_B around the X-axis
        wheelB.Rotate(rotationAmount, 0, 0); // Rotate around X-axis

        // Rotate Wheel_F around the X-axis
        wheelF.Rotate(rotationAmount, 0, 0); // Rotate around X-axis

        // Match Wheel_F's rotation to the steerTransform's Y rotation, while keeping the X rotation of Wheel_F
        wheelF.rotation = Quaternion.Euler(wheelF.eulerAngles.x, steerTransform.eulerAngles.y, steerTransform.eulerAngles.z);
    }

    void RotateSteer(float horizontalInput)
    {
        // Calculate the steering angle and apply the steer offset
        float steerAngle = horizontalInput * steerRotationAmount;

        // Apply the rotation to the steer transform around its local Y-axis with the offset
        steerTransform.localRotation = steerOffset * Quaternion.Euler(0, steerAngle, 0);
    }
}
