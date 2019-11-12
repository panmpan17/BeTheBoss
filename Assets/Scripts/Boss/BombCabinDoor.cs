#pragma warning disable 649

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class BombCabinDoor : BossShipWeapon
{
    [SerializeField]
    private Transform burstTransform;
    [SerializeField]
    private float shootInterval, activeTime, speed, minDistance, maxDistance, torqueForce, rotateRangeMax, rotateRangeMin, rotateSpeed;
    private Timer shootTimer, activeTimer;
    private bool actived;

    private void Awake() {
        shootTimer = new Timer(shootInterval);
        activeTimer = new Timer(activeTime);
        enabled = false;
    }

    public void Rotate(int _drection)
    {
        float angle;
        Vector3 axis;
        transform.rotation.ToAngleAxis(out angle, out axis);
        if (_drection > 0 && angle > rotateRangeMax) return;
        else if (_drection < 0 && angle < rotateRangeMin) return;

        transform.Rotate(0, 0, _drection * rotateSpeed * Time.deltaTime);
    }

    private void Update() {
        if (!actived) return;

        if (shootTimer.UpdateEnd) {
            shootTimer.Reset();
            Spawn();
        }

        if (activeTimer.UpdateEnd) actived = enabled = false;
    }

    private void Spawn()
    {
        float angle;
        Vector3 axis;
        transform.rotation.ToAngleAxis(out angle, out axis);

        BossBomb.Pools.GetFromPool().Setup(
            burstTransform.position, transform.rotation,
            MachineGun.DegreeToVector2(angle + 90) * speed, Random.Range(minDistance, maxDistance));
    }

    public override void Activate() {
        base.Activate();
        actived = true;
        shootTimer.Reset();
        activeTimer.Reset();
        Debug.Log(1);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BombCabinDoor))]
    public class BombCabinDoorEditor : Editor
    {
        private void OnSceneGUI()
        {
            serializedObject.Update();
            Transform _transform = (target as BombCabinDoor).transform;

            SerializedProperty burstTProperty = serializedObject.FindProperty("burstTransform");
            Transform burstTransform = (Transform)burstTProperty.objectReferenceValue;

            Vector3 pos = Handles.PositionHandle(burstTransform.position, _transform.rotation);
            if (burstTransform.position != pos)
            {
                Undo.RecordObject(burstTransform, "Change burst position");
                burstTransform.position = pos;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
