using UnityEngine;

public class CabinDoor : MonoBehaviour {
    [SerializeField]
    private WeaponeType spawnType;
    [SerializeField]
    private float spawnInterval;
    [SerializeField]
    private Vector2 prefabDefaultVelocity;
    private float intervalCount;
    private bool spawning;

    void Update() {
        if (spawning) {
            intervalCount += Time.deltaTime;
            if (intervalCount >= spawnInterval) {
                intervalCount = 0;

                Spawn();
            }
        }
    }

    public void Spawn() {
        Weapone weapone = Weapone.Spawn(spawnType);
        weapone.Set(transform.position, prefabDefaultVelocity);
    }
}