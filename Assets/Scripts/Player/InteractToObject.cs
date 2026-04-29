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
    private IInteractableObject currentTarget; // Data target interaksi saat ini
    private bool isInteracting = false; // Status apakah sedang berinteraksi

    [Header("Snapshot System")]
    private SnapshotSystem _snapshotSystem;
    private bool _canTakePhoto = false;
    private bool isUIHoveringActive = false;

    public static event Action OnInteractionStarted;
    public static event Action OnUIHoverOn;
    public static event Action OnUIHoverOff;
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
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += OnTapInteract;
        inputActions.Player.HoldInteract.started += OnHoldInteract;
        inputActions.Player.HoldInteract.performed += OnHoldInteract;
        inputActions.Player.HoldInteract.canceled += OnHoldInteract;
        inputActions.Player.TakePhoto.performed += TakePicture;
        inputActions.Player.OpenItemMenu.performed += ToggleItemMenu;
        ItemManager.OnPhotoModeStarted += ToggleTakePhotoAccess;
        ItemManager.OnReadyToDefuse += ToggleCanDefuse;
        UIManager.OnStopInteract += StopInteractObject;
        UIManager.OnTogglePhotoUI += ToggleTakePhotoAccess;
        UIManager.OnNullCurrentUI += TogglePlayerAccess;
        SnapshotSystem.OnPhotoReadyToView += ReviewPhoto;
        //TrapInteract.OnCanDefuse += ToggleCanDefuse;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Interact.performed -= OnTapInteract;
        inputActions.Player.HoldInteract.started -= OnHoldInteract;
        inputActions.Player.HoldInteract.performed -= OnHoldInteract;
        inputActions.Player.HoldInteract.canceled -= OnHoldInteract;
        inputActions.Player.TakePhoto.performed -= TakePicture;
        inputActions.Player.OpenItemMenu.started -= ToggleItemMenu;
        ItemManager .OnPhotoModeStarted -= ToggleTakePhotoAccess;
        ItemManager.OnReadyToDefuse -= ToggleCanDefuse;
        UIManager.OnStopInteract -= StopInteractObject;
        UIManager.OnTogglePhotoUI -= ToggleTakePhotoAccess;
        UIManager.OnNullCurrentUI -= TogglePlayerAccess;
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

                        OnUIHoverOn?.Invoke();
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
                        OnUIHoverOn?.Invoke();
                        canInteract = true;
                    }
                }
            }
            else
            {
                ExitHoveringState();
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
            OnUIHoverOff?.Invoke();
            isUIHoveringActive = false;
        }
    }

    public void OnTapInteract(InputAction.CallbackContext context)
    {

        if (currentTarget is ITapInteractable tapObj && canInteract)
        {
            Debug.Log("Tap Interaction Triggered");
            StartCoroutine(WaitForInteract());
            TogglePlayerAccess(false);
            OnInteractionStarted?.Invoke();
            tapObj.OnTap();

        }
    }

    public void OnHoldInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Hold Interaction Detected");
        
        if (currentTarget is IHoldInteractable holdObj && canInteract)
        {
            if (context.started)
            {
                isInteracting = true; // Kunci status interaksi
                TogglePlayerAccess(false);
                OnInteractionStarted?.Invoke();
                holdObj.OnHoldStart();
            }
            else if (context.canceled)
            {
                isInteracting = false; // Buka kunci agar bisa jalan/interaksi lagi
                TogglePlayerAccess(true);
                holdObj.OnHoldCancel();
            }
        }
    }

    private void StopInteractObject()
    {
        TogglePlayerAccess(true);
    }

    IEnumerator WaitForInteract()
    {
        playerMovement.isAllowToMove = false;
        yield return new WaitForSeconds(1f);
        playerMovement.isAllowToMove = true;
    }

    private void ToggleTakePhotoAccess(bool isAllowed)
    {
        _canTakePhoto = isAllowed;
        if (isAllowed && freeLook.canLook == false && Cursor.visible == true)
        {
            TogglePlayerAccess(true);
        }
        else
        {
            Debug.Log("Gk bisa ambil foto");
        }
    }

    public void TakePicture(InputAction.CallbackContext context)
    {
        if (context.performed && _canTakePhoto)
        {
            Debug.Log("Cekrek");
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
            TogglePlayerAccess(false);
        }
        else
        {
            TogglePlayerAccess(true);
        }
    }

    public void TogglePlayerAccess(bool isAllowed) // Kalau allow kondisi berkeliling, kalau false kondisi interaksi/journal/photo review
    {
        playerMovement.isAllowToMove = isAllowed;
        freeLook.canLook = isAllowed;
        if (!isAllowed)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isInteracting = true;
            Debug.Log("Beku");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isInteracting = false;
            currentTarget = null;
        }
    }

    private void ToggleItemMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePlayerAccess(false);
            OnItemMenuToggle?.Invoke(true);

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
