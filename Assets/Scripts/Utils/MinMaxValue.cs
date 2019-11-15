using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class MinMaxValue
{
    public float min;
    public float max;

    public MinMaxValue() : this(0, 1) { }
    public MinMaxValue(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MinMaxValue))]
class MinMaxSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        float x = position.x;
        EditorGUI.LabelField(new Rect(x, position.y, GUI.skin.box.CalcSize(label).x, position.height), label);
        x += GUI.skin.box.CalcSize(label).x + 10;

        float min = property.FindPropertyRelative("min").floatValue;
        float max = property.FindPropertyRelative("max").floatValue;

        EditorGUI.BeginChangeCheck();

        min = EditorGUI.FloatField(new Rect(x, position.y, 30, position.height), min);
        x += 40;
        EditorGUI.MinMaxSlider(new Rect(x, position.y, 90, position.height), ref min, ref max, 0, 100);
        x += 100;
        max = EditorGUI.FloatField(new Rect(x, position.y, 30, position.height), max);
        x += 30;

        if (EditorGUI.EndChangeCheck()) {
            property.FindPropertyRelative("min").floatValue = min;
            property.FindPropertyRelative("max").floatValue = max;
        }
    }
}
#endif