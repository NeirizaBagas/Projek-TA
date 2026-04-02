using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;

public class TrapInteract : MonoBehaviour, IHoldInteractable
{
    [SerializeField] private bool isDefused;
    private bool isHolding;

    public static event Action OnOpenTrapUI;
    public static event Action OnCloseTrapUI;
    public static event Action OnTrapDefused;
    public static event Action OnTrapDefuseStarted;
    public static event Action OnTrapDefuseFailed;

    private void Awake()
    {
        OnCloseTrapUI?.Invoke();
        isHolding = false;
        isDefused = false;
    }

    private void OnEnable()
    {
        TrapProgresTracker.OnTrapDefuseComplete += TrapDefused;
    }

    private void OnDisable()
    {
        TrapProgresTracker.OnTrapDefuseComplete -= TrapDefused;
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
        OnOpenTrapUI?.Invoke();
    }

    private void Update()
    {
        if (isHolding && !isDefused)
        {
            OnTrapDefuseStarted?.Invoke();
        }
    }

    private void TrapDefused()
    {
        isDefused = true;
        isHolding = false;
        OnCloseTrapUI?.Invoke();
        OnTrapDefused?.Invoke();
        transform.gameObject.SetActive(false);
    }

    public void OnHoldCancel()
    {
        isHolding = false;
        OnCloseTrapUI?.Invoke();
        OnTrapDefuseFailed?.Invoke();
    }

    public void OnHoldSuccess()
    {
        
    }

    
}
