using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionCabinDoor : MonoBehaviour
{
    [SerializeField]
    private Vector2 defaultVelocity;
    [SerializeField]
    private float spawnInterval, activeTime;
    private Timer spawnTimer, activeTimer;
    private bool active;

    private void Awake() {
        spawnTimer = new Timer(spawnInterval);
        activeTimer = new Timer(activeTime);
    }

    private void Update() {
        if (!active) return;

        if (spawnTimer.UpdateEnd) {
            spawnTimer.Reset();
            WeaponPrefabPool.GetPool(WeaponType.Minion).GetFromPool().Setup(transform.position, defaultVelocity);
        }

        if (activeTimer.UpdateEnd) active = false;
    }

    public void Activate() {
        active = true;
        spawnTimer.Reset();
        activeTimer.Reset();
    }
}
