using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float gravity = -15f;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeedMultiplier = 0.75f;
    [SerializeField] private float crouchHeightMultiplier = 0.5f;
    [SerializeField] private float crouchTransitionTime = 0.15f;
    [SerializeField] private Transform meshRoot; // optional visual child to scale/offset

    [Header("Camera")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float rotationSmoothTime = 0.05f;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Camera Collision")]
    [SerializeField] private float cameraRadius = 0.2f;
    [SerializeField] private float cameraCollisionSmooth = 0.05f;

    private float currentCameraDistance;



    private CharacterController controller;
    private PlayerInput playerInput;

    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isTransitioning;

    private float yaw;
    private float pitch;
    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;

    private float defaultHeight;
    private Vector3 defaultCenter;

    private Vector3 meshDefaultLocalPos;
    private Vector3 meshDefaultLocalScale;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null) playerInput.enabled = true;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();

       
        defaultHeight = controller.height;
        defaultCenter = controller.center;

        if (meshRoot != null)
        {
            meshDefaultLocalPos = meshRoot.localPosition;
            meshDefaultLocalScale = meshRoot.localScale;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentCameraDistance = cameraDistance;

    }

    private void Update()
    {
        HandleLook();
        CheckGrounded();
        HandleMovement();
        HandleRotation();
        HandleCameraPosition();
        ApplyGravity();
    }

    private void HandleLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * sensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * sensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
    }

    private void HandleCameraPosition()
    {
        if (cameraTarget == null || playerCamera == null) return;


        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);


        Vector3 desiredOffset = rotation * new Vector3(0f, cameraHeight, -cameraDistance);
        Vector3 desiredCameraPos = cameraTarget.position + desiredOffset;


        Vector3 direction = desiredCameraPos - cameraTarget.position;
        float distance = direction.magnitude;

        if (Physics.SphereCast(cameraTarget.position, cameraRadius, direction.normalized, out RaycastHit hit, distance))
        {
            float adjustedDistance = Mathf.Max(0.1f, hit.distance - cameraRadius);

            currentCameraDistance = Mathf.Lerp(currentCameraDistance, adjustedDistance, cameraCollisionSmooth);
        }
        else
        {
            currentCameraDistance = Mathf.Lerp(currentCameraDistance, cameraDistance, cameraCollisionSmooth);
        }

        Vector3 finalOffset = rotation * new Vector3(0f, cameraHeight, -currentCameraDistance);
        playerCamera.position = cameraTarget.position + finalOffset;
        playerCamera.rotation = rotation;
    }


    private void CheckGrounded()
    {
        Vector3 spherePosition = transform.position + controller.center - Vector3.up * (controller.height * 0.5f);
        isGrounded = Physics.CheckSphere(spherePosition, 0.2f, ~0, QueryTriggerInteraction.Ignore);
        if (isGrounded && velocity.y < 0f) velocity.y = -2f;
    }

    private void HandleMovement()
    {
        Vector3 forward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0f, yaw, 0f) * Vector3.right;

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        float speed = walkSpeed * (isCrouching ? crouchSpeedMultiplier : 1f);
        controller.Move(moveDirection * speed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed && !isTransitioning)
        {
            if (isCrouching && !CanStandUp())
            {
                Debug.Log("No hay suficiente espacio para pararse!");
                return; 
            }

            StartCoroutine(CrouchTransitionCoroutine());
        }
    }


    private IEnumerator CrouchTransitionCoroutine()
    {
        isTransitioning = true;
        bool targetCrouchState = !isCrouching; 

        float startHeight = controller.height;
        float targetHeight = targetCrouchState ? defaultHeight * crouchHeightMultiplier : defaultHeight;

        
        float startBottomWorldY = transform.position.y + controller.center.y - (controller.height * 0.5f);

    
        float targetCenterY = startBottomWorldY - transform.position.y + (targetHeight * 0.5f);
        float startCenterY = controller.center.y;

        Vector3 meshStartPos = meshRoot != null ? meshRoot.localPosition : Vector3.zero;
        Vector3 meshTargetPos = meshRoot != null ? meshDefaultLocalPos - new Vector3(0f, (defaultHeight - targetHeight) * 0.5f, 0f) : Vector3.zero;

        Vector3 meshStartScale = meshRoot != null ? meshRoot.localScale : Vector3.one;
        Vector3 meshTargetScale = meshRoot != null ? new Vector3(meshDefaultLocalScale.x, targetCrouchState ? meshDefaultLocalScale.y * crouchHeightMultiplier : meshDefaultLocalScale.y, meshDefaultLocalScale.z) : Vector3.one;

        float elapsed = 0f;
        while (elapsed < crouchTransitionTime)
        {
            float t = elapsed / crouchTransitionTime;

            float h = Mathf.Lerp(startHeight, targetHeight, t);
            float cY = Mathf.Lerp(startCenterY, targetCenterY, t);

            controller.height = h;
            controller.center = new Vector3(defaultCenter.x, cY, defaultCenter.z);

            if (meshRoot != null)
            {
                meshRoot.localPosition = Vector3.Lerp(meshStartPos, meshTargetPos, t);
                meshRoot.localScale = Vector3.Lerp(meshStartScale, meshTargetScale, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        controller.height = targetHeight;
        controller.center = new Vector3(defaultCenter.x, targetCenterY, defaultCenter.z);

        if (meshRoot != null)
        {
            meshRoot.localPosition = meshTargetPos;
            meshRoot.localScale = meshTargetScale;
        }

        isCrouching = targetCrouchState;
        isTransitioning = false;
    }

    private bool CanStandUp()
    {
        float targetHeight = defaultHeight;
        float radius = controller.radius;

        Vector3 centerWhenStanding = new Vector3(
            controller.center.x,
            defaultCenter.y,
            controller.center.z
        );

        Vector3 point1 = transform.position + centerWhenStanding - Vector3.up * (targetHeight / 2f - radius);

        Vector3 point2 = transform.position + centerWhenStanding + Vector3.up * (targetHeight / 2f - radius);

            return !Physics.CheckCapsule(point1, point2, radius, ~0, QueryTriggerInteraction.Ignore);
    }

}
