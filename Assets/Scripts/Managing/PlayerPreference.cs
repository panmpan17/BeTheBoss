using UnityEngine;
using MultiLanguage;
using System;

public class PlayerPreference
{
    private const string RESOLUTION_INDEX_KEY = "resolution",
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
    static private int resolutionIndex;
    static private int languageIndex = 0;
    static private int fullscreen;
    static private int skipIntro;

    // Actuall data
    static public int ResolutionIndex { get { return resolutionIndex; } set {
        if (value >= resolutions.Length) resolutionIndex = 0;
        else if (value < 0) resolutionIndex = resolutions.Length - 1;
        else resolutionIndex = value;
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

    static public void ReadFromSavedPref() {
        loaded = true;
        if (PlayerPrefs.HasKey(RESOLUTION_INDEX_KEY)) ResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY);
        if (PlayerPrefs.HasKey(FULLSCREEN_KEY)) fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY);
        if (PlayerPrefs.HasKey(LANGUAGE_INDEX_KEY)) LanguageIndex = PlayerPrefs.GetInt(LANGUAGE_INDEX_KEY);
        if (PlayerPrefs.HasKey(SKIP_INTRO_KEY)) skipIntro = PlayerPrefs.GetInt(SKIP_INTRO_KEY);
    }

    static public void Save() {
        PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, ResolutionIndex);
        PlayerPrefs.SetInt(FULLSCREEN_KEY, fullscreen);
        PlayerPrefs.SetInt(LANGUAGE_INDEX_KEY, LanguageIndex);
        PlayerPrefs.SetInt(SKIP_INTRO_KEY, skipIntro);
    }
}
