using UnityEngine;
using MultiLanguage;
using System;

static public class PlayerPreference
{
    private const string MUSIC_VOLUME_KEY = "music_volume",
        SOUND_VOLUME_KEY = "sound_volume",
        RESOLUTION_INDEX_KEY = "resolution",
        FULLSCREEN_KEY = "fullscreen",
        LANGUAGE_INDEX_KEY = "language",
        SKIP_INTRO_KEY = "skipIntro";
    static public Vector2Int[] resolutions = new[] {
        new Vector2Int(720, 450),
        new Vector2Int(1080, 675),
        new Vector2Int(1280, 800),
        new Vector2Int(1600, 1000),
        new Vector2Int(1920, 1200),
        new Vector2Int(2400, 1500),
        new Vector2Int(2880, 1800)
    };

    static public bool loaded;
    static private int languagesLength = Enum.GetNames(typeof(Language)).Length;

    // Saved data
    static private float musicVolume = 1;
    static private float soundVolume = 1;
    static private int resolutionIndex = 1;
    static private int languageIndex = 0;
    static private int fullscreen = 1;
    static private int skipIntro;

    // Actuall data
    static public float MusicVolume
    {
        get { return musicVolume; }
        set
        {
            musicVolume = Mathf.Clamp(value, 0, 1);
        }
    }
    static public float SoundVolume
    {
        get { return soundVolume; }
        set
        {
            soundVolume = Mathf.Clamp(value, 0, 1);
        }
    }

    static public int ResolutionIndex { get { return resolutionIndex; } set {
        resolutionIndex = Mathf.Clamp(value, 0, resolutions.Length - 1);
    } }
    static public Vector2Int Resolution { get { return resolutions[resolutionIndex]; } }

    static public Language Language { get { return (Language) languageIndex; } }
    static public int LanguageIndex { get { return languageIndex; } set {
        if (value < 0) languageIndex = languagesLength - 1;
        else if (value >= languagesLength) languageIndex = 0;
        else languageIndex = value;
    }}

    static public bool Fullscreen { get { return fullscreen == 1; } set { fullscreen = value ? 1 : 0; } }
    static public bool SkipIntro { get { return skipIntro == 1; } set { skipIntro = value ? 1 : 0; } }

    static public void ApplyAllPreference() {
        ApplyVolume();
        ApplyResolution();
        ApplyLanguage();
    }
    static public void ApplyVolume() {
        Audio.AudioManager.ins.ApplyVolume(soundVolume, musicVolume);
    }
    static public void ApplyResolution()
    {
        Screen.SetResolution(Resolution.x, Resolution.y, PlayerPreference.Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
    }
    static public void ApplyLanguage() 
    {
        MultiLanguageMgr.SwitchAllTextsLanguage(PlayerPreference.Language);
    }

    static public void ReadFromSavedPref() {
        loaded = true;
        if (PlayerPrefs.HasKey(MUSIC_VOLUME_KEY)) musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
        if (PlayerPrefs.HasKey(SOUND_VOLUME_KEY)) soundVolume = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY);
        if (PlayerPrefs.HasKey(RESOLUTION_INDEX_KEY)) ResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY);
        if (PlayerPrefs.HasKey(FULLSCREEN_KEY)) fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY);
        if (PlayerPrefs.HasKey(LANGUAGE_INDEX_KEY)) LanguageIndex = PlayerPrefs.GetInt(LANGUAGE_INDEX_KEY);
        if (PlayerPrefs.HasKey(SKIP_INTRO_KEY)) skipIntro = PlayerPrefs.GetInt(SKIP_INTRO_KEY);
    }

    static public void Save() {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, soundVolume);
        PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, ResolutionIndex);
        PlayerPrefs.SetInt(FULLSCREEN_KEY, fullscreen);
        PlayerPrefs.SetInt(LANGUAGE_INDEX_KEY, LanguageIndex);
        PlayerPrefs.SetInt(SKIP_INTRO_KEY, skipIntro);
    }

    static public void ResetDefault() {
        musicVolume = 1;
        soundVolume = 1;
        resolutionIndex = 1;

        switch(Application.systemLanguage) {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
            case SystemLanguage.ChineseTraditional:
                languageIndex = 0;
                break;
            default:
                languageIndex = 1;
                break;
        }

        languageIndex = 0;
        
        fullscreen = 1;
        skipIntro = 0;

        Save();
    }
}
