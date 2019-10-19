using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReleaseVersion.UI {
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(Image))]
    public class SelectableItem : MonoBehaviour
    {
        [SerializeField]
        private string arg;
        public string Arg { get { return arg; } }
        private bool selected, actived, disabled;

        [SerializeField]
        private SelectableStyle style;
        private Image image;

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
            ApplyStyle();
        }

        private void ApplyStyle() {
            if (style == null) return;
            if (image == null) image = GetComponent<Image>();

            if (disabled)
            {
                image.color = style.DisabledColor;
            }
            else if (actived)
            {
                image.color = style.ActiveColor;
            }
            else if (selected)
            {
                image.color = style.SelectedColor;
            }
            else {
                image.color = style.NormalColor;
            }
        }

        private void Update() {
            // TODO: only apply when change happend
            ApplyStyle();
        }
    }
}