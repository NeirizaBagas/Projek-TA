using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
    private PlayerInputActions inputActions;
    private EnergySystem energySystem;
    private float currentSpeed;
    private Vector2 movementInput;

    [Header("Jumping & Moving")]
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isSprinting;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float consumeEnergyAmount = 10;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2.5f;

    [Header("Crouching")]
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchingHeight = 1f;
    [SerializeField] private float standingCamHeight = 1.5f;
    [SerializeField] private float crouchingCamHeight = 0.5f;
    private Transform playerCam;
    private CapsuleCollider playerCollider;


    [Header("Bool Reference")]
    public bool isMoving;
    public bool isJumping;
    public bool isAllowToMove;
    public static bool isCrouching { get; private set; }

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        isCrouching = false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        isAllowToMove = true;
        energySystem = GetComponent<EnergySystem>();
        currentSpeed = walkSpeed;
        playerCam = Camera.main.transform;
        playerCollider = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += ctx => HandleMovement();
        inputActions.Player.Jump.performed += ctx => Jump();
        inputActions.Player.Sprint.started += Sprint;
        inputActions.Player.Sprint.canceled += Sprint;
        inputActions.Player.Crouch.started += Crouch;
        inputActions.Player.Crouch.canceled += Crouch;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Move.performed -= ctx => HandleMovement();
        inputActions.Player.Jump.performed -= ctx => Jump();
        inputActions.Player.Sprint.started -= Sprint;
        inputActions.Player.Sprint.canceled -= Sprint;
        inputActions.Player.Crouch.started -= Crouch;
        inputActions.Player.Crouch.canceled -= Crouch;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleCrouchMode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.started) isSprinting = true;
        else if (context.canceled) isSprinting = false;
    }

    private void Jump()
    {
        bool canJump = energySystem.ConsumeEnergy(consumeEnergyAmount, 1);
        if (isGrounded && isAllowToMove && canJump)
        {
            isJumping = true;
            playerAnimator.HandleJumping(isJumping);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void HandleMovement()
    {
        if (isAllowToMove)
        {
            Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
            isMoving = input.magnitude > 0.1f; // Cek apakah ada input gerakan
            bool isRunning = !isCrouching && isSprinting && isMoving && energySystem.ConsumeEnergy(consumeEnergyAmount * Time.deltaTime, 1);

            if (isCrouching) currentSpeed = crouchSpeed;
            else if (isRunning) currentSpeed = runSpeed;
            else currentSpeed = walkSpeed;

            playerAnimator.HandlePlayerAnimator(isMoving, isRunning);
            energySystem.isPlayerMoving = isSprinting && !isCrouching; // Set status bergerak berdasarkan input
            Vector3 moveDir = (transform.right * input.x + transform.forward * input.y).normalized;

            rb.AddForce(moveDir * currentSpeed, ForceMode.Force);

            //Batasi kecepatan horizontal saja (X dan Z)
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            int roundedSpeed = Mathf.RoundToInt(horizontalVelocity.magnitude);

            if (horizontalVelocity.magnitude > currentSpeed)
            {
                Vector3 limitedVelocity = horizontalVelocity.normalized * currentSpeed;
                rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
            }
        }
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        if (context.started) { isCrouching = true; }
        else if (context.canceled) { isCrouching = false; }
    }

    private void HandleCrouchMode()
    {
        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        float targetCamHeight = isCrouching ? crouchingCamHeight : standingCamHeight;

        Vector3 targetCenter = new Vector3(0, targetHeight / 2f, 0);

        playerCollider.height = Mathf.Lerp(playerCollider.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        playerCollider.center = Vector3.Lerp(playerCollider.center, targetCenter, Time.deltaTime * crouchTransitionSpeed);

        Vector3 camLocalPos = playerCam.localPosition;
        camLocalPos.y = Mathf.Lerp(camLocalPos.y, targetCamHeight, Time.deltaTime * crouchTransitionSpeed);
        playerCam.localPosition = camLocalPos;
    }
}
