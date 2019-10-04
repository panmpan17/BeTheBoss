using UnityEngine;

namespace ReleaseVersion
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager ins;

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
        }
    }
}