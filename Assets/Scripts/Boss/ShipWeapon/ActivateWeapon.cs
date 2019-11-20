using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActivateWeapon : BossShipWeapon
{
    [SerializeField]
    private Weapon activatingWeapon;

    [SerializeField]
    private MoveType moveType;
    private enum MoveType { None, Weapon, Ship }
    // [SerializeField]
    // private bool moveBack;
    [SerializeField]
    private float moveBackSpeed;
    private Vector3 originalPosition;
    [SerializeField]
    private GameObject targetWeapon;
    [SerializeField]
    private float delayTime;
    private Timer delayTimer;
    [SerializeField]
    private float moveMinX, moveMinY, moveMaxX, moveMaxY;
    [SerializeField]
    private Vector2 leftVec, rightVec;

    private bool movable, movingBack;

    Transform target;

    public override void Left() { if (moveType != MoveType.None) Move(-1); }
    public override void Right() { if (moveType != MoveType.None) Move(1); }

    public void Move(int direction) {
        if (!movable) return;

        Transform target = moveType == MoveType.Weapon ? transform : Boss.ins.transform;
        if (direction < 0 && target.position.x > moveMinX) target.Translate(leftVec * Time.deltaTime);
        else if (direction > 0 && target.position.x < moveMaxX) target.Translate(rightVec * Time.deltaTime);
    }

    private void Awake() {
        delayTimer = new Timer(delayTime);
    }

    private void Start() {
        target = moveType == MoveType.Weapon ? transform : Boss.ins.transform;
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingBack) {
            target.position = Vector3.MoveTowards(target.position, originalPosition, moveBackSpeed * Time.deltaTime);

            if ((originalPosition - target.position).sqrMagnitude < 0.05f) {
                target.position = originalPosition;
                movingBack = false;
                enabled = false;
            }
        }
        else if (delayTimer.UpdateEnd && !movable)
        {
            movable = true;
            targetWeapon.SetActive(true);
        }
    }

    public override void Activate() {
        base.Activate();
        delayTimer.Reset();
        movingBack = movable = false;
        originalPosition = target.position;
    }
    public override bool Deactivate() {
        if (!movingBack) {
            movable = false;
            movingBack = true;
            targetWeapon.SetActive(false);
        }
        return !enabled;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ActivateWeapon))]
    public class _Editor : Editor {
        SerializedProperty moveType;

        private void OnEnable() {
            moveType = serializedObject.FindProperty("moveType");
        }

        static public void FloatMinMaxSlider(string name, SerializedProperty property1, SerializedProperty property2, float rangeMin, float rangeMax)
        {
            float min = property1.floatValue, max = property2.floatValue;
            EditorGUILayout.LabelField(name);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property1, GUIContent.none, GUILayout.Width(40));

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.MinMaxSlider(ref min, ref max, rangeMin, rangeMax);
            if (EditorGUI.EndChangeCheck()) {
                property1.floatValue = min;
                property2.floatValue = max;
                return;
            }

            EditorGUILayout.PropertyField(property2, GUIContent.none, GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.Space(8);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
            EditorGUILayout.PropertyField(moveType);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveBack"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetWeapon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveBackSpeed"));
            FloatMinMaxSlider("X Moveble Range", serializedObject.FindProperty("moveMinX"), serializedObject.FindProperty("moveMaxX"), -10, 10);
            FloatMinMaxSlider("Y Moveble Range", serializedObject.FindProperty("moveMinY"), serializedObject.FindProperty("moveMaxY"), -10, 10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftVec"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightVec"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
