using System;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public enum ExclusiveItem { None, Journal, DefuseKit, Camera }

    [Header("Active Item State")]
    public ExclusiveItem currentExclusiveItem = ExclusiveItem.None;

    public int animalIndexPhoto;

    [Header("Item Reference")]
    [SerializeField] private Light senter;

    [Header("Bool Reference")]
    [SerializeField] private bool isSenterActive = false;
    //[SerializeField] private bool isReadyToDefuse = false;

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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

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
        switch (takenItem)
        {
            case ItemType.Journal: isHaveJournal = true; break;
            case ItemType.Camera: isHaveCamera = true; break;
            case ItemType.FlashLight: isHaveSenter = true; break;
            case ItemType.DefuseKit: isHaveDefuseKit = true; break;
        }
    }

    public bool CheckHasItem(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Journal => isHaveJournal,
            ItemType.Camera => isHaveCamera,
            ItemType.FlashLight => isHaveSenter,
            ItemType.DefuseKit => isHaveDefuseKit,
            _ => false,
        };
    }

    public void ToggleJournal()
    {
        if (!isHaveJournal) return;

        // Cek: Apakah ada item lain yang sedang aktif?
        if (currentExclusiveItem != ExclusiveItem.None && currentExclusiveItem != ExclusiveItem.Journal)
        {
            Debug.Log($"Gagal buka Journal. {currentExclusiveItem} sedang aktif!");
            return;
        }

        // Buka / Tutup Journal
        if (currentExclusiveItem == ExclusiveItem.Journal)
        {
            currentExclusiveItem = ExclusiveItem.None;
            OnTriggerJournal?.Invoke(false);
        }
        else
        {
            currentExclusiveItem = ExclusiveItem.Journal;
            OnTriggerJournal?.Invoke(true);
            //ToggleSetActiveRadialMenu();
        }
    }

    public void ToggleDefuseKit()
    {
        if (!isHaveDefuseKit) return;

        // Cek: Apakah ada item lain yang sedang aktif?
        if (currentExclusiveItem != ExclusiveItem.None && currentExclusiveItem != ExclusiveItem.DefuseKit)
        {
            Debug.Log($"Gagal pakai Defuse Kit. {currentExclusiveItem} sedang aktif!");
            return;
        }

        // Buka / Tutup Defuse Kit
        if (currentExclusiveItem == ExclusiveItem.DefuseKit)
        {
            currentExclusiveItem = ExclusiveItem.None;
            OnReadyToDefuse?.Invoke(false);
        }
        else
        {
            currentExclusiveItem = ExclusiveItem.DefuseKit;
            OnReadyToDefuse?.Invoke(true);
            //ToggleSetActiveRadialMenu();
        }
    }

    public void ToggleCameraMode()
    {
        if (!isHaveCamera) return;

        // Cek: Apakah ada item lain yang sedang aktif?
        if (currentExclusiveItem != ExclusiveItem.None && currentExclusiveItem != ExclusiveItem.Camera)
        {
            Debug.Log($"Gagal pakai Kamera. {currentExclusiveItem} sedang aktif!");
            return;
        }

        // Buka / Tutup Kamera
        if (currentExclusiveItem == ExclusiveItem.Camera)
        {
            currentExclusiveItem = ExclusiveItem.None;
            OnPhotoModeStarted?.Invoke(false);
            OnPhotoUiTriggered?.Invoke(false);
        }
        else
        {
            currentExclusiveItem = ExclusiveItem.Camera;
            OnAnimalPhotoRequested?.Invoke(animalIndexPhoto);
            OnPhotoModeStarted?.Invoke(true);
            OnPhotoUiTriggered?.Invoke(true);
            //ToggleSetActiveRadialMenu();
        }
    }

    // --- SENTER (BEBAS NYALA/MATI KAPAN SAJA) ---

    public void ToggleSenter()
    {
        if (!isHaveSenter) return;
        isSenterActive = !isSenterActive;
        senter.enabled = isSenterActive;
    }

    // --- PENGAMAN ---
    // Dipanggil dari UIManager jika menu tertutup paksa (misal tombol ESC)
    public void ResetExclusiveItemState()
    {
        currentExclusiveItem = ExclusiveItem.None;
    }

    //public void ToggleSetActiveRadialMenu()
    //{
    //    if (InteractToObject.isItemMenuActive == true)
    //    {
    //        InteractToObject.isItemMenuActive = false;
    //    }

    //    Debug.Log("Toggling Radial Menu. Current State: " + InteractToObject.isItemMenuActive);
    //}
}
