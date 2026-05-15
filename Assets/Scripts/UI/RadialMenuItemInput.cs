using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialMenuItemInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private RadialMenu menu;
    public Sprite defaultSprite;
    public Sprite hoverSprite;
    RadialMenuItem item;
    RectTransform rect;
    Image background;

    Vector3 baseScale;
    Vector3 targetScale;

    [SerializeField] float hoverSmooth = 10f;

    [Header("Events")]
    [SerializeField] private UnityEvent onHoverEnterEvent; // Slot untuk menyalakan preview
    [SerializeField] private UnityEvent onHoverExitEvent;  // Slot untuk mematikan preview
    [SerializeField] private UnityEvent onClickAction;     // Slot untuk equip/klik utama

    private void Awake()
    {
        item = GetComponent<RadialMenuItem>();
        rect = GetComponent<RectTransform>();
        background = GetComponent<Image>();

        baseScale = rect.localScale;
        targetScale = baseScale;
    }

    private void Update()
    {
        rect.localScale = Vector3.Lerp(rect.localScale, targetScale, Time.unscaledDeltaTime * hoverSmooth);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = baseScale * menu.hoverScale;
        if (background) background.sprite = hoverSprite;

        if (!menu || !menu.IsOpen) return;

        item?.Invoke();

        if (item && item.IconSprite)
        {
            menu.SetCenterIcon(item.IconSprite, item.iconColor);
        }
        onHoverEnterEvent.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = baseScale;
        if (background) background.sprite = defaultSprite;
        onHoverExitEvent.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX(0); // Mainkan SFX klik tombol (asumsi index 0 adalah suara klik)
        onClickAction?.Invoke();
        Reset();
    }

    private void Reset()
    {
        targetScale = baseScale;
        if (background) background.sprite = defaultSprite;
        onHoverExitEvent?.Invoke();
    }
}
