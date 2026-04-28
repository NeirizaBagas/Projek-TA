using System;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public int animalIndexPhoto;

    [Header("Item Reference")]
    [SerializeField] private Light senter;

    [Header("Bool Reference")]
    [SerializeField] private bool isSenterActive = false;
    [SerializeField] private bool isReadyToDefuse = false;

    [Header("Bool Item Access")]
    [SerializeField] private bool isHaveJournal;
    [SerializeField] private bool isHaveDefuseKit;
    [SerializeField] private bool isHaveCamera;
    [SerializeField] private bool isHaveSenter;

    [Header("Event Reference")]
    public static Action<bool> OnTriggerJournal;
    public static event Action<int> OnAnimalPhotoRequested;
    public static event Action<bool> OnPhotoModeStarted;
    public static event Action<bool> OnPhotoUiTriggered;
    public static event Action<bool> OnReadyToDefuse;
    //public static event Action<bool> OnSenterToggled;

    private void Start()
    {
        senter.enabled = isSenterActive;
    }

    private void OnEnable()
    {
        EquippedItem.OnItemPickedUp += GrantItemAccess;
    }

    private void OnDisable()
    {
        EquippedItem.OnItemPickedUp -= GrantItemAccess;
    }

    private void GrantItemAccess(ItemType takenItem)
    {
        // Cek tipe item yang masuk, lalu set bool yang sesuai jadi true
        switch (takenItem)
        {
            case ItemType.Journal:
                isHaveJournal = true;
                Debug.Log("Journal Acquired!");
                break;
            case ItemType.Camera:
                isHaveCamera = true;
                Debug.Log("Camera Acquired!");
                break;
            case ItemType.FlashLight:
                isHaveSenter = true;
                Debug.Log("Senter Acquired!");
                break;
            case ItemType.DefuseKit:
                isHaveDefuseKit = true;
                Debug.Log("Defuse Kit Acquired!");
                break;
        }
    }

    public void OpenJournal()
    {
        if (!isHaveJournal) return;
        OnTriggerJournal?.Invoke(true);
    }

    public void OpenDefuseKit()
    {
        if (!isHaveDefuseKit) return;
        isReadyToDefuse = !isReadyToDefuse;
        OnReadyToDefuse?.Invoke(isReadyToDefuse);
    }

    public void CameraMode()
    {
        if (!isHaveCamera) return;
        OnAnimalPhotoRequested?.Invoke(animalIndexPhoto);
        OnPhotoModeStarted?.Invoke(true);
        OnPhotoUiTriggered?.Invoke(true);
    }

    public void ToggleSenter()
    {
        if (!isHaveSenter) return;
        isSenterActive = !isSenterActive;
        senter.enabled = isSenterActive;
    }
}
