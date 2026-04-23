using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RadialMenuItem : MonoBehaviour
{
    [Header("References")]  
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite iconSprite;

    public Sprite IconSprite => iconSprite;
    public UnityEvent onClick;

    public RectTransform IconRect => iconImage? iconImage.rectTransform : null;
    public Image BackgroundImage => backgroundImage;

    public Color iconColor;

    private void OnEnable()
    {
        EnsureReferences();
        ApplyIcon();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        EnsureReferences();
        ApplyIcon();
    }
#endif

    public void Invoke()
    {
        onClick?.Invoke();
    }

    public void EnsureReferences()
    {
        if (!backgroundImage) backgroundImage = GetComponent<Image>();
        if (!iconImage) iconImage = GetComponentInChildren<Image>();
    }

    void ApplyIcon()
    {
        if (iconImage) iconImage.sprite = iconSprite; 
    }

}
