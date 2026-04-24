using System;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public int animalIndexPhoto;

    [Header("Item Reference")]
    [SerializeField] private GameObject senter;

    [Header("Bool Reference")]
    [SerializeField] private bool isSenterActive;
    [SerializeField] private bool isReadyToDefuse;

    [Header("Event Reference")]
    public static Action<bool> OnTriggerJournal;
    public static event Action<int> OnAnimalPhotoRequested;
    public static event Action<bool> OnPhotoModeStarted;
    public static event Action<bool> OnPhotoUiTriggered;
    public static event Action<bool> OnReadyToDefuse;

    private void OnEnable()
    {

    }

    public void OpenJournal()
    {
        OnTriggerJournal?.Invoke(true);
    }

    public void OpenDefuseKit()
    {
        if (!isReadyToDefuse)
        {
            isReadyToDefuse = true;
            OnReadyToDefuse?.Invoke(isReadyToDefuse);
        }
        else
        {
            isReadyToDefuse = false;
            OnReadyToDefuse.Invoke(isReadyToDefuse);
        }
    }

    public void CameraMode()
    {
        OnAnimalPhotoRequested?.Invoke(animalIndexPhoto);
        OnPhotoModeStarted?.Invoke(true);
        OnPhotoUiTriggered?.Invoke(true);
    }

    public void ToggleSenter()
    {
        if (isSenterActive)
        {
            senter.SetActive(false);
            isSenterActive = false;
        }
        else
        {
            isSenterActive = true;
            senter.SetActive(true);
        }
    }
}
