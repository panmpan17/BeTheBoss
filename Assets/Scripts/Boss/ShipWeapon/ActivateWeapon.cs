using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActivateWeapon : BossShipWeapon
{
    [SerializeField]
    private GameObject targetWeapon;
    [SerializeField]
    private float delayTime;
    private Timer delayTimer;

    [SerializeField]
    private Vector2 moveVec;
    [SerializeField]
    private bool moveBack;
    private Vector3 originalPosition;

    private bool movable;

    private MovableBoss movableBoss;
    Animator animator;

    public override void Left() {
        if (!enabled || !movable) return;
        movableBoss.ApplyForce(-moveVec * Time.deltaTime);
    }
    public override void Right() {
        if (!enabled || !movable) return;
        movableBoss.ApplyForce(moveVec * Time.deltaTime);
    }

    private void Awake() {
        delayTimer = new Timer(delayTime);
        animator = GetComponent<Animator>();
        movableBoss = transform.parent.GetComponent<MovableBoss>();
    }

    private void Start() {
        enabled = false;
    }

    void Update()
    {
        if (delayTimer.UpdateEnd && !movable)
        {
            movable = true;
            targetWeapon.SetActive(true);
        }
    }

    public override void Activate() {
        base.Activate();
        delayTimer.Reset();
        movable = false;

        if (moveBack)
            originalPosition = movableBoss.Position;

        if (animator != null) animator.SetTrigger("Prepare");
    }
    public override bool Deactivate() {

        targetWeapon.SetActive(false);
        movable = enabled = false;
        if (animator != null) animator.SetTrigger("End");
        if (moveBack) {
            return movableBoss.MoveTo(originalPosition);
        }

        return true;
    }

// #if UNITY_EDITOR
//     [CustomEditor(typeof(ActivateWeapon))]
//     public class _Editor : Editor {
//         SerializedProperty moveType;

//         private void OnEnable() {
//             moveType = serializedObject.FindProperty("moveType");
//         }

//         static public void FloatMinMaxSlider(string name, SerializedProperty property1, SerializedProperty property2, float rangeMin, float rangeMax)
//         {
//             float min = property1.floatValue, max = property2.floatValue;
//             EditorGUILayout.LabelField(name);
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.PropertyField(property1, GUIContent.none, GUILayout.Width(40));

//             EditorGUI.BeginChangeCheck();
//             EditorGUILayout.MinMaxSlider(ref min, ref max, rangeMin, rangeMax);
//             if (EditorGUI.EndChangeCheck()) {
//                 property1.floatValue = min;
//                 property2.floatValue = max;
//                 return;
//             }

//             EditorGUILayout.PropertyField(property2, GUIContent.none, GUILayout.Width(40));
//             EditorGUILayout.EndHorizontal();
//         }

//         public override void OnInspectorGUI()
//         {
//             serializedObject.Update();
//             GUILayout.Space(8);
//             EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
//             EditorGUILayout.PropertyField(moveType);
//             EditorGUILayout.PropertyField(serializedObject.FindProperty("delayTime"));
//             EditorGUILayout.PropertyField(serializedObject.FindProperty("targetWeapon"));
//             EditorGUILayout.PropertyField(serializedObject.FindProperty("moveBackSpeed"));
//             FloatMinMaxSlider("X Moveble Range", serializedObject.FindProperty("moveMinX"), serializedObject.FindProperty("moveMaxX"), -10, 10);
//             FloatMinMaxSlider("Y Moveble Range", serializedObject.FindProperty("moveMinY"), serializedObject.FindProperty("moveMaxY"), -10, 10);
//             EditorGUILayout.PropertyField(serializedObject.FindProperty("leftVec"));
//             EditorGUILayout.PropertyField(serializedObject.FindProperty("rightVec"));
//             serializedObject.ApplyModifiedProperties();
//         }
//     }
// #endif
}
