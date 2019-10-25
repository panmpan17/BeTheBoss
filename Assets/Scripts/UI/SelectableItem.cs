#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

// IPointerClickHandler
// public void OnPointerClick(PointerEventData eventData) // 3
// {
//     print("I was clicked");
//     target = Color.blue;
// }
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class SelectableItem : MonoBehaviour
{
    static public AudioSource audioSource;

    [SerializeField]
    private SelectableStyle style;
    private Image image;
    private TextMeshProUGUI text;
    private SpriteRenderer spriteRenderer;

    private Color color {
        get {
            if (image != null) return image.color;
            else if (text != null) return text.color;
            else return spriteRenderer.color;
        }
        set
        {
            if (image != null) image.color = value;
            else if (text != null) text.color = value;
            else if (spriteRenderer != null) spriteRenderer.color = value;
            else Debug.LogError(gameObject, gameObject);
        }
    }

    private bool selected, actived, disabled;

    [SerializeField]
    private SelectableItem leftNavigate, rightNavigate, topNavigate, bottomNavigate;
    public SelectableItem NavLeft { get { return leftNavigate; } }
    public SelectableItem NavRight { get { return rightNavigate; } }
    public SelectableItem NavTop { get { return topNavigate; } }
    public SelectableItem NavBottom { get { return bottomNavigate; } }

    [SerializeField]
    private UnityEvent activeEvent, selectedEvent;

    public bool Selected
    {
        get { return selected; }
        set { selected = value; ApplyStyle(); }
    }
    public bool Active
    {
        get { return actived; }
        set { actived = value; ApplyStyle(); }
    }
    public bool Disabled
    {
        get { return disabled; }
        set { disabled = value; ApplyStyle(); }
    }

    private void Awake() {
        image = GetComponent<Image>();
        text = GetComponent<TextMeshProUGUI>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (image == null && text == null && spriteRenderer == null) {
            Debug.LogError("SelectableItem must have Image, TextMeshProUGUI or SpriteRenderer");
            enabled = false;
            return;
        }

        ApplyStyle();
    }

    private void ApplyStyle() {
        if (style == null) return;
        if (image == null) image = GetComponent<Image>();

        if (!disabled && !actived && !selected) {
            color = style.NormalColor;
        } else if (disabled) {
            if (selected) color = Color.Lerp(style.SelectedColor, style.DisabledColor, 0.5f);
            else color = style.DisabledColor;
        } else if (actived) {
            if (selected) color = Color.Lerp(style.SelectedColor, style.ActiveColor, 0.5f);
            else color = style.ActiveColor;
        } else if (selected) color = style.SelectedColor;
    }

    public void Activate() {
        if (activeEvent != null) activeEvent.Invoke();
        if (audioSource != null) AudioManager.ins.PlayerSound(audioSource, AudioType.Click);
    }

    public void Select() {
        Selected = true;
        if (selectedEvent != null) selectedEvent.Invoke();
        if (audioSource != null) AudioManager.ins.PlayerSound(audioSource, AudioType.Select);
    }
}