using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelSelectable : SelectableItem
{
    [SerializeField]
    private int levelId, requireUnlockLevel;

    private void Start() {
        Disabled = !LevelSelectMgr.LevelHasUnlock(requireUnlockLevel);
    }

    public override void Activate() {
        if (requireUnlockLevel < 0 || LevelSelectMgr.LevelHasUnlock(requireUnlockLevel))
            LevelSelectMgr.LoadLevel(levelId);
    }

    // #if UNITY_EDITOR
    // [CustomEditor(typeof(LevelSelectable))]
    // public class _Editor: Editor {
    //     private void OnEnable() {
    //         LevelSelectable selectable = (LevelSelectable) target;

    //         serializedObject.Update();

    //         SelectableItem[] selectables = selectable.GetComponents<SelectableItem>();
    //         if (selectables.Length > 1) {
    //             for (int i = 0; i < selectables.Length; i++) {
    //                 if (!selectable.Equals(selectables[i])) {
    //                     if (EditorUtility.DisplayDialog("Find Selectable", "Find selectable repeat,\nReplace it?", "Yes", "No")) {
    //                         SerializedObject replaceSerObj = new SerializedObject(selectables[i]);
    //                         serializedObject.FindProperty("style").objectReferenceValue = replaceSerObj.FindProperty("style").objectReferenceValue;
    //                         serializedObject.FindProperty("leftNavigate").objectReferenceValue = replaceSerObj.FindProperty("leftNavigate").objectReferenceValue;
    //                         serializedObject.FindProperty("rightNavigate").objectReferenceValue = replaceSerObj.FindProperty("rightNavigate").objectReferenceValue;
    //                         serializedObject.FindProperty("topNavigate").objectReferenceValue = replaceSerObj.FindProperty("topNavigate").objectReferenceValue;
    //                         serializedObject.FindProperty("bottomNavigate").objectReferenceValue = replaceSerObj.FindProperty("bottomNavigate").objectReferenceValue;
    //                         DestroyImmediate(selectables[i]);
    //                         break;
    //                     }
    //                 }
    //             }
    //         }

    //         serializedObject.ApplyModifiedProperties();
    //     }
    // }
    // #endif
}
