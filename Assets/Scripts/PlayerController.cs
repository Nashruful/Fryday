using UnityEngine;

// This attribute automatically adds a CharacterController if one is not already on the GameObject.
// It also prevents you from accidentally removing it, which would break the script.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Object References")]
    [Tooltip("Assign the 'CameraPivot' empty GameObject here.")]
    public Transform cameraPivot;
    [Tooltip("Assign the 'Model' GameObject which contains your chicken's mesh.")]
    public Transform characterModel;

    [Header("Movement")]
    [Tooltip("The walking speed of the character.")]
    public float speed = 5.0f;
    [Tooltip("How quickly the character model turns to face the movement direction.")]
    public float rotationSpeed = 15f;

    [Header("Camera Control")]
    [Tooltip("How sensitive the camera is to mouse movement.")]
    public float lookSpeed = 2f;
    [Tooltip("How smoothly the camera follows the mouse. Lower values are smoother.")]
    public float cameraSmoothSpeed = 10f;
    [Tooltip("The steepest angle you can look down.")]
    public float minPitch = -45f;
    [Tooltip("The steepest angle you can look up.")]
    public float maxPitch = 75f;

    [Header("Physics & Jumping")]
    [Tooltip("The strength of standard gravity.")]
    public float standardGravity = -9.81f;
    [Tooltip("The gentle gravity used when the chicken is falling, creating a floaty effect.")]
    public float floatingGravity = -2.0f;
    [Tooltip("How high the chicken can jump from a standstill.")]
    public float jumpHeight = 1.5f;
    [Tooltip("The force of the jump when escaping the chef's pan.")]
    public float escapeJumpForce = 5f;

    // Private variables for internal script logic
    private CharacterController controller;
    private float yVelocity = 0f;
    private bool isImmobilized = false;

    // Variables for storing and smoothing camera rotation
    private float targetYaw = 0f;
    private float targetPitch = 0f;
    private float currentYaw = 0f;
    private float currentPitch = 0f;

    void Start()
    {
        // Get the CharacterController component attached to this GameObject
        controller = GetComponent<CharacterController>();

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // If the player is immobilized (e.g., has fallen on the ground),
        // skip all movement and camera control logic.
        if (isImmobilized) return;

        HandleCameraRotation();
        HandleMovement();
    }

    void HandleCameraRotation()
    {
        // Get mouse input and accumulate the target rotation
        targetYaw += Input.GetAxis("Mouse X") * lookSpeed;
        targetPitch -= Input.GetAxis("Mouse Y") * lookSpeed;

        // Clamp the vertical (pitch) rotation to prevent the camera from flipping over
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        // Smoothly interpolate the current rotation values towards the target values
        currentYaw = Mathf.Lerp(currentYaw, targetYaw, cameraSmoothSpeed * Time.deltaTime);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, cameraSmoothSpeed * Time.deltaTime);

        // Apply the rotation. The parent object handles horizontal (yaw) rotation for camera orbit.
        transform.eulerAngles = new Vector3(0, currentYaw, 0);
        // The camera pivot handles vertical (pitch) rotation.
        cameraPivot.localEulerAngles = new Vector3(currentPitch, 0, 0);
    }

    void HandleMovement()
    {
        // --- Gravity and Jumping ---
        if (controller.isGrounded)
        {
            // If we are on the ground, reset vertical velocity to a small negative value
            // This ensures the CharacterController stays firmly on the ground.
            if (yVelocity < 0)
            {
                yVelocity = -2f;
            }

            // Check for the jump button press (default is Spacebar)
            if (Input.GetButtonDown("Jump"))
            {
                // Calculate the upward velocity needed to reach the desired jump height using a physics formula
                yVelocity = Mathf.Sqrt(jumpHeight * -2f * standardGravity);
            }
        }

        // --- "Chicken Float" Gravity Logic ---
        // Determine which gravity value to use
        float currentGravity;
        if (!controller.isGrounded && yVelocity < 0)
        {
            // If we are in the air AND falling, use the gentle floating gravity
            currentGravity = floatingGravity;
        }
        else
        {
            // Otherwise (going up from a jump or on the ground), use standard gravity
            currentGravity = standardGravity;
        }

        // Apply the chosen gravity to our vertical velocity over time
        yVelocity += currentGravity * Time.deltaTime;


        // --- Horizontal Movement ---
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            // Calculate the movement direction relative to the camera's current rotation
            Vector3 moveDirection = Quaternion.Euler(0, targetYaw, 0) * inputDirection;

            // Move the entire CharacterController in the calculated direction
            controller.Move(moveDirection * speed * Time.deltaTime);

            // Rotate the separate character model to face the direction it is moving
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            characterModel.rotation = Quaternion.Slerp(characterModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Apply the final vertical movement (from gravity and jumping)
        controller.Move(new Vector3(0, yVelocity, 0) * Time.deltaTime);
    }

    // This function is called automatically by the CharacterController whenever it collides with something
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check the tag of the object we are standing on or bumped into
        string objectTag = hit.gameObject.tag;

        if (objectTag == "Countertop")
        {
            isImmobilized = false;
        }
        else if (objectTag == "Ground")
        {
            isImmobilized = true;
        }
    }

    // This public function can be called from other scripts (like PanEscape)
    public void PerformJump()
    {
        // Set the vertical velocity directly to the escape jump force
        yVelocity = escapeJumpForce;
    }
}