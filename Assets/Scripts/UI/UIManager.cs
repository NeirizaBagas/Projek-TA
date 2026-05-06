using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] uiElements;

    [SerializeField] private GameObject uiHoveringContainer;
    [SerializeField] private GameObject interactContainer;
    [SerializeField] private GameObject journalContainer;
    [SerializeField] private GameObject photoModeContainer;
    [SerializeField] private GameObject playerIndicator;
    [SerializeField] private GameObject photoReviewUI;
    [SerializeField] private GameObject itemMenuUI;
    [SerializeField] private RadialMenu radialMenu;

    public bool isJournalOpen;
    public static bool isPhotoModeOpen { get; private set; }
    private GameObject currentActiveUI;

    public static event Action OnStopInteract;
    public static event Action OnTriggerUpdateJournal;
    public static event Action<bool> OnTogglePhotoUI;
    public static event Action<bool> OnNullCurrentUI;

    private void Start()
    {
        CloseAllUI();

        TogglePlayerIndicatorUI(true);
    }

    private void OnEnable()
    {
        InteractToObject.OnInteractionStarted += ToggleUIInteract;
        InteractToObject.OnUIHoverToggle += ToggleUIHovering;
        InteractToObject.OnItemMenuToggle += ToggleItemMenu;
        TrapInteract.OnToggleUIInteract += ToggleUIInteract;
        JournalManager.OnJournalPageOpenClose += ToggleJournalUI;
        ItemManager.OnPhotoUiTriggered += TogglePhotoUI;
        SnapshotSystem.OnPhotoModeReadyToCapture += TogglePhotoUI;
        SnapshotSystem.OnPhotoReadyToView += ToggleReviewPhotoUI;
        ItemManager.OnTriggerJournal += ToggleJournalUI;
        EquippedItem.ToggleUIItemEquippable += ToggleUIInteract;
        BedSystemInteract.ToggleSleepUI += ToggleUIInteract;
    }

    private void OnDisable()
    {
        InteractToObject.OnInteractionStarted -= ToggleUIInteract;
        InteractToObject.OnUIHoverToggle -= ToggleUIHovering;
        InteractToObject.OnItemMenuToggle -= ToggleItemMenu;
        TrapInteract.OnToggleUIInteract -= ToggleUIInteract;
        JournalManager.OnJournalPageOpenClose -= ToggleJournalUI;
        ItemManager.OnPhotoUiTriggered -= TogglePhotoUI;
        SnapshotSystem.OnPhotoModeReadyToCapture -= TogglePhotoUI;
        SnapshotSystem.OnPhotoReadyToView -= ToggleReviewPhotoUI;
        ItemManager.OnTriggerJournal -= ToggleJournalUI;
        EquippedItem.ToggleUIItemEquippable -= ToggleUIInteract;
        BedSystemInteract.ToggleSleepUI -= ToggleUIInteract;
    }

    public void OpenUi(GameObject uiToOpen)
    {
        if (currentActiveUI == uiToOpen && uiToOpen.activeSelf) return;

        if (currentActiveUI != null) currentActiveUI.SetActive(false);

        if (uiToOpen != null)
        {
            uiToOpen.SetActive(true);
            if (uiToOpen != uiHoveringContainer && uiToOpen != photoModeContainer) OnNullCurrentUI?.Invoke(false);
            currentActiveUI = uiToOpen;
            //Debug.Log("Opening UI: " + uiToOpen.name);
        }
    }

    public void CloseCurrentUI()
    {
        isPhotoModeOpen = false;
        isJournalOpen = false;
        if (currentActiveUI != null)
        {
            if (currentActiveUI == photoModeContainer) OnTogglePhotoUI?.Invoke(false);
            currentActiveUI.SetActive(false);
            currentActiveUI = null;
            OnNullCurrentUI?.Invoke(true);
            //Debug.Log("Closing Current UI");
        }
    }

    private void CloseAllUI()
    {
        isJournalOpen = false;  
        isPhotoModeOpen = false;
        foreach (GameObject ui in uiElements)
        {
            if (ui != null && ui.activeSelf)
            {
                if (ui != playerIndicator)
                {
                    ui.SetActive(false);
                }
            }
        }
        currentActiveUI = null;
    }

    public void ToggleUIHovering(bool isHovering)
    {
        if (isHovering) OpenUi(uiHoveringContainer);
        else CloseCurrentUI();
    }

    public void ToggleUIInteract(bool isInteracting)
    {
        if (isInteracting) OpenUi(interactContainer);
        else
        {
            CloseCurrentUI();
            OnStopInteract?.Invoke();
        }
    }

    public void ToggleJournalUI(bool toggleJournal)
    {
        if (toggleJournal && !isJournalOpen)
        {
            CloseCurrentUI();
            OpenUi(journalContainer);
            isJournalOpen= true;
            TogglePlayerIndicatorUI(false);
        }
        else
        {
            CloseCurrentUI();
            OnStopInteract?.Invoke();
            TogglePlayerIndicatorUI(true);
        }
    }

    public void TogglePhotoUI(bool isVisible)
    {
        if (isVisible && !isPhotoModeOpen)
        {
            TogglePlayerIndicatorUI(false);
            OpenUi(photoModeContainer);
            OnTogglePhotoUI?.Invoke(true);
        }
        else
        {
            CloseCurrentUI();
            OnTogglePhotoUI?.Invoke(false);
            OnTriggerUpdateJournal?.Invoke();
            if (!SnapshotSystem.isCapturingPhoto)
            {
                TogglePlayerIndicatorUI(true);
            }
        }
        isPhotoModeOpen = isVisible;
        Debug.Log("Photo Mode " + (isVisible ? "Opened" : "Closed"));
    }

    public void ToggleReviewPhotoUI(bool isVisible)
    {
        if (isVisible) OpenUi(photoReviewUI);
        else CloseCurrentUI();
    }

    public void TogglePlayerIndicatorUI(bool isVisible)
    {
        if (playerIndicator != null)
        {
            playerIndicator.SetActive(isVisible);
        }
        Debug.Log("Player Indicator " + (isVisible ? "Shown" : "Hidden"));
    }

    public void ToggleItemMenu(bool isVisible)
    {
        if (isVisible == true && currentActiveUI == null)
        {
            TogglePlayerIndicatorUI(false);
            OpenUi(itemMenuUI);
            radialMenu.Open();
        }
        else if (isVisible == true && currentActiveUI != null)
        {
            if (currentActiveUI == itemMenuUI)
            {
                CloseCurrentUI();
                radialMenu.Close();
                TogglePlayerIndicatorUI(true); 
                Debug.Log("Closing Item Menu because it's already open");
            }
             else
            {
                TogglePlayerIndicatorUI(false);
                OpenUi(itemMenuUI);
                radialMenu.Open();
                Debug.Log("Opening Item Menu and closing current UI: " + currentActiveUI.name);
            }
        }
        else
        {
            radialMenu.Close();
            
            if (currentActiveUI == itemMenuUI)
            {
                CloseCurrentUI();
                TogglePlayerIndicatorUI(true);
            }
        }
    }
}
