using System;
using UnityEngine;

public class Friends : MonoBehaviour, IInteractableObject
{
    public static event Action OnInteractionStarted;

    private void OnEnable()
    {
        UIManager.OnCloseButtonPressed += StopInteract;
    }

    private void OnDisable()
    {
        UIManager.OnCloseButtonPressed -= StopInteract;
    }

    public void Interact()
    {
        
        OnInteractionStarted?.Invoke();
    }

    public void StopInteract()
    {
        Debug.Log("Stopped interacting with Friends object.");
    }
}
