using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MultiLanguage {
    public class MultiLanguageText : MonoBehaviour
    {
		[SerializeField]
		private int id;
		public int Id { get { return id; } }

		private TextMesh textMesh;
		private TextMeshProUGUI textMeshUGUI;

		public string Text {
			get {
				if (textMesh != null) return textMesh.text;
				else return textMeshUGUI.text;
			}
			set {
                if (textMesh != null) textMesh.text = value;
                else if (textMeshUGUI != null) textMeshUGUI.text = value;
			}
		}

		public void ChangeId(int _id) {
			id = _id;
			Text = MultiLanguageMgr.GetTextById(PlayerPreference.Language, Id);
		}

        private void Awake() {
			textMesh = GetComponent<TextMesh>();
            textMeshUGUI = GetComponent<TextMeshProUGUI>();

			if (textMesh == null && textMeshUGUI == null) {
				Debug.LogError("MultiLanguageText require Text or TextMesh or TMP text");
				enabled = false;
				return;
			}

            MultiLanguageMgr.AddText(this);
		}

		private void Start() {
            if (MultiLanguageMgr.jsonLoaded) Text = MultiLanguageMgr.GetTextById(PlayerPreference.Language, Id);
		}
    }
}