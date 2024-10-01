using UnityEngine;

public class BikeCameraFollow : MonoBehaviour
{
    public Transform target; // The bike to follow
    public float height = 1.5f; // Height above the bike
    public float distance = 0f; // Distance behind the bike (optional, can be set to 0 for close follow)
    public float mouseSensitivity = 100f; // Sensitivity of mouse movement
    public float verticalLookLimit = 30f; // Limit for looking up and down

    private float xRotation = 0f; // To store the vertical rotation
    private float yRotation = 0f; // To store the horizontal rotation
    private bool isFollowing = true; // To track if the camera is following the bike

    void Start()
    {
        // Set the initial position and rotation of the camera
        transform.position = new Vector3(-2f, 2f, 10f);
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
    }

    void Update()
    {
        // Toggle following when the J key is pressed
        if (Input.GetKeyDown(KeyCode.J))
        {
            isFollowing = !isFollowing;
            Cursor.lockState = isFollowing ? CursorLockMode.Locked : CursorLockMode.None; // Lock/unlock the cursor
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // If following the bike, update the position and rotation accordingly
        if (isFollowing)
        {
            // Mouse input for looking up and down and left and right
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

            // Update vertical rotation and clamp it
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);

            // Update horizontal rotation
            yRotation += mouseX;

            // Set the camera's position directly above the bike at the specified height
            Vector3 newPosition = target.position + Vector3.up * height;
            newPosition.z -= distance; // Optional: move the camera back if distance is set

            // Directly set the camera's position
            transform.position = newPosition;

            // Calculate the new rotation based on bike's rotation and mouse input
            Quaternion targetRotation = Quaternion.Euler(xRotation, yRotation + target.eulerAngles.y, 0f);
            transform.rotation = targetRotation;
        }
    }
}
