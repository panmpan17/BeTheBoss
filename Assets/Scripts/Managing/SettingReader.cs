using System.Collections.Generic;
using UnityEngine;

namespace Setting {
    public static class SettingReader
    {
        private const string LEVEL_SETTING_JSON_PATH = "JsonData/LevelSetting";

        private static bool levelSettingLoaded;
        private static Dictionary<string, Data.LevelSetting> levelSettingDict = new Dictionary<string, Data.LevelSetting>();

        public static bool TryLoadLevelSetting() {
            if (levelSettingLoaded) { return true; }

            TextAsset _text = Resources.Load<TextAsset>(LEVEL_SETTING_JSON_PATH);

            #if UNITY_EDITOR
            if (_text == null) {
                Debug.LogErrorFormat("Can't load '{0}'", LEVEL_SETTING_JSON_PATH);
                return false;
            }
            #endif

            Data.LevelSettingJson setting = JsonUtility.FromJson<Data.LevelSettingJson>(_text.text);
            Resources.UnloadAsset(_text);

            for (int i = 0; i < setting.Levels.Length; i++) {
                Data.LevelSetting level = setting.Levels[i];
                if (levelSettingDict.ContainsKey(level.Scene)) {
                    #if UNITY_EDITOR
                    Debug.LogErrorFormat("Level setting dict alreay has key '{0}'", level.Scene);
                    #endif
                } else {
                    levelSettingDict.Add(level.Scene, level);
                }
            }

            levelSettingLoaded = true;
            return true;
        }

        public static bool TryGetLevelSetting(string sceneName, out Data.LevelSetting setting) {
            return levelSettingDict.TryGetValue(sceneName, out setting);
        }
    }
}

namespace Setting.Data {
    [System.Serializable]
    public class LevelSettingJson
    {
        public LevelSetting[] Levels;
    }

    [System.Serializable]
    public class LevelSetting
    {
        public string Scene;
        public PlayerSetting Player;
    }

    [System.Serializable]
    public class PlayerSetting
    {
        public int StartingHealth;
        public float FlySpeed;
        public float RebirthPortection;
        public int Life;

        public float BulletFireRate;
        public float BulletSpeed;

        public int MissleStartCount;
        public int MissleMaxCount;
        public float MissleSpeed;
    }
}