using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    #region # Movement and Physics Parameters
    [SerializeField] private float speed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;
    #endregion

    #region # Ground Check Parameters
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    #endregion

    #region # UI and Anchor References
    [SerializeField] private Transform anchorTransform;
    [SerializeField] private RawImage signalImage;
    [SerializeField] private Texture2D[] signalSprites;
    #endregion

    // Public flag to control whether movement is allowed
    public bool canMove = false;

    #region # Private Variables
    private GlobalControls controls;
    private Vector2 movementInput;
    private Vector3 movementDirection;
    private Vector3 velocity;
    private CharacterController characterController;
    private Camera mainCamera;
    private Animator animator;
    private bool isMovementPressed;
    private bool isJumping;
    private bool isGrounded;
    #endregion

    private void Awake()
    {
        // Initialize the control scheme and set up input callbacks
        controls = new GlobalControls();
        controls.Movement.Running.performed += ctx => Movement(ctx);
        controls.Movement.Running.canceled += ctx => Movement(ctx);
        controls.Movement.Jumping.performed += ctx => Jump();
    }

    void Start()
    {
        // Get essential components and lock the cursor (nobody likes a runaway cursor)
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Check if the character is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (canMove)
        {
            // Calculate Movement Direction
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;
            camForward.y = 0; // Ignore vertical tilt – keepin' it level
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            movementDirection = camForward * movementInput.y + camRight * movementInput.x;
            movementDirection.Normalize();

            // Smooth Velocity Transition (Acceleration/Deceleration)
            Vector3 targetVelocity = movementDirection * speed;
            velocity = Vector3.Lerp(velocity, targetVelocity, Time.deltaTime * (targetVelocity.magnitude > velocity.magnitude ? acceleration : deceleration));

            // Handle Jump and Gravity
            if (isGrounded)
            {
                if (isJumping)
                {
                    velocity.y = Mathf.Sqrt(2 * jumpHeight * -gravity);
                    isJumping = false;
                }
                else
                {
                    velocity.y = Mathf.Max(velocity.y, -2f); // Apply a small constant downward force to prevent floating
                }
            }
            else
            {
                velocity.y += gravity * Time.deltaTime * 5;
            }


            // Update Animation and Rotation
            HandleAnimation();
            HandleRotation();

            // Move Character and Update Signal UI
            characterController.Move(velocity * Time.deltaTime);
            UpdateSignalStrength();
        }
    }

    // UI Update Section: Adjusts the signal strength image based on distance from the anchor
    private void UpdateSignalStrength()
    {
        if (signalImage == null || signalSprites.Length == 0 || anchorTransform == null)
            return;

        float distance = Vector3.Distance(transform.position, anchorTransform.position);
        float maxDistance = 50f;
        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);

        int spriteIndex = Mathf.RoundToInt((1 - normalizedDistance) * (signalSprites.Length - 1));
        signalImage.texture = signalSprites[spriteIndex];

        // If signal is completely lost, trigger game end
        if (spriteIndex == 0)
        {
            EndGame.Instance.GameEnd(1, 0, "-1");
        }
    }

    // Animation Section: Updates animation parameters based on movement
    void HandleAnimation()
    {
        animator.SetFloat("Movement", new Vector2(velocity.x, velocity.z).magnitude / speed);
        animator.SetBool("IsGrounded", isGrounded);
    }

    // Rotation Section: Smoothly rotates the character to face the movement direction
    void HandleRotation()
    {
        Vector3 positionToLookAt = new Vector3(velocity.x, 0f, velocity.z);
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    //  Input Handling Section: Processes movement input
    private void Movement(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        isMovementPressed = movementInput.x != 0 || movementInput.y != 0;
    }

    // Input Handling Section: Initiates a jump if grounded
    private void Jump()
    {
        if (isGrounded)
        {
            isJumping = true;
        }
    }

    private void OnEnable()
    {
        controls.Movement.Enable();
    }

    private void OnDisable()
    {
        controls.Movement.Disable();
    }
}
