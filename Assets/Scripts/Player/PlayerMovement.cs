using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
    private PlayerInputActions inputActions;
    private EnergySystem energySystem;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    private float currentSpeed;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float jumpForce = 5f;

    private bool isMoving;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        
        energySystem = GetComponent<EnergySystem>();
        currentSpeed = walkSpeed;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += ctx => Jump();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void Movement()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        isMoving = input.magnitude > 0.1f; // Cek apakah ada input gerakan
        Vector3 moveDir = (transform.right * input.x + transform.forward * input.y).normalized;

        rb.AddForce(moveDir * currentSpeed, ForceMode.Force);

        // Batasi kecepatan horizontal saja (X dan Z)
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        int roundedSpeed = Mathf.RoundToInt(horizontalVelocity.magnitude);

        if (horizontalVelocity.magnitude > walkSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * walkSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
        CheckEnergy();
    }

    private void CheckEnergy()
    {
        if (isMoving)
        {
            energySystem.isPlayerMoving = true;
            if (!energySystem.ConsumeEnergy(10 * Time.deltaTime))
            {
                // Jika energi habis, hentikan pergerakan
                Debug.Log("Energy depleted! Stopping movement.");
            }
        }
        else
        {
            energySystem.isPlayerMoving = false;
        }
    }
}
