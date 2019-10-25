using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiLanguage {
    public class MultiLanguageText : MonoBehaviour
    {
		[SerializeField]
		private int id;
		public int Id { get { return id; } }

		private TextMesh textMesh;
		private Text uiText;
		private TextMeshProUGUI tmp_text;

		public string Text {
			get {
				if (textMesh != null) return textMesh.text;
				else if (tmp_text != null) return tmp_text.text;
				else return uiText.text;
			}
			set {
                if (textMesh != null) textMesh.text = value;
                else if (uiText != null) uiText.text = value;
				else if (tmp_text != null) tmp_text.text = value;
			}
		}

		public void ChangeId(int _id) {
			id = _id;
			Text = MultiLanguageMgr.GetTextById(PlayerPreference.Language, Id);
		}

        private void Awake() {
			textMesh = GetComponent<TextMesh>();
			uiText = GetComponent<Text>();
            tmp_text = GetComponent<TextMeshProUGUI>();

			if (textMesh == null && uiText == null && tmp_text == null) {
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