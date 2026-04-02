using System;
using UnityEngine;
using UnityEngine.UI;

public class TrapProgresTracker : MonoBehaviour
{

    [Header("Trap Settings UI")]
    [SerializeField] private float fillSpeed = 3f;
    [SerializeField] private float maxValue = 10f;
    [SerializeField] private Slider trapSliderProgres;

    private bool isDecreasing = false;

    public static event Action OnTrapDefuseComplete;

    private void Start()
    {
        trapSliderProgres.minValue = 0f;
        trapSliderProgres.maxValue = maxValue;
        trapSliderProgres.value = 0f;
    }

    private void OnEnable()
    {
        TrapInteract.OnTrapDefuseStarted += StartDefusing;
        TrapInteract.OnTrapDefuseFailed += Decreasing;
    }

    private void OnDisable()
    {
        TrapInteract.OnTrapDefuseStarted -= StartDefusing;
        TrapInteract.OnTrapDefuseFailed -= Decreasing;
    }

    private void StartDefusing()
    {
        trapSliderProgres.value += fillSpeed * Time.deltaTime; // Mengisi slider secara bertahap
        if (trapSliderProgres.value >= maxValue)
        {
            OnTrapDefuseComplete?.Invoke();
        }
    }

    private void Decreasing()
    {
        isDecreasing = true;
    }

    private void Update()
    {
        if (isDecreasing && trapSliderProgres.value > trapSliderProgres.minValue)
        {
            trapSliderProgres.value -= 1f * Time.deltaTime; // Mengurangi slider secara bertahap
            if (trapSliderProgres.value <= trapSliderProgres.minValue)
            {
                trapSliderProgres.value = trapSliderProgres.minValue;
                isDecreasing = false;
            }
        }
    }

   
}
