using UnityEngine;

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

        // BossBomb.Pools.SetupPrefab("Prefab/BossBomb");
        // PlayerMissle.Pools.SetupPrefab("Prefab/PlayerMissle");
        // PlayerBullet.Pools.SetupPrefab("Prefab/PlayerBullet");
    }
}