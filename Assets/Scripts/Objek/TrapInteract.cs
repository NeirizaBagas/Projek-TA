using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;

public class TrapInteract : MonoBehaviour, IHoldInteractable
{
    [Header("Trap Settings")]
    [SerializeField] private float fillSpeed = 3f;
    [SerializeField] private float maxValue = 10f;

    [Header("Trap Defuse UI")]
    [SerializeField] private GameObject trapContainerUI;
    [SerializeField] private Slider trapSliderProgres;

    [SerializeField] private bool isDefused;
    private bool isHolding;

    public static event Action OnTrapDefused;

    private void Awake()
    {

        trapSliderProgres.minValue = 0f;
        trapSliderProgres.maxValue = maxValue;
        trapSliderProgres.value = 0f;
        trapContainerUI.SetActive(false);
        isHolding = false;
        isDefused = false;
    }

    public void OnHoverEnter()
    {
        Debug.Log("Hovering over Trap.");
    }

    public void OnHoverExit()
    {
        OnHoldCancel();
    }

    public void OnHoldStart()
    {
        if (isDefused) return;
        isHolding = true;
        trapContainerUI.SetActive(true);
    }

    private void Update()
    {
        if (isHolding && !isDefused)
        {
            trapSliderProgres.value += fillSpeed * Time.deltaTime;
            if (trapSliderProgres.value >= maxValue)
            {
                TrapDefused();
            }
        }
    }

    private void TrapDefused()
    {
        isDefused = true;
        isHolding = false;
        trapContainerUI.SetActive(false);
        Debug.Log("Bom defused, mission success!");
        OnTrapDefused?.Invoke();
        transform.gameObject.SetActive(false);
    }

    public void OnHoldCancel()
    {
        isHolding = false;
        trapContainerUI.SetActive(false);
    }

    public void OnHoldSuccess()
    {
        
    }

    
}
