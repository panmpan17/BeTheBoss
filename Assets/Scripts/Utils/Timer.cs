using UnityEngine;

public struct Timer {
    private float targetTime, counting;
    public Timer(float time) {
        targetTime = time;
        counting = 0;
    }

    public void Reset() {
        counting = 0;
    }

    public float Progress { get { return counting / targetTime; } }
    public bool UpdateEnd { get { counting += Time.deltaTime; return counting >= targetTime; }}
    public bool FixedUpdateEnd { get { counting += Time.fixedDeltaTime; return counting >= targetTime; }}
}