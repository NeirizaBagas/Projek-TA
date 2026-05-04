using System;
using UnityEngine;
using UnityEngine.UI;

public class TrapProgresTracker : MonoBehaviour
{

    [Header("Trap Settings UI")]
    [SerializeField] private Slider trapSliderProgres;

    //private bool isDecreasing = false;
    //private bool isIncreasing = false;

    //public static event Action OnTrapDefuseComplete;

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

    //private void StartProgress()
    //{
    //    isIncreasing = true;
    //    isDecreasing = false;
    //}

    //private void DecreasProgress()
    //{
    //    isDecreasing = true;
    //    isIncreasing = false;
    //}

    //private void Update()
    //{
    //    if (isIncreasing)
    //    {
    //        trapSliderProgres.value += fillSpeed * Time.deltaTime; // Mengisi slider secara bertahap
    //        if (trapSliderProgres.value >= maxValue)
    //        {
    //            trapSliderProgres.value = minValue; // Reset slider setelah berhasil
    //            isIncreasing = false;
    //            OnTrapDefuseComplete?.Invoke();
    //        }
    //    }

    //    if (isDecreasing && trapSliderProgres.value > minValue)
    //    {
    //        trapSliderProgres.value -= 1f * Time.deltaTime; // Mengurangi slider secara bertahap
    //        if (trapSliderProgres.value <= minValue)
    //        {
    //            trapSliderProgres.value = minValue;
    //            isDecreasing = false;
    //        }
    //    }
    //}

    private void UpdateSlider(float currentProgress, float maxProgress)
    {
        trapSliderProgres.maxValue = maxProgress;
        trapSliderProgres.value = currentProgress;
    }
}
