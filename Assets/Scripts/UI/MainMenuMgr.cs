#pragma warning disable 649

using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MultiLanguage;
using Saving;
using Audio;
using Menu;

public class MainMenuMgr : MonoBehaviour
{
    static public MainMenuMgr ins;

    private const string GAMERELEASE_SCENE = "GameRelease",
        LEVELSELECT_SCENE = "LevelSelect",
        SMASHMENU_SCENE = "SmashMenu";
    private bool loadingScene;
    [SerializeField]
    private Selectable selected;
    [SerializeField]
    private Transform canvas;
    [SerializeField]
    private GameObject intro;
    private SettingMenu settingMenu;
    private bool usingSetting;

    public void InitialLoading() {
        if (!MultiLanguageMgr.jsonLoaded) MultiLanguageMgr.LoadJson();
        if (!PlayerPreference.loaded) PlayerPreference.ReadFromSavedPref();
        if (!SavingMgr.Loaded) SavingMgr.LoadSaving();
        MultiLanguageMgr.SwitchAllTextsLanguage(PlayerPreference.Language);
        PlayerPreference.ApplyAllPreference();
    }

    private void Awake() {
        ins = this;
        InitialLoading();
        intro.SetActive(!PlayerPreference.SkipIntro);

        settingMenu = Instantiate(Resources.Load<GameObject>("Prefab/SettingMenu"), canvas).GetComponent<SettingMenu>();
        settingMenu.gameObject.SetActive(false);
    }

    private void Start() {

        selected.Select = true;
        settingMenu.SetupCloseEvent(delegate { usingSetting = false; });

        AudioManager.ins.PlayBGM(AudioEnum.MenuBGM);
    }

    public void LoadStoryMode() {
        loadingScene = true;
        ins = null;
        SceneManager.LoadSceneAsync(LEVELSELECT_SCENE);
    }

    public void LoadSmashMenu() {
        loadingScene = true;
        ins = null;
        SceneManager.LoadSceneAsync(SMASHMENU_SCENE);
    }

    public void OpenSetting() {
        usingSetting = true;
        settingMenu.gameObject.SetActive(true);
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Update() {
        if (loadingScene || usingSetting || intro.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) selected.Up(ref selected);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) selected.Down(ref selected);
        else if (Input.GetButtonDown("Submit")) selected.Submit();
    }
}