using System;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Header("Item Reference")]
    [SerializeField] private GameObject senter;

    [Header("Bool Reference")]
    [SerializeField] private bool isSenterActive;
    [SerializeField] private bool isJournalActive;

    [Header("Event Reference")]
    public static Action<bool> OnTriggerJournal;

    private void OnEnable()
    {

    }

    //public void OpenCloseJournalTrigger()
    //{
    //    if (isJournalActive)
    //    {

    //    }
    //    else
    //    {
    //        Debug.Log("TesOpen");
    //        OpenJournal();
    //    }
    //}

    public void OpenJournal()
    {
        Debug.Log("Open Journal");
        OnTriggerJournal?.Invoke(true);
    }

    //private void CloseJournal()
    //{
    //    if (!isJournalActive) return;
    //    isJournalActive = false;
    //    Debug.Log("Close Journal");
    //    OnTriggerJournal?.Invoke(false);
    //}
}
