using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class InteractToObject : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInputActions inputActions;
    private Transform camPos;
    private FreeLook freeLook;
    private PlayerMovement playerMovement;

    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3f; // Jarak maksimal untuk interaksi
    [SerializeField] private bool canInteract; // Status apakah bisa berinteraksi
    [SerializeField] private bool canDefuse;
    [SerializeField] private TextMeshProUGUI textInteract;
    private IInteractableObject currentTarget; // Data target interaksi saat ini
    private bool isInteracting = false; // Status apakah sedang berinteraksi

    [Header("Snapshot System")]
    private SnapshotSystem _snapshotSystem;
    private bool isUIHoveringActive = false;

    [Header("Bool Check")]
    public bool isItemMenuActive;

    public static event Action<bool> OnInteractionStarted;
    public static event Action<bool> OnUIHoverToggle;
    public static event Action<bool> OnItemMenuToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
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
        _snapshotSystem = GetComponent<SnapshotSystem>();
        camPos = Camera.main.transform;
        isItemMenuActive = false;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += OnTapInteract;
        inputActions.Player.HoldInteract.started += OnHoldInteract;
        inputActions.Player.HoldInteract.canceled += OnHoldInteract;
        inputActions.Player.TakePhoto.performed += TakePicture;
        inputActions.Player.OpenItemMenu.performed += ToggleItemMenu;
        ItemManager.OnPhotoModeStarted += ToggleTakePhotoAccess;
        ItemManager.OnReadyToDefuse += ToggleCanDefuse;
        UIManager.OnStopInteract += StopInteractObject;
        UIManager.OnTogglePhotoUI += ToggleTakePhotoAccess;
        UIManager.OnNullCurrentUI += TogglePlayerMovement;
        //UIManager.OnNullCurrentUI += ToggleAnyUIActive;
        SnapshotSystem.OnPhotoReadyToView += ReviewPhoto;
        //TrapInteract.OnCanDefuse += ToggleCanDefuse;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Interact.performed -= OnTapInteract;
        inputActions.Player.HoldInteract.started -= OnHoldInteract;
        inputActions.Player.HoldInteract.canceled -= OnHoldInteract;
        inputActions.Player.TakePhoto.performed -= TakePicture;
        inputActions.Player.OpenItemMenu.started -= ToggleItemMenu;
        ItemManager .OnPhotoModeStarted -= ToggleTakePhotoAccess;
        ItemManager.OnReadyToDefuse -= ToggleCanDefuse;
        UIManager.OnStopInteract -= StopInteractObject;
        UIManager.OnTogglePhotoUI -= ToggleTakePhotoAccess;
        UIManager.OnNullCurrentUI -= TogglePlayerMovement;
        //UIManager.OnAnyUIActive -= ToggleAnyUIActive;
        //TrapInteract.OnCanDefuse -= ToggleCanDefuse;
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

                if (interactable != null && !isInteracting && canDefuse)
                {
                    if (currentTarget != interactable && !isUIHoveringActive)
                    {
                        if (currentTarget != null) currentTarget.OnHoverExit();
                        currentTarget = interactable;
                        currentTarget.OnHoverEnter();
                        if (textInteract != null) textInteract.text = currentTarget.InteractMessage;
                        OnUIHoverToggle?.Invoke(true);
                        canInteract = true;
                    }
                }
            }
            else if (hit.collider != null && hit.collider.CompareTag("Equipable") && !isInteracting)
            {
                IInteractableObject interactableObject = hit.collider.GetComponent<IInteractableObject>();

                if (interactableObject != null && !isInteracting && !isUIHoveringActive)
                {
                    if (currentTarget != interactableObject)
                    {
                        if (currentTarget != null) currentTarget.OnHoverExit();
                        currentTarget = interactableObject;
                        currentTarget.OnHoverEnter();
                        if (textInteract != null) textInteract.text = currentTarget.InteractMessage;
                        OnUIHoverToggle?.Invoke(true);
                        canInteract = true;
                    }
                }
            }
            else
            {
                //ExitHoveringState();
                if (hit.collider != null && !hit.collider.CompareTag("Interactable") && !hit.collider.CompareTag("Equipable"))
                {
                    ExitHoveringState();
                }
            }
        }
        else
        {
            ExitHoveringState();
        }
    }

    public void ExitHoveringState()
    {
        canInteract = false;
        if (currentTarget != null)
        {
            currentTarget.OnHoverExit();    
            currentTarget = null;
        }
        if (isUIHoveringActive)
        {
            OnUIHoverToggle?.Invoke(false);
            isUIHoveringActive = false;
        }
    }

    public void OnTapInteract(InputAction.CallbackContext context)
    {

        if (currentTarget is ITapInteractable tapObj && canInteract)
        {
            StartCoroutine(WaitForInteract());
            //TogglePlayerMovement(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //OnInteractionStarted?.Invoke(true);
            tapObj.OnTap();

        }
    }

    public void OnHoldInteract(InputAction.CallbackContext context)
    {
        
        
        if (currentTarget is IHoldInteractable holdObj && canInteract)
        {
            if (context.started)
            {
                TogglePlayerMovement(false);
                OnInteractionStarted?.Invoke(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                holdObj.OnHoldStart();
            }
            else if (context.canceled)
            {
                TogglePlayerMovement(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                holdObj.OnHoldCancel();
            }
        }
    }

    private void StopInteractObject()
    {
        TogglePlayerMovement(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isItemMenuActive = false;
    }

    IEnumerator WaitForInteract()
    {
        //playerMovement.isAllowToMove = false;
        TogglePlayerMovement(false);
        yield return new WaitForSeconds(1f);
        //playerMovement.isAllowToMove = true;
        TogglePlayerMovement(true);
    }

    private void ToggleTakePhotoAccess(bool isAllowed)
    {
        Debug.Log("Toggle Take Photo Access: " + isAllowed);
        if (isAllowed/* && freeLook.canLook == false && Cursor.visible == true*/)
        {
            TogglePlayerMovement(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Debug.Log("Gk bisa ambil foto");
        }
    }

    public void TakePicture(InputAction.CallbackContext context)
    {
        if (context.performed && UIManager.isPhotoModeOpen == true)
        {
            _snapshotSystem.CaptureSnapshot();
        }
        //else
        //{
        //    _snapshotSystem.ClearSnapshot();
        //}
    }

    public void ReviewPhoto(bool isReviewingPhoto)
    {
        if (isReviewingPhoto)
        {
            TogglePlayerMovement(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            TogglePlayerMovement(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void TogglePlayerMovement(bool isAllowed) // Kalau allow kondisi berkeliling, kalau false kondisi interaksi/journal/photo review
    {
        playerMovement.isAllowToMove = isAllowed;
        freeLook.canLook = isAllowed;
        if (!isAllowed)
        {
            isInteracting = true;
        }
        else
        {
            isInteracting = false;
            currentTarget = null;
        }
    }

    private void ToggleItemMenu(InputAction.CallbackContext context)
    {
        if (context.performed && !isItemMenuActive)
        {
            TogglePlayerMovement(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnItemMenuToggle?.Invoke(true);
            isItemMenuActive = true;
            ItemManager.Instance.ResetExclusiveItemState();
        }
        else
        {
            TogglePlayerMovement(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            OnItemMenuToggle?.Invoke(false);
            isItemMenuActive = false;
        }
    }

    private void ToggleCanDefuse(bool isReadyToDefuse)
    {
        Debug.Log(isReadyToDefuse);
        canDefuse = isReadyToDefuse;
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
