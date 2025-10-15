using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -15f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer = -1;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTarget;  
    [SerializeField] private Transform playerCamera;  
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float rotationSmoothTime = 0.05f;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;

    // Components
    private CharacterController controller;

    // Input
    private Vector2 moveInput;
    private bool isRunning;
    private bool jumpPressed;

    // Movement
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;

    // Camera rotation
    private float yaw;
    private float pitch;
    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null && playerInput.defaultActionMap != null)
        {
            playerInput.enabled = true;
        }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        CheckGrounded();
        HandleMovement();
        HandleRotation();
        HandleCameraPosition();
    }

    private void HandleLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * sensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * sensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Smooth the rotation
        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
    }

    private void HandleCameraPosition()
    {
        if (!cameraTarget || !playerCamera) return;

        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        playerCamera.rotation = rotation;

        Vector3 offset = rotation * new Vector3(0, cameraHeight, -cameraDistance);
        playerCamera.position = cameraTarget.position + offset;
    }

    private void CheckGrounded()
    {
        Vector3 spherePosition = transform.position - new Vector3(0, controller.height / 2, 0);
        isGrounded = Physics.CheckSphere(spherePosition, groundCheckDistance, groundLayer);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    private void HandleMovement()
    {
        Vector3 forward = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0, yaw, 0) * Vector3.right;

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        currentSpeed = isRunning ? runSpeed : walkSpeed;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        // Player faces same yaw as camera
        Quaternion targetRotation = Quaternion.Euler(0, yaw, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
    }

    //Lo saco por el momento porque me dijeron que Mati pide que no tenga jump
    private void HandleGravityAndJump()
    {
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Input System events
    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void OnRun(InputAction.CallbackContext context) => isRunning = context.ReadValueAsButton();
    public void OnJump(InputAction.CallbackContext context) { if (context.performed) jumpPressed = true; }
}