using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameMetrics))]
public class GameMetricsInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameMetrics metrics = (GameMetrics)target;

        for (int i = 0; i < metrics.Elements.Length; i++)
        {
            Element element = metrics.Elements[i];

            EditorGUILayout.LabelField(element.Name, EditorStyles.boldLabel);
            element.Colour = EditorGUILayout.ColorField("Colour", element.Colour);

            EditorGUILayout.LabelField("Strength Modifiers");
            EditorGUI.indentLevel = 1;
            element.StrengthModifiers[(int)ElementType.Air] = EditorGUILayout.FloatField("Air", element.StrengthModifiers[(int)ElementType.Air]);
            element.StrengthModifiers[(int)ElementType.Earth] = EditorGUILayout.FloatField("Earth", element.StrengthModifiers[(int)ElementType.Earth]);
            element.StrengthModifiers[(int)ElementType.Fire] = EditorGUILayout.FloatField("Fire", element.StrengthModifiers[(int)ElementType.Fire]);
            element.StrengthModifiers[(int)ElementType.Water] = EditorGUILayout.FloatField("Water", element.StrengthModifiers[(int)ElementType.Water]);
            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("Element Capacity Modifiers");
            EditorGUI.indentLevel = 1;
            element.ElementCapacityModifiers[(int)ElementType.Air] = EditorGUILayout.FloatField("Air", element.ElementCapacityModifiers[(int)ElementType.Air]);
            element.ElementCapacityModifiers[(int)ElementType.Earth] = EditorGUILayout.FloatField("Earth", element.ElementCapacityModifiers[(int)ElementType.Earth]);
            element.ElementCapacityModifiers[(int)ElementType.Fire] = EditorGUILayout.FloatField("Fire", element.ElementCapacityModifiers[(int)ElementType.Fire]);
            element.ElementCapacityModifiers[(int)ElementType.Water] = EditorGUILayout.FloatField("Water", element.ElementCapacityModifiers[(int)ElementType.Water]);
            EditorGUI.indentLevel = 0;
        }
    }
}
