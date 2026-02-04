using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractToObject : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private bool canInteract = false;
    private bool isInteracting = false;
    private PlayerInput playerInput;
    private PlayerInputActions inputActions;
    private Transform camPos;
    private GameObject interactAbleObject;
    private FreeLook freeLook;

    [SerializeField] private GameObject uiContainer;
    [SerializeField] private TextMeshProUGUI textInteract;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        uiContainer.SetActive(false);
        inputActions = new PlayerInputActions();
        playerInput = GetComponent<PlayerInput>();
        freeLook = GetComponent<FreeLook>();
        camPos = Camera.main.transform;
        inputActions.UI.Disable();
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        //inputActions.Player.Interact.Enable();
        //inputActions.Player.HoldInteract.Enable();
        inputActions.Player.Interact.performed += InteracttoObject;
        inputActions.Player.HoldInteract.performed += InteracttoObject;
        UIManager.OnCloseButtonPressed += StopInteractObject;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Interact.performed -= InteracttoObject;
        inputActions.Player.HoldInteract.performed -= InteracttoObject;
        UIManager.OnCloseButtonPressed -= StopInteractObject;
    }

    private void Update()
    {
        InteractAble();
    }


    private void InteractAble()
    {
        Ray r = new Ray(camPos.position, camPos.forward);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, interactDistance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Interactable") && !isInteracting)
            {
                interactAbleObject = hit.collider.gameObject;
                canInteract = true;
                uiContainer.SetActive(true);
                textInteract.text = "Press 'E' to interact with " + hit.collider.name;
                
            }
        }
        else
        {
            canInteract = false;
            uiContainer.SetActive(false);
        }
    }

    public void InteracttoObject(InputAction.CallbackContext context)
    {
        Friends friend = interactAbleObject.GetComponent<Friends>();

        //if (canInteract)
        //{
        //    isInteracting = true;
        //    Debug.Log("Interacted with object!");
        //    uiContainer.SetActive(false);
        //    // Sistem interaksi disini
        //    freeLook.canLook = false;
        //    Cursor.lockState = CursorLockMode.Confined;
        //    Cursor.visible = true;
        //    interactAbleObject.GetComponent<IInteractableObject>().Interact();
        //}

        // Hanya eksekusi jika statusnya 'performed' (aksi selesai dilakukan)
        if (!context.performed) return;

        if (canInteract && !isInteracting)
        {
            // DEBUG: Cek action mana yang masuk
            Debug.Log($"Action dipicu oleh: {context.action.name}");

            // Logika pembeda
            if (context.action.name == "HoldInteract")
            {
                Debug.Log("Memicu interaksi TAHAN (Hold)");
                // Jalankan fungsi khusus hold di sini jika perlu
            }
            else
            {
                Debug.Log("Memicu interaksi KLIK (Press)");
            }

            // Jalankan logika umum interaksi
            ExecuteInteraction();
        }
    }

    //public void HoldInteractObject(InputAction.CallbackContext context)
    //{
    //    if (canInteract)
    //    {
    //        isInteracting = true;
    //        Debug.Log("Interacted with object!");
    //        uiContainer.SetActive(false);
    //        // Sistem interaksi disini
    //        freeLook.canLook = false;
    //        Cursor.lockState = CursorLockMode.Confined;
    //        Cursor.visible = true;
    //        interactAbleObject.GetComponent<IInteractableObject>().Interact();
    //    }
    //}

    private void ExecuteInteraction()
    {
        isInteracting = true;
        uiContainer.SetActive(false);

        freeLook.canLook = false;
        Cursor.lockState = CursorLockMode.None; // None lebih baik untuk UI daripada Confined
        Cursor.visible = true;

        IInteractableObject interactable = interactAbleObject.GetComponent<IInteractableObject>();
        if (interactable != null)
        {
            interactable.Interact();
        }
    }

    private void StopInteractObject()
    {
        freeLook.canLook = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isInteracting = false;
    }

    private void OnDrawGizmos()
    {
        if (camPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(camPos.position, camPos.forward * interactDistance);
        }
    }
}
