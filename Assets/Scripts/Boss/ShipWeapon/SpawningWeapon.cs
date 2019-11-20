using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpawningWeapon : BossShipWeapon
{
    [SerializeField]
    private WeaponType spawnWeaponeType;

    [SerializeField]
    private Transform spawningPos;
    [SerializeField]
    private float spawningTime, spawningIntervalTime;
    private Timer spawningTimer, spawningIntervalTimer;
    [SerializeField]
    private bool useCustomVec;
    [SerializeField]
    private Vector2 initialVec;

    [SerializeField]
    private RotateType rotateType = RotateType.None;
    private enum RotateType { None, Auto, Manual }
    [SerializeField]
    private bool revertRotate;
    [SerializeField]
    private int angleMin, angleMax, rotateDirection = 1;
    [SerializeField]
    private float rotateSpeed;

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    private void Awake() {
        spawningTimer = new Timer(spawningTime);
        spawningIntervalTimer = new Timer(spawningIntervalTime);
        enabled = false;
    }

    private void Update() {
        if (!spawningTimer.UpdateEnd) {
            if (spawningIntervalTimer.UpdateEnd) {
                spawningIntervalTimer.Reset();

                Spawn();
            }
        }

        if (rotateType == RotateType.Auto) {
            transform.Rotate(0, 0, rotateDirection * rotateSpeed * Time.deltaTime);

            Vector3 axis;
            float angle;
            transform.rotation.ToAngleAxis(out angle, out axis);

            if (rotateDirection > 0 && angle > angleMax) rotateDirection *= -1;
            else if (rotateDirection < 0 && angle < angleMin) rotateDirection *= -1;
        }
    }

    public override void Left() { Rotate(-1); }
    public override void Right() { Rotate(1); }

    public void Rotate(int _drection)
    {
        if (!enabled || rotateType != RotateType.Manual) return;

        if (revertRotate) _drection *= -1;

        float angle;
        Vector3 axis;
        transform.rotation.ToAngleAxis(out angle, out axis);
        if (_drection > 0 && angle > angleMax) return;
        else if (_drection < 0 && angle < angleMin) return;

        transform.Rotate(0, 0, _drection * rotateSpeed * Time.deltaTime);
    }

    void Spawn() {
        float angle;
        Vector3 axis;
        transform.rotation.ToAngleAxis(out angle, out axis);

        Weapon bullet = WeaponPrefabPool.GetPool(spawnWeaponeType).GetFromPool();

        bullet.Setup(spawningPos.position, useCustomVec? initialVec : DegreeToVector2(angle + 90) * 3, transform.rotation);
    }

    public override void Activate() {
        base.Activate();
        spawningTimer.Reset();
        spawningIntervalTimer.Reset();
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(SpawningWeapon))]
    public class _Editor : Editor {
        SerializedProperty rotateType;

        private void OnEnable() {
            rotateType = serializedObject.FindProperty("rotateType");
        }

        public static void IntMinMaxSlider(string name, SerializedProperty property1, SerializedProperty property2, int rangeMin, int rangeMax) {
            float min = property1.intValue, max = property2.intValue;
            EditorGUILayout.LabelField(name);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property1, GUIContent.none, GUILayout.Width(40));
            // EditorGUILayout.MinMaxSlider(ref min, ref max, rangeMin, rangeMax);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.MinMaxSlider(ref min, ref max, rangeMin, rangeMax);
            if (EditorGUI.EndChangeCheck())
            {
                property1.intValue = Mathf.RoundToInt(min);
                property2.intValue = Mathf.RoundToInt(max);
                return;
            }

            EditorGUILayout.PropertyField(property2, GUIContent.none, GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI() {
            // DrawDefaultInspector();

            GUILayout.Space(8);
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));

            EditorGUILayout.LabelField("Spawning Control", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawningPos"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnWeaponeType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawningTime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawningIntervalTime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useCustomVec"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("initialVec"));

            EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(rotateType);
            RotateType _rotateType = (RotateType) rotateType.enumValueIndex;

            switch (_rotateType) {
                case RotateType.Auto:
                    IntMinMaxSlider("Allow Rotation Angle", serializedObject.FindProperty("angleMin"), serializedObject.FindProperty("angleMax"), -360, 360);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("rotateSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("rotateDirection"), new GUIContent("Initial Rotation Direction"));
                    break;
                case RotateType.Manual:
                    IntMinMaxSlider("Allow Rotation Angle", serializedObject.FindProperty("angleMin"), serializedObject.FindProperty("angleMax"), -360, 360);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("rotateSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("revertRotate"));
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void OnSceneGUI()
        {
            serializedObject.Update();
            Transform spawningPos = (Transform)serializedObject.FindProperty("spawningPos").objectReferenceValue;

            if (spawningPos == null) return;

            Transform _transform = (target as SpawningWeapon).transform;

            Vector3 pos = Handles.PositionHandle(spawningPos.position, _transform.rotation);
            if (spawningPos.position != pos)
            {
                Undo.RecordObject(spawningPos, "Change spawning pos");
                spawningPos.position = pos;
            }
        }
    }
    #endif
}
