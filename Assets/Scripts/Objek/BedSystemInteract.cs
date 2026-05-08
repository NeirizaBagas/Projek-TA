using System;
using TMPro;
using UnityEngine;

public class BedSystemInteract : MonoBehaviour, ITapInteractable
{
    public string InteractMessage => "Press [F] to sleep";
    [SerializeField] private float skipTimeHours = 8f; // Jumlah jam yang akan dilewati saat tidur
    public static event Action<bool> ToggleSleepUI;
    public static event Action OnStartSleep;
    public static event Action<float> OnTimeSkip; // Event untuk melewati waktu, dengan parameter jumlah jam yang dilewati

    public void OnHoverEnter()
    {

    }
    public void OnHoverExit()
    {
        ToggleSleepUI?.Invoke(false);
    }
    public void OnTap()
    {
        OnStartSleep?.Invoke();
        OnTimeSkip?.Invoke(skipTimeHours);
    }
}
