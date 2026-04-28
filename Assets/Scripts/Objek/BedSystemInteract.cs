using System;
using TMPro;
using UnityEngine;

public class BedSystemInteract : MonoBehaviour, ITapInteractable
{
    [SerializeField] private TextMeshProUGUI textItemInteract;
    [SerializeField] private float skipTimeHours = 8f; // Jumlah jam yang akan dilewati saat tidur
    public static event Action OnCloseSleepUI;
    public static event Action OnStartSleep;
    public static event Action<float> OnTimeSkip; // Event untuk melewati waktu, dengan parameter jumlah jam yang dilewati

    public void OnHoverEnter()
    {
        textItemInteract.text = "Press [F] to sleep";
    }
    public void OnHoverExit()
    {
        OnCloseSleepUI?.Invoke();
    }
    public void OnTap()
    {
        Debug.Log("Tapped on " + this.name + "Reset Energy" + "TimeSkip");
        OnStartSleep?.Invoke();
        OnTimeSkip?.Invoke(skipTimeHours);
    }
}
