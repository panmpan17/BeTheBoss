using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace ReleaseVersion
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager ins;
        [SerializeField]
        private GameObject scorePanel;
        [SerializeField]
        private TextMeshProUGUI scoreText;
        // private string
        private PlayerContoller player;

        public const int BossLayer = 8,
            BossWeaponeLayer = 9,
            PlayerLayer = 10,
            PlayerWeaponeLayer = 11,
            WallLayer = 12;

        private void Awake()
        {
            ins = this;

            MedPack.Pools.SetupPrefab("Prefab/MedPack");
            BossBomb.Pools.SetupPrefab("Prefab/BossBomb");

            scorePanel.SetActive(false);
            WeaponePrefabPool.ClearAllPool();
            MedPack.Pools.Clear();
            BossBomb.Pools.Clear();

            player = FindObjectOfType<PlayerContoller>();
            player.ApplySetting(Setting.SettingReader.ReadPlayerSetting("JsonData/PlayerSetting"));
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
            while (!Input.anyKeyDown) {
                yield return null;
            }

            SceneManager.LoadScene("MainMenu");
        }
    }
}