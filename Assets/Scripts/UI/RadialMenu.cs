using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class RadialMenu : MonoBehaviour
{
    [Header("Layout")]
    public float radius = 150f;
    public float startAngle = 90f;
    public List<RectTransform> items = new();
    public GameObject RadialMenuObject;

    [Header("Button Design")]
    public Sprite buttonBackground;
    public Color defaultColor = Color.white;
    public Color hoverColor = Color.cyan;
    public float hoverScale = 1.15f;

    [Header("Icon Settings")]
    public Vector2 iconSize = new Vector2(32, 32);

    [Header("Open/Close")]
    public bool startOpen = true;
    public float openCloseDuration = 0.3f;
    public AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private CanvasGroup canvasGroup;
    Coroutine openCloseRoutine;
    
    public bool IsOpen { get; private set; }

    [Header("Center Icon")]
    [SerializeField] private Image centerIcon;
    [SerializeField] private Vector2 centerIconSize = new Vector2(48, 48);


    void OnEnable()
    {
        UpdateLayout();
        ApplyDefaultsToAll();

        if (startOpen)
            Open(true);
        else
            Close(true);

        if (centerIcon)
        {
            centerIcon.rectTransform.sizeDelta = centerIconSize;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying) return;

        UpdateLayout();
        ApplyDefaultsToAll();
    }
#endif

    void UpdateLayout()
    {
        int count = items.Count;
        if (count == 0) return;

        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            if (!items[i]) continue;

            RectTransform rect = items[i].GetComponent<RectTransform>();

            float angle = startAngle - step * i;
            float rad = angle * Mathf.Deg2Rad;

            rect.anchoredPosition = new Vector2(
                Mathf.Cos(rad) * radius,
                Mathf.Sin(rad) * radius
            );
        }
    }

    void SetInteractable(bool value)
    {
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }

    void SetVisualState(float v)
    {
        canvasGroup.alpha = v;
        if (RadialMenuObject != null) RadialMenuObject.transform.localScale = Vector3.one * v;
    }

    IEnumerator OpenCloseRoutine(float from, float to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / openCloseDuration;
            float v = Mathf.Lerp(from, to, openCurve.Evaluate(t));
            SetVisualState(v);
            yield return null;
        }

        SetVisualState(to);
    }

    void StartOpenClose(bool instant, float from, float to)
    {
        if (openCloseRoutine != null) StopCoroutine(openCloseRoutine);

        if (instant)
        {
            SetVisualState(to);
            return;
        }

        openCloseRoutine = StartCoroutine(OpenCloseRoutine(from, to));
    }

    public void Open(bool instant = false)
    {
        if (IsOpen) return;
        IsOpen = true;
        SetInteractable(true);
        StartOpenClose(instant, 0f, 1f);
    }

    public void Close(bool instant = false)
    {
        if (!IsOpen) return;
        IsOpen = false;
        SetInteractable(false);
        StartOpenClose(instant, 1f, 0f);
    }

    public void ApplyDefaultsToAll()
    {
        foreach (var item in items)
        {
            if (!item) continue;
            RadialMenuItem menuItem = item.GetComponent<RadialMenuItem>();
            if (menuItem == null) continue;
            ApplyDefaultsToItem(menuItem);
        }
    }

    public void ApplyDefaultsToItem(RadialMenuItem item)
    {
        if (!item) return;

        if (item.BackgroundImage)
        {
            item.BackgroundImage.sprite = buttonBackground;
            item.BackgroundImage.color = defaultColor;
        }

        //if (item.IconRect)
        //{
        //    item.IconRect.sizeDelta = iconSize;
        //}
    }

    public void ToggleItemMenu()
    {
        if (IsOpen) Close();
        else Open();
    }

    public void SetCenterIcon(Sprite sprite, Color color)
    {
        if (!centerIcon) return;

        centerIcon.sprite = sprite;
        centerIcon.enabled = sprite != null;
        centerIcon.color = color;
    }

}
