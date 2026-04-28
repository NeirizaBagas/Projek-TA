using System;
using TMPro;
using UnityEngine;

public enum ItemType { Journal, Camera, DefuseKit, FlashLight}

public class EquippedItem : MonoBehaviour, ITapInteractable
{
    [SerializeField] private TextMeshProUGUI textItemInteract;
    [SerializeField] private ItemType itemType;

    public static event Action OnUIInteractHoverOFF;
    public static event Action<ItemType> OnItemPickedUp;

    public void OnHoverEnter()
    {
        textItemInteract.text = "Press [F] to Take " + itemType;
    }

    public void OnHoverExit()
    {
        OnUIInteractHoverOFF?.Invoke();
    }

    public void OnTap()
    {
        Debug.Log("Tapped on " + this.name);
        OnItemPickedUp?.Invoke(itemType);
        this.gameObject.SetActive(false);
        OnUIInteractHoverOFF?.Invoke();
        // Setaktif false item ini, dan instruksi enable ke itemmanagement di player
    }
}
