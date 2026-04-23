using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
    private PlayerInputActions inputActions;
    private EnergySystem energySystem;
    private PlayerAnimator playerAnimator;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    private float currentSpeed;
    private Vector2 movementInput;

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isSprinting;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float consumeEnergyAmount = 10;

    public bool isMoving;
    public bool isJumping;
    public bool isAllowToMove;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<PlayerAnimator>();
        isAllowToMove = true;
        energySystem = GetComponent<EnergySystem>();
        currentSpeed = walkSpeed;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += ctx => HandleMovement();
        inputActions.Player.Jump.performed += ctx => Jump();
        inputActions.Player.Sprint.started += Sprint;
        inputActions.Player.Sprint.canceled += Sprint;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Move.performed -= ctx => HandleMovement();
        inputActions.Player.Jump.performed -= ctx => Jump();
        inputActions.Player.Sprint.started -= Sprint;
        inputActions.Player.Sprint.canceled -= Sprint;
    }

    private void FixedUpdate()
    {
        HandleMovement();
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
        if (isGrounded && isAllowToMove)
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
            bool isRunning = isSprinting && isMoving && energySystem.ConsumeEnergy(consumeEnergyAmount * Time.deltaTime, 1);
            currentSpeed = isRunning ? runSpeed : walkSpeed;
            playerAnimator.HandlePlayerAnimator(isMoving, isRunning);
            energySystem.isPlayerMoving = isSprinting; // Set status bergerak berdasarkan input
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
}
