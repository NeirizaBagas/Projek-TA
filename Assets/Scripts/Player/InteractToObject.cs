using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

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
    private PlayerMovement playerMovement;

    [SerializeField] private GameObject uiContainer;
    [SerializeField] private TextMeshProUGUI textInteract;

    public static event Action OnInteractionStarted;
    public static event Action OnJournalTriggered;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        uiContainer.SetActive(false);
        canInteract = false;
        isInteracting = false;
        inputActions = new PlayerInputActions();
        
        inputActions.UI.Disable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        freeLook = GetComponent<FreeLook>();
        camPos = Camera.main.transform;
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
        inputActions.Player.Journal.performed += triggerJournal;
        UIManager.OnStopInteract += StopInteractObject;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Interact.performed -= OnTapInteract;
        inputActions.Player.HoldInteract.started -= OnHoldInteract;
        inputActions.Player.HoldInteract.performed -= OnHoldInteract;
        inputActions.Player.HoldInteract.canceled -= OnHoldInteract;
        inputActions.Player.Journal.performed -= triggerJournal;
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
            StartCoroutine(WaitForInteract());
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
                playerMovement.isAllowToMove = false;
                freeLook.ResetTargetLook(); // Reset rotasi kamera saat mulai interaksi
                freeLook.canLook = false; // Matikan kamera
                OnInteractionStarted?.Invoke();
                holdObj.OnHoldStart();
            }
            else if (context.canceled)
            {
                isInteracting = false; // Buka kunci agar bisa jalan/interaksi lagi
                playerMovement.isAllowToMove = true;
                freeLook.canLook = true; // Aktifkan kamera lagi
                holdObj.OnHoldCancel();
            }
        }
    }

    public void triggerJournal(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isInteracting = true; // Kunci status interaksi
            freeLook.canLook = false; // Matikan kamera
            playerMovement.isAllowToMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log(Cursor.lockState);
            OnJournalTriggered?.Invoke();
        }
    }

    private void StopInteractObject()
    {
        currentTarget = null;
        playerMovement.isAllowToMove = true;
        freeLook.canLook = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isInteracting = false;
    }

    IEnumerator WaitForInteract()
    {
        playerMovement.isAllowToMove = false;
        yield return new WaitForSeconds(1f);
        playerMovement.isAllowToMove = true;
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
