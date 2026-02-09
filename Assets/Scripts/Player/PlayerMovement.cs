using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
    private PlayerInputActions inputActions;
    [SerializeField] private float speed = 5f;
    public ForceMode forceMode = ForceMode.Force;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float jumpForce = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        inputActions = new PlayerInputActions();
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
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 moveDir = (transform.right * input.x + transform.forward * input.y).normalized;
        rb.AddForce(moveDir * speed, forceMode);

        // Batasi kecepatan horizontal saja (X dan Z)
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        int roundedSpeed = Mathf.RoundToInt(horizontalVelocity.magnitude);

        if (horizontalVelocity.magnitude > speed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * speed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
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
}
