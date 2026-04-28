using System;
using TMPro;
using UnityEngine;

public class BedSystemInteract : MonoBehaviour, ITapInteractable
{
    [SerializeField] private TextMeshProUGUI textItemInteract;
    public static event Action OnCloseSleepUI;
    public static event Action OnStartSleep;

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
    }
}
