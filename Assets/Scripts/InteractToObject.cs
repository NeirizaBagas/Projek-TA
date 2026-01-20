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
        inputActions.Player.Interact.performed += InteracttoObject;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.Enable();
        UIManager.OnCloseButtonPressed += StopInteractObject;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
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
        if (canInteract)
        {
            isInteracting = true;
            Debug.Log("Interacted with object!");
            uiContainer.SetActive(false);
            // Sistem interaksi disini
            freeLook.canLook = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            interactAbleObject.GetComponent<IInteractableObject>().Interact();
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
