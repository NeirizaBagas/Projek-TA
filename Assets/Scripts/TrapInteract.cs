using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;

public class TrapInteract : MonoBehaviour
{
    [SerializeField] private GameObject trapContainerUI;
    [SerializeField] private Slider trapSliderProgres;
    int trapProgress = 0;

    public static Action OnTrapInteract;

    private void Awake()
    {
        
    }

    private void Start()
    {
        trapSliderProgres.value = trapProgress;
    }

    private void Update()
    {
        if (Keyboard.current.eKey.isPressed)
        {
            UpdateProgress();
        }
    }

    private void OnEnable()
    {
        Friends.OnInteractionStarted += UpdateProgress;
    }

    private void OnDisable()
    {
        
    }

    public void Interact()
    {
        
    }

    public void UpdateProgress()
    {
        trapContainerUI.SetActive(true);
        trapProgress++;
        trapSliderProgres.value = trapProgress;
        if (trapProgress >= 100)
        {
            Destroy(this.gameObject);
            trapContainerUI.SetActive(false);
        }
    }


}
