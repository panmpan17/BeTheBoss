#pragma warning disable 649

using UnityEngine;
using ReleaseVersion;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MachineGun : MonoBehaviour {

    [SerializeField]
    private Transform burstTransform;
    [SerializeField]
    private float rotateRangeMax, rotateRangeMin, rotateSpeed, fireRate, bulletSpeed, shootTime;
    [SerializeField]
    private bool isAimMode;
    public bool IsAimMode { get { return isAimMode; } }
    private Timer fireRateTimer, activeTimer;
    private bool activated;
    private int direction = 1;

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    void Awake() {
        fireRateTimer = new Timer(fireRate);
        activeTimer = new Timer(shootTime);
    }

    public void Rotate(int _drection) {
        float angle;
        Vector3 axis;
        transform.rotation.ToAngleAxis(out angle, out axis);
        if (_drection > 0 && angle > rotateRangeMax) return;
        else if (_drection < 0 && angle < rotateRangeMin) return;

        transform.Rotate(0, 0, _drection * rotateSpeed * Time.deltaTime);
    }

    void Update() {
        if (!activated) return;

        if (!isAimMode) {
            transform.Rotate(0, 0, direction * rotateSpeed * Time.deltaTime);

            Vector3 axis;
            float angle;
            transform.rotation.ToAngleAxis(out angle, out axis);

            if (direction > 0 && angle > rotateRangeMax) direction *= -1;
            else if (direction < 0 && angle < rotateRangeMin) direction *= -1;
        }

        if (fireRateTimer.UpdateEnd) {
            fireRateTimer.Reset();

            float angle;
            Vector3 axis;
            transform.rotation.ToAngleAxis(out angle, out axis);

            WeaponPrefabPool.GetPool(WeaponType.BossBullet).GetFromPool().Setup(burstTransform.position, DegreeToVector2(angle + 90) * bulletSpeed, transform.rotation);
        }

        if (activeTimer.UpdateEnd) {
            activeTimer.Reset();

            activated = false;
        }
    }

    public void Activate() {
        activated = true;
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(MachineGun))]
    public class MachineGunEditor : Editor {
        private void OnSceneGUI() {
            serializedObject.Update();
            Transform _transform = (target as MachineGun).transform;

            SerializedProperty burstTProperty = serializedObject.FindProperty("burstTransform");
            Transform burstTransform = (Transform) burstTProperty.objectReferenceValue;

            Vector3 pos = Handles.PositionHandle(burstTransform.position, _transform.rotation);
            if (burstTransform.position != pos) {
                Undo.RecordObject(burstTransform, "Change burst position");
                burstTransform.position = pos;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}