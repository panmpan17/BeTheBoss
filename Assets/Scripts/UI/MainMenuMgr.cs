#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MultiLanguage;

public class MainMenuMgr : MonoBehaviour
{
    static public MainMenuMgr ins;

    private const string GAMERELEASE_SCENE = "GameRelease",
        LEVELSELECT_SCENE = "LevelSelect",
        SMASHMENU_SCENE = "SmashMenu";
    private bool loadingScene;
    [SerializeField]
    private SelectableItem selected;
    [SerializeField]
    private Transform canvas;
    [SerializeField]
    private GameObject intro;
    private SettingMenu settingMenu;
    private bool usingSetting;

    private void Awake() {
        ins = this;

        if (!MultiLanguageMgr.jsonLoaded) MultiLanguageMgr.LoadJson();
        if (!PlayerPreference.loaded) PlayerPreference.ReadFromSavedPref();

        intro.SetActive(!PlayerPreference.SkipIntro);

        MultiLanguageMgr.SwitchAllTextsLanguage(PlayerPreference.Language);

        settingMenu = Instantiate(Resources.Load<GameObject>("Prefab/SettingMenu"), canvas).GetComponent<SettingMenu>();
        settingMenu.gameObject.SetActive(false);

        SelectableItem.audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start() {
        selected.Selected = true;
        settingMenu.SetupCloseEvent(delegate { usingSetting = false; });
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
        SelectableItem.audioSource = null;
    }

    public void Update() {
        if (loadingScene || usingSetting || intro.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            selected.Selected = false;
            selected = selected.NavTop;
            selected.Select();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            selected.Selected = false;
            selected = selected.NavBottom;
            selected.Select();
        }
        else if (Input.GetButtonDown("Submit")) selected.Activate();
    }
}