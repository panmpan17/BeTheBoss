using UnityEngine;

[System.Serializable]
public class PlayerSetting {
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

public class SettingReader {
    public static PlayerSetting ReadPlayerSetting(string path) {
        TextAsset _text = Resources.Load<TextAsset>(path);
        PlayerSetting setting = JsonUtility.FromJson<PlayerSetting>(_text.text);
        Resources.UnloadAsset(_text);

        return setting;
    }
}