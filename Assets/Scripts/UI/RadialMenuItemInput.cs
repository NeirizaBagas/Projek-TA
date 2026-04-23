using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialMenuItemInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private RadialMenu menu;
    [SerializeField] private UnityEvent onClickAction;
    RadialMenuItem item;
    RectTransform rect;
    Image background;

    Vector3 baseScale;
    Vector3 targetScale;

    [SerializeField] float hoverSmooth = 10f;

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
        if (background) background.color = menu.hoverColor;

        if (!menu || !menu.IsOpen) return;

        item?.Invoke();

        if (item && item.IconSprite)
        {
            menu.SetCenterIcon(item.IconSprite, item.iconColor);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = baseScale;
        if (background) background.color = menu.defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickAction?.Invoke();
    }
}
