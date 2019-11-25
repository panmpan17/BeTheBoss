using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private const float Z = -10;
    static public CameraControl ins;
    private new Camera camera;

    // Shaking
    private int shakeTime;
    private Timer shakInterval;
    private float shakeXdelta, shakeYdelta;
    private Vector3 originalPos;
    private bool shaking;

    private void Awake() {
        camera = GetComponent<Camera>();
        ins = this;
    }

    private void Update() {
        if (shaking) {
            if (shakInterval.UpdateEnd) {
                shakInterval.Reset();

                transform.position = originalPos + new Vector3(Random.Range(-shakeXdelta, shakeXdelta),
                                                               Random.Range(-shakeYdelta, shakeYdelta));

                shakeTime--;
                if (shakeTime <= 0) shaking = false;
            }
        }
    }

    public void ShakeCamera(ShakeInfo info) { ShakeCamera(info.Time, info.Interval, info.XDelta, info.YDelta, info.ForceShake); }
    public void ShakeCamera(int time = 3, float interval = 0.1f, float xDelta = 0.1f, float yDelta = 0.1f, bool forceShake = false) {
        if (shaking) {
            if (!forceShake) return;
            transform.position = originalPos;
        }

        shakeTime = time;
        shakInterval = new Timer(interval);
        shakeXdelta = xDelta;
        shakeYdelta = yDelta;
        originalPos = transform.position;
        originalPos.z = Z;
        shaking = true;
    }

    [System.Serializable]
    public struct ShakeInfo {
        public int Time;
        public float Interval, XDelta, YDelta;
        public bool ForceShake;
        
        public ShakeInfo(int time = 3, float interval = 0.1f, float xDelta = 0.1f, float yDelta = 0.1f, bool forceShake = false) {
            Time = time;
            Interval = interval;
            XDelta = xDelta;
            YDelta = yDelta;
            ForceShake = forceShake;
        }
    }
}
