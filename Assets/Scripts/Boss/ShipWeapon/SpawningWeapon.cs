using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class SpawningWeapon : BossShipWeapon
{
    static private Vector2 MiltiplyVector2(Vector2 first, Vector2 second)
    {
        return new Vector2(first.x * second.x, first.y * second.y);
    }

    [SerializeField]
    private WeaponType spawnWeaponeType;

    [SerializeField]
    private int spawnLimit = -1;
    [SerializeField]
    private SpawnType spawnType;
    private enum SpawnType { OneByOne, AllAtOnce }
    [SerializeField]
    private Transform[] spawningPos;
    public Transform[] SpawningPos { get { return spawningPos; } }
    private int spawningPosIndex;
    [SerializeField]
    private float spawningTime, spawningIntervalTime;
    private Timer spawningTimer, spawningIntervalTimer;
    [SerializeField]
    private Vector2 vecMultiplier;

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

        spawningPos = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            spawningPos[i] = transform.GetChild(i);
        }
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
        WeaponPrefabPool pool = WeaponPrefabPool.GetPool(spawnWeaponeType);
        if (spawnLimit > -1 && pool.AliveObjects.Count >= spawnLimit) return;

        if (spawnType == SpawnType.OneByOne) {
            spawningPosIndex++;
            if (spawningPosIndex >= spawningPos.Length) spawningPosIndex = 0;

            float angle;
            Vector3 axis;
            spawningPos[spawningPosIndex].rotation.ToAngleAxis(out angle, out axis);
            angle += 90 + Random.Range(-2f, 2f);

            Weapon bullet = pool.GetFromPool();

            bullet.Setup(spawningPos[spawningPosIndex].position, MiltiplyVector2(DegreeToVector2(angle), vecMultiplier), rotateType == RotateType.None? Quaternion.identity : transform.rotation);
        } else {
            for (spawningPosIndex = 0; spawningPosIndex < spawningPos.Length; spawningPosIndex++) {
                float angle;
                Vector3 axis;
                spawningPos[spawningPosIndex].rotation.ToAngleAxis(out angle, out axis);

                Weapon bullet = pool.GetFromPool();
                angle += 90 + Random.Range(-2f, 2f);

                bullet.Setup(spawningPos[spawningPosIndex].position, MiltiplyVector2(DegreeToVector2(angle), vecMultiplier), rotateType == RotateType.None ? Quaternion.identity : transform.rotation);
            }
        }
    }

    public override void Activate() {
        base.Activate();
        spawningTimer.Reset();
        spawningIntervalTimer.Reset();
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(SpawningWeapon))]
    public class _Editor : Editor {
        SpawningWeapon weapon;
        SerializedProperty rotateType;
        ReorderableList spawnPos;

        private void OnEnable() {
            weapon = (SpawningWeapon) target;
            rotateType = serializedObject.FindProperty("rotateType");

            spawnPos = new ReorderableList(serializedObject, serializedObject.FindProperty("spawningPos"));
            spawnPos.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Spawninng Position"); };
            spawnPos.drawElementCallback = (rect, index, _1, _2) => {
                // EditorGUI.ObjectField(rect, weapon.spawningPos[index], typeof(Transform), false);
                EditorGUI.PropertyField(rect, spawnPos.serializedProperty.GetArrayElementAtIndex(index));
            };
        }

        public static void IntMinMaxSlider(string name, SerializedProperty property1, SerializedProperty property2, int rangeMin, int rangeMax) {
            float min = property1.intValue, max = property2.intValue;
            EditorGUILayout.LabelField(name);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property1, GUIContent.none, GUILayout.Width(40));

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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnLimit"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnType"));
            GUILayout.Space(5);

            spawnPos.DoLayoutList();
            if (GUILayout.Button("Scan")) {
                Undo.RecordObject(weapon, "");

                Transform[] children = new Transform[weapon.transform.childCount];
                for (int i = 0; i < weapon.transform.childCount; i++) children[i] = weapon.transform.GetChild(i);
                weapon.spawningPos = children;
            }
            GUILayout.Space(5);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnWeaponeType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawningTime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawningIntervalTime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("vecMultiplier"));

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
            if (weapon.SpawningPos == null) return;

            for (int i = 0; i < weapon.SpawningPos.Length; i++) {
                EditorGUI.BeginChangeCheck();
                Vector3 pos = Handles.PositionHandle(weapon.SpawningPos[i].position, weapon.SpawningPos[i].rotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(weapon.SpawningPos[i], "Change spawning pos");
                    weapon.SpawningPos[i].position = pos;
                }
            }
        }
    }
    #endif
}
