using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TrapInteract : MonoBehaviour, IHoldInteractable
{
    [Header("Trap Settings")]
    [SerializeField] private float timeToDefuse = 3f;
    [SerializeField] private float drainSpeedMultiplier = 1.5f;

    public string InteractMessage => "Hold [F] to Defuse";

    //[Header("UI Elements")]
    //[SerializeField] private TextMeshProUGUI textTrapInteract;

    private bool isDefused;
    private bool isHolding;
    private float currentProgress = 0f;

    public static event Action<bool> OnToggleUIInteract;
    public static event Action OnTrapDefused;
    //public static event Action OnTrapDefuseStarted;
    //public static event Action OnTrapDefuseFailed;
    public static event Action<float, float> OnUpdateProgressTrapUI;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        isHolding = false;
        isDefused = false;
        currentProgress = 0f;
    }

    private void OnDisable()
    {
        isHolding = false;
    }

    public void OnHoverEnter()
    {
        //if (!isDefused) textTrapInteract.text = "Hold [F] to Defuse";
        //Debug.Log("Trap Hovered");
    }

    public void OnHoverExit()
    {
        OnHoldCancel();
    }

    public void OnHoldStart()
    {
        if (isDefused) return;
        isHolding = true;
        OnToggleUIInteract?.Invoke(true);
        Debug.Log("Hold Started");

        //OnTrapDefuseStarted?.Invoke();
    }

    private void Update()
    {
        if (isDefused) return;

        if (isHolding)
        {
            currentProgress += Time.deltaTime;
            if (currentProgress >= timeToDefuse)
            {
                TrapDefused();
            }
        }
        else if (!isHolding && currentProgress > 0f)
        {
            currentProgress -= Time.deltaTime * drainSpeedMultiplier;
            if (currentProgress < 0f) currentProgress = 0f;
        }

        if (isHolding || currentProgress > 0f)
        {
            OnUpdateProgressTrapUI?.Invoke(currentProgress, timeToDefuse);
        }
        else if (currentProgress <= 0f && !isHolding)
        {
            //OnToggleUIInteract?.Invoke(false);
        }
    }

    private void TrapDefused()
    {
        isDefused = true;
        isHolding = false;
        currentProgress = 0f;
        OnToggleUIInteract?.Invoke(false);
        OnTrapDefused?.Invoke();
        transform.gameObject.SetActive(false);
    }

    public void OnHoldCancel()
    {
        if (isDefused) return;
        isHolding = false;
        OnToggleUIInteract?.Invoke(false);
    }

    public void OnHoldSuccess()
    {
        
    }

    
}
