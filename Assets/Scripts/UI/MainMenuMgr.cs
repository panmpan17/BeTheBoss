#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MultiLanguage;

public class MainMenuMgr : MonoBehaviour
{
    private const string gameScene = "GameRelease";
    private bool loadingScene;
    [SerializeField]
    private SelectableItem selected;
    [SerializeField]
    private Transform canvas;
    private SettingMenu settingMenu;
    private bool usingSetting;

    private void Awake() {
        if (!MultiLanguageMgr.jsonLoaded) MultiLanguageMgr.LoadJson();
        if (!PlayerPreference.loaded) PlayerPreference.ReadFromSavedPref();
        MultiLanguageMgr.SwitchAllTextsLanguage(PlayerPreference.Language);

        settingMenu = Instantiate(Resources.Load<GameObject>("Prefab/SettingMenu"), canvas).GetComponent<SettingMenu>();
        settingMenu.gameObject.SetActive(false);
    }

    private void Start() {
        selected.Selected = true;
        settingMenu.SetupCloseEvent(delegate { usingSetting = false; });
    }

    public void Play() {
        loadingScene = true;
        SceneManager.LoadSceneAsync(gameScene);
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
        if (loadingScene || usingSetting) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            selected.Selected = false;
            selected = selected.NavTop;
            selected.Selected = true;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            selected.Selected = false;
            selected = selected.NavBottom;
            selected.Selected = true;
        }
        else if (Input.GetButtonDown("Submit")) selected.Activate();
    }
}