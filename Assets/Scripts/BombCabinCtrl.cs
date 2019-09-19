using UnityEngine;

public class BombCabinCtrl : MonoBehaviour {
    [SerializeField]
    private float spawnInterval, spawnTime, weaponeUseTime, minVecY, maxVecy;
    [SerializeField]
    private Transform[] cabinDoors;
    private float intervalCount, timer;

    private bool activated, spawing;

    void Update() {
        if (activated) {
            timer += Time.deltaTime;

            if (spawing) {
                intervalCount += Time.deltaTime;
                if (intervalCount >= spawnInterval) {
                    intervalCount = 0;
                    Spawn();
                }
            }

            if (timer >= spawnTime) spawing = false;
            if (timer >= weaponeUseTime) {
                activated = false;
                Boss.ins.WeaponeFinished();
            }
        }
    }

    public void Spawn() {
        Transform cabinDoor = cabinDoors[Random.Range(0, cabinDoors.Length)];
        Weapone weapone = Weapone.Spawn(WeaponeType.Minion);
        weapone.Set(cabinDoor.position, new Vector2(0, Random.Range(minVecY, maxVecy)));
    }

    public void Activate() {
        if (!activated) return;
    }
}