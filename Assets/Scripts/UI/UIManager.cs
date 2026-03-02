using Mono.Cecil;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] uiElements;

    [SerializeField] private GameObject uiContainer;
    [SerializeField] private GameObject interactContainer;
    [SerializeField] private GameObject journalContainer;
    public bool isJournalOpen;

    public static event Action OnStopInteract;

    private void Start()
    {
        CloseAllUI();
    }

    private void OnEnable()
    {
        InteractToObject.OnInteractionStarted += OpenUiInteract;
        InteractToObject.OnJournalTriggered += TriggerJournal;
        TrapInteract.OnTrapDefused += CloseUiInteract;
        TrapInteract.OnTrapDefuseFailed += CloseUiInteract;
        JournalManager.OnJournalPageClosed += CloseUiInteract;
    }

    private void OnDisable()
    {
        InteractToObject.OnInteractionStarted -= OpenUiInteract;
        TrapInteract.OnTrapDefused -= CloseUiInteract;
        JournalManager.OnJournalPageClosed -= CloseUiInteract;

    }

    private void CloseAllUI()
    {
        foreach (GameObject ui in uiElements)
        {
            ui.SetActive(false);
        }
    }

    public void OpenUiInteract()
    {
        CloseAllUI();
        interactContainer.SetActive(true);
    }

    public void CloseUiInteract()
    {
        isJournalOpen = false;
        CloseAllUI();
        uiContainer.SetActive(true);
        OnStopInteract?.Invoke();
    }

    public void TriggerJournal()
    {
        journalContainer.SetActive(true);
        isJournalOpen = true;
    }

    
}
