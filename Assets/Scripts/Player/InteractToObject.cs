using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InteractToObject : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f; // Jarak maksimal untuk interaksi
    [SerializeField] private bool canInteract; // Status apakah bisa berinteraksi
    private bool isInteracting = false; // Status apakah sedang berinteraksi
    private PlayerInput playerInput;
    private PlayerInputActions inputActions;
    private Transform camPos;
    private IInteractableObject currentTarget; // Data target interaksi saat ini
    private FreeLook freeLook;

    [SerializeField] private GameObject uiContainer;
    [SerializeField] private TextMeshProUGUI textInteract;

    public static event Action OnInteractionStarted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        uiContainer.SetActive(false);
        canInteract = false;
        isInteracting = false;
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
        inputActions.Player.Interact.performed += OnTapInteract;
        inputActions.Player.HoldInteract.started += OnHoldInteract;
        inputActions.Player.HoldInteract.performed += OnHoldInteract;
        inputActions.Player.HoldInteract.canceled += OnHoldInteract;
        UIManager.OnStopInteract += StopInteractObject;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Interact.performed -= OnTapInteract;
        inputActions.Player.HoldInteract.performed -= OnHoldInteract;
        UIManager.OnStopInteract -= StopInteractObject;
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
                IInteractableObject interactable = hit.collider.GetComponent<IInteractableObject>();

                if (interactable != null && !isInteracting)
                {
                    if (currentTarget != interactable)
                    {
                        if (currentTarget != null) currentTarget.OnHoverExit();
                        currentTarget = interactable;
                        currentTarget.OnHoverEnter();
                    }

                    uiContainer.SetActive(true);

                    if (currentTarget is ITapInteractable)
                    {
                        textInteract.text = "Press [E] to Interact";
                        canInteract = true;
                    }
                    else if (currentTarget is IHoldInteractable)
                    {
                        textInteract.text = "Hold [E] to Interact";
                        canInteract = true;
                    }
                }
            }
        }
        else
        {
            canInteract = false;
            if (currentTarget != null)
            {
                currentTarget.OnHoverExit();
                currentTarget = null;
            }
            uiContainer.SetActive(false);
        }
    }

    public void OnTapInteract(InputAction.CallbackContext context)
    {

        if (currentTarget is ITapInteractable tapObj && canInteract)
        {
            Debug.Log("Tap Interaction Triggered");
            freeLook.canLook = false;
            isInteracting = true;
            OnInteractionStarted?.Invoke();
            tapObj.OnTap();

        }
    }

    public void OnHoldInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Hold Interaction Detected");
        if (currentTarget is IHoldInteractable holdObj && canInteract)
        {
            Debug.Log("Hold Interaction Triggered" + context);
            if (context.started)
            {
                isInteracting = true; // Kunci status interaksi
                freeLook.canLook = false; // Matikan kamera
                OnInteractionStarted?.Invoke();
                holdObj.OnHoldStart();
                Debug.Log("Hold Started");
            }
            else if (context.canceled)
            {
                isInteracting = false; // Buka kunci agar bisa jalan/interaksi lagi
                freeLook.canLook = true; // Aktifkan kamera lagi
                holdObj.OnHoldCancel();
                Debug.Log("Hold Canceled");
            }
        }
    }

    private void StopInteractObject()
    {
        currentTarget = null;
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
