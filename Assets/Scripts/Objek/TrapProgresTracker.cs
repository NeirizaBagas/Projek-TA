using System;
using UnityEngine;
using UnityEngine.UI;

public class TrapProgresTracker : MonoBehaviour
{

    [Header("Trap Settings UI")]
    [SerializeField] private Slider trapSliderProgres;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        TrapInteract.OnUpdateProgressTrapUI += UpdateSlider;
    }

    private void OnDisable()
    {
        TrapInteract.OnUpdateProgressTrapUI -= UpdateSlider;
    }

    private void UpdateSlider(float currentProgress, float maxProgress)
    {
        trapSliderProgres.maxValue = maxProgress;
        trapSliderProgres.value = currentProgress;
    }
}
