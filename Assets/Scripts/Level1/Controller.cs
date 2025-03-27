
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Transform anchorTransform;
    [SerializeField] private RawImage signalImage;
    [SerializeField] private Texture2D[] signalSprites;
    public bool canMove = false;

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

    private void Awake()
    {
        controls = new GlobalControls();
        controls.Movement.Running.performed += ctx => Movement(ctx);
        controls.Movement.Running.canceled += ctx => Movement(ctx);
        controls.Movement.Jumping.performed += ctx => Jump();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (canMove)
        {
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;
            camForward.y = 0; // Ignore vertical tilt of the camera
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            movementDirection = camForward * movementInput.y + camRight * movementInput.x;
            movementDirection.Normalize();

            Vector3 targetVelocity = movementDirection * speed;
            velocity = Vector3.Lerp(velocity, targetVelocity, Time.deltaTime * (targetVelocity.magnitude > velocity.magnitude ? acceleration : deceleration));

            if (isGrounded)
            {
                if (isJumping)
                {
                    velocity.y = Mathf.Sqrt(2 * jumpHeight * -gravity);
                    isJumping = false;
                }
                else
                {
                    velocity.y = -0.1f; // Small value to keep grounded
                }
            }
            else
            {
                velocity.y += gravity * Time.deltaTime * 4;
            }

            HandleAnimation();
            HandleRotation();

            characterController.Move(velocity * Time.deltaTime);
            UpdateSignalStrength();
        }
    }

    private void UpdateSignalStrength()
    {
        if (signalImage == null || signalSprites.Length == 0 || anchorTransform == null)
            return;

        float distance = Vector3.Distance(transform.position, anchorTransform.position);
        float maxDistance = 50f; // Adjust as needed for signal scaling
        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);

        int spriteIndex = Mathf.RoundToInt((1 - normalizedDistance) * (signalSprites.Length - 1));
        signalImage.texture = signalSprites[spriteIndex];

        if (spriteIndex == 0)
        {
            EndGame.Instance.GameEnd(1, 0);
        }

        // Debug.Log(normalizedDistance);
    }

    void HandleAnimation()
    {
        animator.SetFloat("Movement", new Vector2(velocity.x, velocity.z).magnitude / speed);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt = new Vector3(velocity.x, 0f, velocity.z);

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);

            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Movement(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        isMovementPressed = movementInput.x != 0 || movementInput.y != 0;
    }

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

/*
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private GlobalControls controls;
    private Vector2 movementInput;
    private Vector3 movementDirection;
    private Vector3 velocity;
    private CharacterController characterController;
    private Camera mainCamera;
    private bool canMove = true;
    private Animator animator;
    private bool isMovementPressed;
    private bool isJumping;
    private bool isGrounded;
    private float jumpStartTime;
    private float initialY;

    private void Awake()
    {
        controls = new GlobalControls();
        controls.Movement.Running.performed += ctx => Movement(ctx);
        controls.Movement.Running.canceled += ctx => Movement(ctx);
        controls.Movement.Jumping.performed += ctx => Jump();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (canMove)
        {
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;
            camForward.y = 0; // Ignore vertical tilt of the camera
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            movementDirection = camForward * movementInput.y + camRight * movementInput.x;
            movementDirection.Normalize();
        }

        Vector3 targetVelocity = movementDirection * speed;
        velocity = Vector3.Lerp(velocity, targetVelocity, Time.deltaTime * (targetVelocity.magnitude > velocity.magnitude ? acceleration : deceleration));

        if (isJumping)
        {
            float timeSinceJump = Time.time - jumpStartTime;
            if (timeSinceJump < jumpDuration)
            {
                float jumpProgress = timeSinceJump / jumpDuration;
                float newY = initialY + Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
            else
            {
                isJumping = false;
            }
        }

        HandleAnimation();
        HandleRotation();
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleAnimation()
    {
        animator.SetFloat("Movement", new Vector2(velocity.x, velocity.z).magnitude / speed);
        animator.SetBool("IsJumping", isJumping);
    }

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

    private void Movement(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        isMovementPressed = movementInput.x != 0 || movementInput.y != 0;
    }

    private void Jump()
    {
        if (isGrounded && !isJumping)
        {
            isJumping = true;
            jumpStartTime = Time.time;
            initialY = transform.position.y;
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
*/