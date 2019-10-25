#pragma warning disable 649

using UnityEngine;


// TODO: shoot bomb
public class MinionCabinCtrl : MonoBehaviour {
    [SerializeField]
    private float spawnInterval, spawnTime;
    [SerializeField]
    private CabinDoor[] cabinDoors;
    private Timer spawnTimer, activeTimer;

    private bool activated;

    private void Awake() {
        spawnTimer = new Timer(spawnInterval);
        activeTimer = new Timer(spawnTime);
    }

    void Update() {
        if (!activated) return;

        if (spawnTimer.UpdateEnd) {
            spawnTimer.Reset();

            for (int i = 0; i < cabinDoors.Length; i++)
            {
                // CabinDoor cabinDoor = cabinDoors[i];
                WeaponPrefabPool.GetPool(WeaponType.Minion).GetFromPool().Setup(cabinDoors[i].door.position, cabinDoors[i].defaultVelocity);
            }
        }

        if (activeTimer.UpdateEnd)
        {
            activeTimer.Reset();
            activated = false;
        }
    }

    public void Activate() {
        activated = true;
    }

    [System.Serializable]
    private class CabinDoor {
        public Transform door;
        public Vector2 defaultVelocity;
    }
}