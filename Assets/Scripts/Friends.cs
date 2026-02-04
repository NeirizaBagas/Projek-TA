using System;
using UnityEngine;

public enum InteractionType
{
    Press, Hold
}

public enum FriendType
{
    Trap, Friendly
}

public class Friends : MonoBehaviour, IInteractableObject
{
    public InteractionType interactionType;
    public FriendType friendType;
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
        Debug.Log("Interacted with Friends object.");
        OnInteractionStarted?.Invoke();
    }

    public void StopInteract()
    {
        Debug.Log("Stopped interacting with Friends object.");
    }
}
