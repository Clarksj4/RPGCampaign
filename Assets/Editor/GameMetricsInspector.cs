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

            EditorGUILayout.LabelField("Strengths");
            EditorGUI.indentLevel = 1;
            element.Strengths[(int)ElementType.Air] = EditorGUILayout.FloatField("Air", element.Strengths[(int)ElementType.Air]);
            element.Strengths[(int)ElementType.Earth] = EditorGUILayout.FloatField("Earth", element.Strengths[(int)ElementType.Earth]);
            element.Strengths[(int)ElementType.Fire] = EditorGUILayout.FloatField("Fire", element.Strengths[(int)ElementType.Fire]);
            element.Strengths[(int)ElementType.Water] = EditorGUILayout.FloatField("Water", element.Strengths[(int)ElementType.Water]);
            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("Default meter levels");
            EditorGUI.indentLevel = 1;
            element.DefaultMeterLevel[(int)ElementType.Air] = EditorGUILayout.FloatField("Air", element.DefaultMeterLevel[(int)ElementType.Air]);
            element.DefaultMeterLevel[(int)ElementType.Earth] = EditorGUILayout.FloatField("Earth", element.DefaultMeterLevel[(int)ElementType.Earth]);
            element.DefaultMeterLevel[(int)ElementType.Fire] = EditorGUILayout.FloatField("Fire", element.DefaultMeterLevel[(int)ElementType.Fire]);
            element.DefaultMeterLevel[(int)ElementType.Water] = EditorGUILayout.FloatField("Water", element.DefaultMeterLevel[(int)ElementType.Water]);
            EditorGUI.indentLevel = 0;
        }
    }
}
