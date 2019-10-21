using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ReleaseVersion.UI {
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class SelectableItem : MonoBehaviour
    {
        [SerializeField]
        private string arg;
        public string Arg { get { return arg; } }
        private bool selected, actived, disabled;

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
                else spriteRenderer.color = value;
            }
        }

        [SerializeField]
        private SelectableItem leftNavigate, rightNavigate, topNavigate, bottomNavigate;
        public SelectableItem NavLeft { get { return leftNavigate; } }
        public SelectableItem NavRight { get { return rightNavigate; } }
        public SelectableItem NavTop { get { return topNavigate; } }
        public SelectableItem NavBottom { get { return bottomNavigate; } }

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }
        public bool Active
        {
            get { return actived; }
            set { actived = value; }
        }
        public bool Disabled
        {
            get { return disabled; }
            set { disabled = value; }
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

            if (disabled)
            {
                color = style.DisabledColor;
            }
            else if (actived)
            {
                color = style.ActiveColor;
            }
            else if (selected)
            {
                color = style.SelectedColor;
            }
            else {
                color = style.NormalColor;
            }
        }

        private void Update() {
            // TODO: only apply when change happend
            ApplyStyle();
        }
    }
}