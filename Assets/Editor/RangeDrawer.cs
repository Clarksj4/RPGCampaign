using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Range))]
public class RangeDrawer : PropertyDrawer
{

    // Air              Min [_____] Max [_____]
    //  Current -------o----------------- [___]

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string name = property.displayName;

        SerializedProperty minimum = property.FindPropertyRelative("min");
        SerializedProperty current = property.FindPropertyRelative("current");
        SerializedProperty maximum = property.FindPropertyRelative("max");

        EditorGUI.BeginProperty(new Rect(position.x, position.y, position.width, position.height), label, property);
        {
            // Label
            Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(name));
            EditorGUIUtility.labelWidth = 28f;

            // Capacity
            Rect minRect = new Rect(contentPosition.x, contentPosition.y + 1, contentPosition.width / 2, contentPosition.height - 2);
            if (Range.Size(minimum.floatValue, maximum.floatValue) > 0) minRect.height /= 2;

            Rect maxRect = new Rect(contentPosition.x + (contentPosition.width / 2), contentPosition.y + 1, contentPosition.width / 2, contentPosition.height - 2);
            if (Range.Size(minimum.floatValue, maximum.floatValue) > 0) maxRect.height /= 2;


            minimum.floatValue = EditorGUI.FloatField(minRect, new GUIContent(minimum.displayName), minimum.floatValue);
            maximum.floatValue = EditorGUI.FloatField(maxRect, new GUIContent(maximum.displayName), maximum.floatValue);

            // Current
            if (Range.Size(minimum.floatValue, maximum.floatValue) > 0)
            {
                EditorGUIUtility.labelWidth = 52f;
                Rect currentRect = new Rect(position.x + 10, contentPosition.y + (contentPosition.height / 2) + 2, position.width - 10, (contentPosition.height / 2) - 2);
                current.floatValue = EditorGUI.Slider(currentRect, new GUIContent(current.displayName), current.floatValue, minimum.floatValue, maximum.floatValue);
            }

            if (maximum.floatValue < minimum.floatValue)
                maximum.floatValue = minimum.floatValue;

            if (minimum.floatValue > maximum.floatValue)
                minimum.floatValue = maximum.floatValue;

        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty minimum = property.FindPropertyRelative("min");
        SerializedProperty maximum = property.FindPropertyRelative("max");

        if (Range.Size(minimum.floatValue, maximum.floatValue) > 0)
            return 36f;

        return 18f;
    }
}
