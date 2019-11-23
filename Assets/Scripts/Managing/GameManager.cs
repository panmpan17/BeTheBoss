#pragma warning disable 649

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MultiLanguage;
using Audio;
using UnityEngine.UI;
using Setting;
using Setting.Data;

public class GameManager : MonoBehaviour
{
    static public GameManager ins;
    [SerializeField]
    private GameObject scorePanel;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private Collider2D topCeilingCollider;
    [SerializeField]
    private PauseMenu pauseMenu;

    private string activeSceneName;

    private Canvas mainCanvas;
    public Canvas MainCanvas { get {
        if (mainCanvas == null) mainCanvas = FindObjectOfType<Canvas>();
        return mainCanvas;
    } }

    public bool GamePaused { get { return Time.timeScale == 0; } }

    public const int BossLayer = 8,
        BossWeaponLayer = 9,
        PlayerLayer = 10,
        PlayerWeaponLayer = 11,
        WallLayer = 12;

    private void Awake()
    {
        ins = this;
        activeSceneName = SceneManager.GetActiveScene().name;

        scorePanel.SetActive(false);
        WeaponPrefabPool.ClearAllPool();
        RewardPrefabPool.ClearAllPool();

        if (!MultiLanguageMgr.jsonLoaded) MultiLanguageMgr.LoadJson();
        if (!PlayerPreference.loaded) PlayerPreference.ReadFromSavedPref();
        MultiLanguageMgr.SwitchAllTextsLanguage(PlayerPreference.Language);

        pauseMenu.gameObject.SetActive(false);

        StartCoroutine(LoadSetting());
    }

    private IEnumerator LoadSetting() {
        yield return new WaitForEndOfFrame();
        if (SettingReader.TryLoadLevelSetting())
        {
            LevelSetting setting;
            if (SettingReader.TryGetLevelSetting(activeSceneName, out setting))
            {
                PlayerContoller.ins.ApplySetting(setting.Player);
                PlayerContoller.ins.enabled = true;
            }
            else
            {
                Debug.LogErrorFormat("Can't find '{0}' level setting", activeSceneName);
            }
        }
    }

    public Canvas GetCopyOfCanvas(string name="New Canvas") {
        CanvasScaler mainCanvasCaler = MainCanvas.GetComponent<CanvasScaler>();

        GameObject newCanvasObj = new GameObject(name, typeof(RectTransform));
        Canvas newCanvas = newCanvasObj.AddComponent<Canvas>();
        CanvasScaler newCanvasScaler = newCanvasObj.AddComponent<CanvasScaler>();

        newCanvas.renderMode = MainCanvas.renderMode;
        newCanvas.sortingOrder = MainCanvas.sortingOrder;
        newCanvas.pixelPerfect = MainCanvas.pixelPerfect;
        newCanvas.additionalShaderChannels = MainCanvas.additionalShaderChannels;

        newCanvasScaler.uiScaleMode = mainCanvasCaler.uiScaleMode;
        newCanvasScaler.referenceResolution = mainCanvasCaler.referenceResolution;
        newCanvasScaler.screenMatchMode = mainCanvasCaler.screenMatchMode;
        newCanvasScaler.matchWidthOrHeight = mainCanvasCaler.matchWidthOrHeight;
        newCanvasScaler.referencePixelsPerUnit = mainCanvasCaler.referencePixelsPerUnit;

        return newCanvas;
    }

    private void Update() {
        if (Input.GetButtonDown("Cancel")) {
            Time.timeScale = 0;
            pauseMenu.gameObject.SetActive(true);
            AudioManager.ins.Pause();
        }
    }

    public void BossLose() {
        scorePanel.SetActive(true);
        scoreText.text = "Player Win";
        StartCoroutine(WaitInputToReturnManu());
    }

    public void PlayerLose() {
        scorePanel.SetActive(true);
        scoreText.text = "Boss Win";
        StartCoroutine(WaitInputToReturnManu());
    }

    IEnumerator WaitInputToReturnManu() {
        while (!Input.GetButton("Submit")) {
            yield return null;
        }

        SceneManager.LoadScene("LevelSelect");
    }
}