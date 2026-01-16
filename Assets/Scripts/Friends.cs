using UnityEngine;

public class Friends : MonoBehaviour, IInteractableObject
{
    [SerializeField] private GameObject canvasPlayer;

    public void Interact()
    {
        canvasPlayer.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Debug.Log("Interacting with Friends object.");
    }

    public void StopInteract()
    {
        canvasPlayer.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
