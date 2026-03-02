using UnityEngine;
using UnityEngine.InputSystem;

public class FreeLook : MonoBehaviour
{
    private Transform playerCam;
    public float mouseSensitivity;
    [SerializeField] private float lerpSpeed = 10f;
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
        inputActions.Player.Look.Disable();

    }

    // Update is called once per frame
    void Update()
    {
        Look();
    }

    public void Look()
    {
        // Cek input HANYA jika canLook aktif
        if (canLook)
        {
            Vector2 mouseDelta = inputActions.Player.Look.ReadValue<Vector2>();

            targetLook.x += mouseDelta.x * mouseSensitivity;
            targetLook.y -= mouseDelta.y * mouseSensitivity;
            targetLook.y = Mathf.Clamp(targetLook.y, -90f, 90f);
        }

        // Bagian ini HARUS tetap jalan meski canLook false 
        // agar rotasi player tetap terkunci ke nilai currentLook terakhir
        currentLook.x = Mathf.Lerp(currentLook.x, targetLook.x, Time.deltaTime * lerpSpeed);
        currentLook.y = Mathf.Lerp(currentLook.y, targetLook.y, Time.deltaTime * lerpSpeed);

        transform.rotation = Quaternion.Euler(0f, currentLook.x, 0f);
        playerCam.localRotation = Quaternion.Euler(currentLook.y, 0f, 0f);
    }

    public void ResetTargetLook()
    {
        targetLook = currentLook;
        // Paksa rotasi saat itu juga agar tidak ada sisa sisa Lerp
        transform.rotation = Quaternion.Euler(0f, currentLook.x, 0f);
        playerCam.localRotation = Quaternion.Euler(currentLook.y, 0f, 0f);
    }
}
