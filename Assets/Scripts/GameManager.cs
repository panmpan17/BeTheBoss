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
    private PlayerContoller player;


    public bool GamePaused { get { return Time.timeScale == 0; } }

    public const int BossLayer = 8,
        BossWeaponLayer = 9,
        PlayerLayer = 10,
        PlayerWeaponLayer = 11,
        WallLayer = 12;

    private void Awake()
    {
        ins = this;

        BossBomb.Pools.SetupPrefab("Prefab/BossBomb");

        scorePanel.SetActive(false);
        WeaponPrefabPool.ClearAllPool();
        RewardPrefabPool.ClearAllPool();

        BossBomb.Pools.Clear();
        // BossBomb.Pools.AddInstantiateEvent(delegate (BossBomb bomb) {
        //     Physics2D.IgnoreCollision(bomb.GetComponent<Collider2D>(), topCeilingCollider, true);
        // });

        player = FindObjectOfType<PlayerContoller>();
        player.ApplySetting(SettingReader.ReadPlayerSetting("JsonData/PlayerSetting"));

        if (!MultiLanguageMgr.jsonLoaded) MultiLanguageMgr.LoadJson();
        if (!PlayerPreference.loaded) PlayerPreference.ReadFromSavedPref();
        MultiLanguageMgr.SwitchAllTextsLanguage(PlayerPreference.Language);

        pauseMenu.gameObject.SetActive(false);
    }

    private void Update() {
        if (Input.GetButtonDown("Cancel")) {
            Time.timeScale = 0;
            pauseMenu.gameObject.SetActive(true);
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