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
    [SerializeField] private TextMeshProUGUI textInteract;
    private IInteractableObject currentTarget; // Data target interaksi saat ini
    private bool isInteracting = false; // Status apakah sedang berinteraksi

    [Header("Snapshot System")]
    private SnapshotSystem _snapshotSystem;
    private bool _canTakePhoto = false;
    private bool isUIHoveringActive = false;

    [Header("Radial Item Ui")]
    //[SerializeField] private RadialMenu radialMenu;
    private bool isItemMenuActive = false;

    public static event Action OnInteractionStarted;
    public static event Action OnUIHoverOn;
    public static event Action OnUIHoverOff;
    public static event Action OnJournalTriggered;
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
        JournalCamButton.OnPhotoModeStarted += ToggleTakePhotoAccess;
        UIManager.OnStopInteract += StopInteractObject;
        UIManager.OnTogglePhotoUI += ToggleTakePhotoAccess;
        UIManager.OnNullCurrentUI += TogglePlayerAccess;
        SnapshotSystem.OnPhotoReadyToView += ReviewPhoto;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Interact.performed -= OnTapInteract;
        inputActions.Player.HoldInteract.started -= OnHoldInteract;
        inputActions.Player.HoldInteract.performed -= OnHoldInteract;
        inputActions.Player.HoldInteract.canceled -= OnHoldInteract;
        //inputActions.Player.Journal.performed -= triggerJournal;
        inputActions.Player.TakePhoto.performed -= TakePicture;
        inputActions.Player.OpenItemMenu.started -= ToggleItemMenu;
        JournalCamButton.OnPhotoModeStarted -= ToggleTakePhotoAccess;
        UIManager.OnStopInteract -= StopInteractObject;
        UIManager.OnTogglePhotoUI -= ToggleTakePhotoAccess;
        UIManager.OnNullCurrentUI -= TogglePlayerAccess;

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
                    if (currentTarget != interactable && !isUIHoveringActive)
                    {
                        if (currentTarget != null) currentTarget.OnHoverExit();
                        currentTarget = interactable;
                        currentTarget.OnHoverEnter();

                        OnUIHoverOn?.Invoke();

                        if (currentTarget is ITapInteractable)
                        {
                            textInteract.text = "Press [E] to Interact";
                        }
                        else if (currentTarget is IHoldInteractable)
                        {
                            textInteract.text = "Hold [E] to Interact";
                        }
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
            if (isUIHoveringActive)
            {
                OnUIHoverOff?.Invoke();
                isUIHoveringActive = false;
            }
        }
    }

    public void OnTapInteract(InputAction.CallbackContext context)
    {

        if (currentTarget is ITapInteractable tapObj && canInteract)
        {
            Debug.Log("Tap Interaction Triggered");
            StartCoroutine(WaitForInteract());
            TogglePlayerAccess(false);
            //freeLook.canLook = false;
            //isInteracting = true;
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
                //playerMovement.isAllowToMove = false;
                //freeLook.ResetTargetLook(); // Reset rotasi kamera saat mulai interaksi
                //freeLook.canLook = false; // Matikan kamera
                OnInteractionStarted?.Invoke();
                holdObj.OnHoldStart();
            }
            else if (context.canceled)
            {
                isInteracting = false; // Buka kunci agar bisa jalan/interaksi lagi
                TogglePlayerAccess(true);
                //playerMovement.isAllowToMove = true;
                //freeLook.canLook = true; // Aktifkan kamera lagi
                holdObj.OnHoldCancel();
            }
        }
    }

    //public void triggerJournal(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        isInteracting = true; // Kunci status interaksi
    //        TogglePlayerAccess(false);
    //        //freeLook.canLook = false; // Matikan kamera
    //        //playerMovement.isAllowToMove = false;
    //        //Cursor.lockState = CursorLockMode.None;
    //        //Cursor.visible = true;
    //        //Debug.Log(Cursor.lockState);
    //        OnJournalTriggered?.Invoke();
    //    }
    //}

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

            //if (isItemMenuActive)
            //{
            //    isItemMenuActive = false;
            //    TogglePlayerAccess(true);
            //    OnItemMenuToggle?.Invoke(false);
            //}
            //else
            //{
            //    isItemMenuActive = true;
            //    TogglePlayerAccess(false);
            //    OnItemMenuToggle?.Invoke(true);
            //}

        }

        //if (context.started)
        //{
        //    radialMenu.Open();
        //    TogglePlayerAccess(false);
        //    OnItemMenuToggle?.Invoke(true);
        //}
        //else if (context.canceled)
        //{
        //    radialMenu.Close();
        //    TogglePlayerAccess(true);
        //    OnItemMenuToggle?.Invoke(false);
        //}
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
