using UnityEngine;

// TODO: shoot bomb
public class MinionCabinCtrl : MonoBehaviour {
    [SerializeField]
    private float spawnInterval, spawnTime, weaponeUseTime;
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
        for (int i = 0; i < cabinDoors.Length; i++) {
            CabinDoor cabinDoor = cabinDoors[i];
            Weapone weapone = Weapone.Spawn(WeaponeType.Minion);
            weapone.Set(cabinDoor.door.position, cabinDoor.defaultVelocity);
        }
    }

    public void Activate() {
        Debug.Log("active minon");
        activated = true;
        spawing = true;
        timer = 0;
        // spawnTime = 0;
        // weaponeUseTime = 0;
    }

    [System.Serializable]
    private class CabinDoor {
        public Transform door;
        public Vector2 defaultVelocity;
    }
}