using UnityEngine;

public class BombCabinCtrl : MonoBehaviour {
    [SerializeField]
    private float spawnInterval, spawnTime, weaponeUseTime, minVecSpeed, maxVecSpeed, minDistance, maxDistance;
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

        BossBomb.Pools.GetFromPool().Setup(cabinDoor.position, new Vector2(0, Random.Range(minVecSpeed, maxVecSpeed)), Random.Range(minDistance, maxDistance));
    }

    public void Activate() {
        if (activated) return;
        activated = true;
        spawing = true;
        intervalCount = 0;
        timer = 0;
    }
}