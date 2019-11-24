using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiLanguage {
	public enum Language { en_us, zh_tw }

    public static class MultiLanguageMgr
    {
        private const string jsonPath = "JsonData/MultiLanguage";

		static public bool jsonLoaded;
		static private Dictionary<int, Dictionary<string, string>> languagesDict = new Dictionary<int, Dictionary<string, string>>();

        static private List<MultiLanguageText> texts = new List<MultiLanguageText>();
        static public void AddText(MultiLanguageText text) { texts.Add(text); }
        static public void ClearTexts() { texts.Clear(); }

        static public string GetLanguageName(Language _language)
        {
            switch (_language)
            {
                case Language.en_us:
                    return "English";
                case Language.zh_tw:
                    return "繁體中文";
                default:
                    return "Unknown";
            }
        }

		static public void LoadJson() {
            TextAsset _text = Resources.Load<TextAsset>(jsonPath);
            LanguageSetting setting = JsonUtility.FromJson<LanguageSetting>(_text.text);
            Resources.UnloadAsset(_text);

            languagesDict = new Dictionary<int, Dictionary<string, string>>();

            TextLanguage[] languages = setting.Languages;
            for (int i = 0; i < languages.Length; i++)
            {
                languagesDict.Add(languages[i].ID, languages[i].GetDictionary());
            }

            jsonLoaded = true;
		}

        static public string GetTextById(Language languageType, int id) { return GetTextById(System.Enum.GetName(typeof(Language), languageType), id); }
		static public string GetTextById(string languageType, int id) {
			if (!languagesDict.ContainsKey(id)) {
                Debug.LogWarningFormat("Text has no '{0}'", id);
				id = 0;
			}
            string text;
            if (languagesDict[id].TryGetValue(languageType, out text)) return text;

            Debug.LogErrorFormat("Text '{0}' has no language '{1}'", id, languageType);

			return null;
		}

		static public void SwitchAllTextsLanguage(Language language) {
            string languageType = System.Enum.GetName(typeof(Language), language);
            for (int i = 0; i < texts.Count; i++) {
				texts[i].Text = GetTextById(languageType, texts[i].Id);
			}
		}

    #if UNITY_EDITOR
        [MenuItem("Michael/掃描所有使用文字")]
        static public void ScaneMultiText() {
            TextAsset _text = Resources.Load<TextAsset>(jsonPath);
            LanguageSetting setting = JsonUtility.FromJson<LanguageSetting>(_text.text);
            Resources.UnloadAsset(_text);

            List<char> allChars = new List<char>();
            string basics = " 0123456789abcdefghijklmnopqrstuvewxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_+,./<>?;':\"[]\\{}|";
            for (int i = 0; i < basics.Length; i++) {
                allChars.Add(basics[i]);
            }

            for (int i = 0; i < setting.Languages.Length; i++) {
                for (int e = 0; e < setting.Languages[i].zh_tw.Length; e++)
                {
                    if (!allChars.Contains(setting.Languages[i].zh_tw[e])) allChars.Add(setting.Languages[i].zh_tw[e]);
                }
                for (int e = 0; e < setting.Languages[i].en_us.Length; e++)
                {
                    if (!allChars.Contains(setting.Languages[i].en_us[e])) allChars.Add(setting.Languages[i].en_us[e]);
                }
            }

            Debug.Log(new string(allChars.ToArray()));
        }
    #endif
    }

	[System.Serializable]
	public class LanguageSetting {
		public TextLanguage[] Languages;
    }

    [System.Serializable]
    public class TextLanguage
    {
        public int ID;
        public string en_us, zh_tw;

		public override string ToString() {
			return string.Format("({0}) en_us: {1} zh_tw: {2}", ID, en_us, zh_tw);
		}

		public Dictionary<string, string> GetDictionary() {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            System.Type type = GetType();
            System.Reflection.FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++) {
				string name = fields[i].Name;
				if (name == "ID") {
					continue;
				}
				dict.Add(name, (string) fields[i].GetValue(this));
            }

			return dict;
		}
    }
}