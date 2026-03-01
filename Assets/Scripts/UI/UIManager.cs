using Mono.Cecil;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private GameObject interactContainer;
    [SerializeField] private GameObject journalContainer;
    public bool isJournalOpen;

    public static event Action OnStopInteract;

    private void Awake()
    {
        uiContainer.SetActive(false);
        interactContainer.SetActive(false);
    }

    private void OnEnable()
    {
        InteractToObject.OnInteractionStarted += OpenUiInteract;
        InteractToObject.OnJournalTriggered += TriggerJournal;
        TrapInteract.OnTrapDefused += CloseUiInteract;
        JournalManager.OnJournalPageClosed += CloseUiInteract;
    }

    private void OnDisable()
    {
        InteractToObject.OnInteractionStarted -= OpenUiInteract;
        TrapInteract.OnTrapDefused -= CloseUiInteract;
        JournalManager.OnJournalPageClosed -= CloseUiInteract;

    }

    public void OpenUiInteract()
    {
        uiContainer.SetActive(false);
        interactContainer.SetActive(true);
    }

    public void CloseUiInteract()
    {
        isJournalOpen = false;
        interactContainer.SetActive(false);
        uiContainer.SetActive(true);
        journalContainer.SetActive(false);
        OnStopInteract?.Invoke();
    }

    public void TriggerJournal()
    {
        if (!isJournalOpen)
        {
            journalContainer.SetActive(true);
            isJournalOpen = true;
        }
    }
}
