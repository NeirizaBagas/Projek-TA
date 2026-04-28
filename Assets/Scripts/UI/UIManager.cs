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
    private bool isPhotoModeOpen;
    private GameObject currentActiveUI;

    public static event Action OnStopInteract;
    public static event Action OnTriggerUpdateJournal;
    public static event Action<bool> OnTogglePhotoUI;
    public static event Action<bool> OnNullCurrentUI;

    private void Start()
    {
        CloseAllUI();
    }

    private void OnEnable()
    {
        InteractToObject.OnInteractionStarted += OpenUiInteract;
        InteractToObject.OnUIHoverOn += OpenUiHovering;
        InteractToObject.OnUIHoverOff += CloseAllUI;
        InteractToObject.OnItemMenuToggle += ToggleItemMenu;
        TrapInteract.OnOpenTrapUI += OpenUiInteract;
        TrapInteract.OnCloseTrapUI += CloseUiInteract;
        TrapInteract.OnTrapDefused += CloseUiInteract;
        TrapInteract.OnTrapDefuseFailed += CloseUiInteract;
        JournalManager.OnJournalPageClosed += CloseUiInteract;
        ItemManager.OnPhotoUiTriggered += TogglePhotoUI;
        SnapshotSystem.OnPhotoModeReadyToCapture += TogglePhotoUI;
        SnapshotSystem.OnPhotoModeReadyToCapture += TogglePlayerIndicatorUI;
        SnapshotSystem.OnPhotoReadyToView += ToggleReviewPhotoUI;
        //SnapshotSystem.OnAnimalPhotoUpdated += ToggleJournalUI;
        ItemManager.OnTriggerJournal += ToggleJournalUI;
        EquippedItem.OnUIInteractHoverOFF += CloseUiInteract;
        BedSystemInteract.OnCloseSleepUI += CloseUiInteract;
    }

    private void OnDisable()
    {
        InteractToObject.OnInteractionStarted -= OpenUiInteract;
        InteractToObject.OnUIHoverOn -= OpenUiHovering;
        InteractToObject.OnUIHoverOff -= CloseAllUI;
        InteractToObject.OnItemMenuToggle -= ToggleItemMenu;
        TrapInteract.OnOpenTrapUI -= OpenUiInteract;
        TrapInteract.OnCloseTrapUI -= CloseUiInteract;
        TrapInteract.OnTrapDefused -= CloseUiInteract;
        TrapInteract.OnTrapDefuseFailed -= CloseUiInteract;
        JournalManager.OnJournalPageClosed -= CloseUiInteract;
        ItemManager.OnPhotoUiTriggered -= TogglePhotoUI;
        SnapshotSystem.OnPhotoModeReadyToCapture -= TogglePhotoUI;
        SnapshotSystem.OnPhotoModeReadyToCapture -= TogglePlayerIndicatorUI;
        SnapshotSystem.OnPhotoReadyToView -= ToggleReviewPhotoUI;
        //SnapshotSystem.OnAnimalPhotoUpdated -= ToggleJournalUI;
        ItemManager.OnTriggerJournal -= ToggleJournalUI;
        EquippedItem.OnUIInteractHoverOFF -= CloseUiInteract;
        BedSystemInteract.OnCloseSleepUI -= CloseUiInteract;
    }

    public void OpenUi(GameObject uiToOpen)
    {
        if (currentActiveUI == uiToOpen && uiToOpen.activeSelf) return;

        if (currentActiveUI != null) currentActiveUI.SetActive(false);

        if (uiToOpen != null)
        {
            uiToOpen.SetActive(true);
            if (uiToOpen != uiHoveringContainer) OnNullCurrentUI?.Invoke(false);
            currentActiveUI = uiToOpen;
            Debug.Log("Opening UI: " + uiToOpen.name);
        }
    }

    public void CloseCurrentUI()
    {
        isPhotoModeOpen = false;
        isJournalOpen = false;
        if (currentActiveUI != null)
        {
            OnNullCurrentUI?.Invoke(true);
            currentActiveUI.SetActive(false);
            currentActiveUI = null;
            Debug.Log("Closing Current UI");
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
                ui.SetActive(false);
            }
        }
        currentActiveUI = null;
    }

    public void OpenUiInteract() => OpenUi(interactContainer);

    public void OpenUiHovering() => OpenUi(uiHoveringContainer);

    public void CloseUiInteract()
    {
        CloseCurrentUI();
        OnStopInteract?.Invoke();
        TogglePlayerIndicatorUI(true);
    }

    public void ToggleJournalUI(bool toggleJournal)
    {
        if (toggleJournal && !isJournalOpen)
        {
            CloseCurrentUI();
            OpenUi(journalContainer);
            isJournalOpen= true;
        }
        else
        {
            CloseCurrentUI();
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
        }
        isPhotoModeOpen = isVisible;
    }

    public void ToggleReviewPhotoUI(bool isVisible)
    {
        if (isVisible) OpenUi(photoReviewUI);
        else CloseCurrentUI();
    }

    public void TogglePlayerIndicatorUI(bool isVisible)
    {
        playerIndicator.SetActive(isVisible);
    }

    public void ToggleItemMenu(bool isVisible)
    {
        if (isVisible && currentActiveUI == null)
        {
            OpenUi(itemMenuUI);
            radialMenu.Open();
        }
        else
        {
            radialMenu.Close();
            if (currentActiveUI == itemMenuUI)
            {
                CloseCurrentUI();
            }
        }
    }
}
