using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Menu {
	public class SideEffectorSet : Selectable {
	#if UNITY_EDITOR
		[MenuItem("GameObject/UI/Side Effector Set")]
		static public void OnCreate()
		{
			GameObject obj = new GameObject("Side Effector Set", typeof(RectTransform));

			if (Selection.activeGameObject)
			{
				obj.GetComponent<RectTransform>().parent = Selection.activeGameObject.transform;
			}
			else
			{
				obj.GetComponent<RectTransform>().parent = FindObjectOfType<Canvas>().transform;
			}
			obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			obj.AddComponent<SideEffectorSet>();

			Selection.activeGameObject = obj;
		}
	#endif

		[SerializeField]
		private Graphic leftEffector, rightEffector;
        [SerializeField]
		private Selectable up, down;
		[SerializeField]
		private float delayTime;
		private bool delay;
		private Timer delayTimer;

		[SerializeField]
		private UnityEvent leftEvent, righEvent;

		protected override void Awake() {
			base.Awake();
            leftEffector.color = style.NormalColor;
			rightEffector.color = style.NormalColor;
            delayTimer = new Timer(delayTime);
		}

		public override bool Left(ref Selectable menuSelected) {
			AudioManager.ins.PlayerSound(AudioEnum.UIClick);
			Color = style.NormalColor;
			leftEffector.color = style.SelectedColor;
			leftEvent.Invoke();
            delay = true;
            delayTimer.Reset();
			return true;
		}

		public override bool Right(ref Selectable menuSelected) {
            AudioManager.ins.PlayerSound(AudioEnum.UIClick);
            Color = style.NormalColor;
			rightEffector.color = style.SelectedColor;
			righEvent.Invoke();
            delay = true;
			delayTimer.Reset();
			return true;
		}

		private void Update() {
			if (delay && delayTimer.UnscaleUpdateTimeEnd) {
                rightEffector.color = style.NormalColor;
                leftEffector.color = style.NormalColor;
                Color = style.SelectedColor;
                delay = false;
			}
		}
		public override bool Up(ref Selectable menuSelected) {
			if (delay) return false;
			return ChangeNav(ref menuSelected, up);
		}
		public override bool Down(ref Selectable menuSelected) {
			if (delay) return false;
			return ChangeNav(ref menuSelected, down);
		}
		public override void Submit() {}
	}
}