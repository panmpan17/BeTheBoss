#pragma warning disable 649

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MultiLanguage;

public class GameManager : MonoBehaviour
{
    public static GameManager ins;
    [SerializeField]
    private GameObject scorePanel;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private Collider2D topCeilingCollider;
    [SerializeField]
    private PauseMenu pauseMenu;


    public bool GamePaused { get { return Time.timeScale == 0; } }

    public const int BossLayer = 8,
        BossWeaponLayer = 9,
        PlayerLayer = 10,
        PlayerWeaponLayer = 11,
        WallLayer = 12;

    private void Awake()
    {
        ins = this;

        scorePanel.SetActive(false);
        WeaponPrefabPool.ClearAllPool();
        RewardPrefabPool.ClearAllPool();
        BossBomb.Pools.SetupPrefab("Prefab/BossBomb");
        BossBomb.Pools.Clear();

        if (!MultiLanguageMgr.jsonLoaded) MultiLanguageMgr.LoadJson();
        if (!PlayerPreference.loaded) PlayerPreference.ReadFromSavedPref();
        MultiLanguageMgr.SwitchAllTextsLanguage(PlayerPreference.Language);

        pauseMenu.gameObject.SetActive(false);
        SelectableItem.audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update() {
        if (Input.GetButtonDown("Cancel")) {
            Time.timeScale = 0;
            pauseMenu.gameObject.SetActive(true);
            PlayerContoller.ins.Pause();
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

        SceneManager.LoadScene("MainMenu");
    }
}