using System;
using TMPro;
using UnityEngine;

public enum ItemType { Journal, Camera, DefuseKit, FlashLight}

public class EquippedItem : MonoBehaviour, ITapInteractable
{
    [SerializeField] private ItemType itemType;

    public string InteractMessage => "Press [F] to Take " + itemType;

    public static event Action<bool> ToggleUIItemEquippable;
    public static event Action<ItemType> OnItemPickedUp;

    public void OnHoverEnter()
    {

    }

    public void OnHoverExit()
    {
        ToggleUIItemEquippable?.Invoke(false);
    }

    public void OnTap()
    {
        AudioManager.Instance.PlaySFX(3); // Mainkan SFX klik tombol (asumsi index 0 adalah suara klik)
        OnItemPickedUp?.Invoke(itemType);
        this.gameObject.SetActive(false);
        ToggleUIItemEquippable?.Invoke(false);
        // Setaktif false item ini, dan instruksi enable ke itemmanagement di player
    }
}
