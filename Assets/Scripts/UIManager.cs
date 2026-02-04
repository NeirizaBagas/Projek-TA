using Mono.Cecil;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private GameObject interactContainer;
    public static event Action OnCloseButtonPressed;
    private TypingManager typingManager;

    private void Awake()
    {
        uiContainer.SetActive(false);
        interactContainer.SetActive(false);
        typingManager = interactContainer.GetComponentInChildren<TypingManager>();
        typingManager.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Friends.OnInteractionStarted += OpenUiInteract;
    }

    private void OnDisable()
    {
        Friends.OnInteractionStarted -= OpenUiInteract;
    }


    public void OpenUiInteract()
    {
        uiContainer.SetActive(false);
        interactContainer.SetActive(true);
        //typingManager.gameObject.SetActive(true);
    }

    public void CloseUiInteract()
    {
        interactContainer.SetActive(false);
        uiContainer.SetActive(true);
        //typingManager.gameObject.SetActive(false);
        OnCloseButtonPressed?.Invoke();
        Debug.Log("Tes");
    }
}
