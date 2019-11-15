#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Audio;

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
    [SerializeField]
    protected SelectableStyle style;
    protected Image image;
    protected TextMeshProUGUI text;
    protected SpriteRenderer spriteRenderer;

    protected Color color {
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

    protected bool selected, actived, disabled;

    [SerializeField]
    protected SelectableItem leftNavigate, rightNavigate, topNavigate, bottomNavigate;
    public virtual SelectableItem NavLeft { get { return leftNavigate; } set { leftNavigate = value; } }
    public virtual SelectableItem NavRight { get { return rightNavigate; } set { rightNavigate = value; } }
    public virtual SelectableItem NavTop { get { return topNavigate; } set { topNavigate = value; } }
    public virtual SelectableItem NavBottom { get { return bottomNavigate; } set { bottomNavigate = value; } }

    [SerializeField]
    protected UnityEvent activeEvent, selectedEvent;

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

    protected void Awake() {
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

    protected void ApplyStyle() {
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

    public virtual void Activate() {
        if (activeEvent != null) activeEvent.Invoke();
        AudioManager.ins.PlayerSound(AudioEnum.UIClick);
    }

    public void AddActivateEvent(UnityAction newEvent) {
        activeEvent.AddListener(newEvent);
    }

    public void Select() {
        Selected = true;
        if (selectedEvent != null) selectedEvent.Invoke();
        AudioManager.ins.PlayerSound(AudioEnum.UISelect);
    }
}