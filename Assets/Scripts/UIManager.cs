using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private GameObject interactContainer;
    public static event Action OnCloseButtonPressed;

    private void OnEnable()
    {
        Friends.OnInteractionStarted += OpenUiInteract;
    }

    private void OnDisable()
    {
        Friends.OnInteractionStarted -= OpenUiInteract;
    }

    private void Awake()
    {
        uiContainer.SetActive(false);
        interactContainer.SetActive(false);
    }

    public void OpenUiInteract()
    {
        uiContainer.SetActive(false);
        interactContainer.SetActive(true);
    }

    public void CloseUiInteract()
    {
        interactContainer.SetActive(false);
        uiContainer.SetActive(true);
        OnCloseButtonPressed?.Invoke();
    }
}
