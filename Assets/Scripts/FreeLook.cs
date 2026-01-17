using UnityEngine;
using UnityEngine.InputSystem;

public class FreeLook : MonoBehaviour
{
    private Transform playerCam;
    public float mouseSensitivity;
    private Vector3 targetLook;
    private Vector3 currentLook;
    private PlayerInput playerInput;
    private PlayerInputActions inputActions;
    public bool canLook;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerCam = GetComponentInChildren<Camera>().transform;
        playerInput = GetComponent<PlayerInput>();
        inputActions = new PlayerInputActions();
        inputActions.UI.Disable();
        canLook = true;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Look.Enable();
        
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();


    }

    // Update is called once per frame
    void Update()
    {
        if (canLook)
        {
            Look();
        }
    }

    public void Look()
    {
        Vector2 mouseDelta = inputActions.Player.Look.ReadValue<Vector2>(); // Ngebaca input vektor dari mouse

        targetLook.x += mouseDelta.x * mouseSensitivity; // Update rotasi horizontal ngikutin gerakan mouse dan vektor
        targetLook.y -= mouseDelta.y * mouseSensitivity; // Update rotasi vertikal ngikutin gerakan mouse dan vektor

        targetLook.y = Mathf.Clamp(targetLook.y, -90f, 90f); // Batasi rotasi vertikal antara -90 dan 90 derajat

        currentLook.x = Mathf.Lerp(currentLook.x, targetLook.x, Time.deltaTime * 10f); // Interpolasi rotasi horizontal biar smooth
        currentLook.y = Mathf.Lerp(currentLook.y, targetLook.y, Time.deltaTime * 10f); // Interpolasi rotasi vertikal biar smooth

        transform.rotation = Quaternion.Euler(0f, currentLook.x, 0f); // Terapkan rotasi horizontal ke player
        playerCam.localRotation = Quaternion.Euler(currentLook.y, 0f, 0f); // Terapkan rotasi vertikal ke kamera
    }
}
