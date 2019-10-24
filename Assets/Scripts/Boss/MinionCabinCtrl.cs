#pragma warning disable 649

using UnityEngine;
using ReleaseVersion;

// TODO: shoot bomb
public class MinionCabinCtrl : MonoBehaviour {
    [SerializeField]
    private float spawnInterval, spawnTime, WeaponUseTime;
    [SerializeField]
    private CabinDoor[] cabinDoors;
    private float intervalCount, timer;

    private bool activated, spawing;

    void Update() {
        if (activated) {
            timer += Time.deltaTime;

            if (spawing) {
                intervalCount += Time.deltaTime;
                if (intervalCount >= spawnInterval) {
                    intervalCount = 0;

                    for (int i = 0; i < cabinDoors.Length; i++)
                    {
                        // CabinDoor cabinDoor = cabinDoors[i];
                        WeaponPrefabPool.GetPool(WeaponType.Minion).GetFromPool().Setup(cabinDoors[i].door.position, cabinDoors[i].defaultVelocity);
                    }
                }
            }

            if (timer >= spawnTime) spawing = false;
            if (timer >= WeaponUseTime) {
                activated = false;
                Boss.ins.WeaponFinished();
            }
        }
    }

    public void Activate() {
        activated = true;
        spawing = true;
        timer = 0;
    }

    [System.Serializable]
    private class CabinDoor {
        public Transform door;
        public Vector2 defaultVelocity;
    }
}