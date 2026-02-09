using Mono.Cecil;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private GameObject interactContainer;

    public static event Action OnStopInteract;

    private void Awake()
    {
        uiContainer.SetActive(false);
        interactContainer.SetActive(false);
    }

    private void OnEnable()
    {
        InteractToObject.OnInteractionStarted += OpenUiInteract;
        TrapInteract.OnTrapDefused += CloseUiInteract;
    }

    private void OnDisable()
    {
        InteractToObject.OnInteractionStarted -= OpenUiInteract;
        TrapInteract.OnTrapDefused -= CloseUiInteract;
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
        Debug.Log("Tes");
        OnStopInteract?.Invoke();
    }
}
