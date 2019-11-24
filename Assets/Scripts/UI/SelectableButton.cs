using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Audio;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Menu {
    #if UNITY_EDITOR
    [CanEditMultipleObjects]
    #endif
    public abstract class Selectable : MonoBehaviour {
        protected Image image;
        protected TextMeshProUGUI text;
        protected SpriteRenderer spriteRenderer;

        protected Color Color
        {
            get
            {
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

		[SerializeField]
		protected SelectableStyle style;
		[System.NonSerialized]
		protected bool selected, actived, disabled;

        public bool Disable {
            get { return disabled; }
            set {
                disabled = value;
                ApplyStyle();
            }
        }

		public abstract bool Left(ref Selectable menuSelected);
        public abstract bool Right(ref Selectable menuSelected);
        public abstract bool Up(ref Selectable menuSelected);
		public abstract bool Down(ref Selectable menuSelected);
        public abstract void Submit();

        protected virtual void Awake() {
            image = GetComponent<Image>();
            text = GetComponent<TextMeshProUGUI>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (image == null && text == null && spriteRenderer == null)
            {
                Debug.LogError("SelectableItem must have Image, TextMeshProUGUI or SpriteRenderer");
                enabled = false;
                return;
            }

            ApplyStyle();
        }

        public bool Select {
            get { return selected; }
            set {
                selected = value;
                ApplyStyle();
                AudioManager.ins.PlayerSound(AudioEnum.UISelect);
            }
        }

		public virtual void ApplyStyle() {
            if (style == null) return;
			Color color;
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
            else
            {
                color = style.NormalColor;
            }

            Color = color;
		}

        protected bool ChangeNav(ref Selectable menuSelected, Selectable selectable)
        {
            if (selectable == null) return false;
            selected = false;
            menuSelected = selectable;
            menuSelected.Select = true;
            ApplyStyle();
            return true;
        }
	}

	public class SelectableButton : Selectable {

    #if UNITY_EDITOR
        [MenuItem("GameObject/UI/Selectable Button")]
        static public void OnCreate() {
            GameObject obj = new GameObject("Selectable Button", typeof(RectTransform));

            if (Selection.activeGameObject) {
                obj.GetComponent<RectTransform>().parent = Selection.activeGameObject.transform;
            } else {
                obj.GetComponent<RectTransform>().parent = FindObjectOfType<Canvas>().transform;
            }
            obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            obj.AddComponent<SelectableButton>();

            Selection.activeGameObject = obj;
        }
    #endif

		[SerializeField]
		private Selectable left, right, up, down;

		[SerializeField]
		private UnityEvent submitEvent;

        public override bool Left(ref Selectable menuSelected) { return ChangeNav(ref menuSelected, left); }
        public override bool Right(ref Selectable menuSelected) { return ChangeNav(ref menuSelected, right); }
        public override bool Up(ref Selectable menuSelected) { return ChangeNav(ref menuSelected, up); }
        public override bool Down(ref Selectable menuSelected) { return ChangeNav(ref menuSelected, down); }

        public override void Submit() {
            if (disabled || actived) return;
            AudioManager.ins.PlayerSound(AudioEnum.UIClick);
            submitEvent.Invoke();
        }
	}
}