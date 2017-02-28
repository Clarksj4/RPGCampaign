using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassPicker : MonoBehaviour
{
    public Character character;

    public void PickClass(string elementType)
    {
        ElementType type = (ElementType)Enum.Parse(typeof(ElementType), elementType);

        Element element = GameMetrics.Instance[type];

        character.Element = type;
        character.Air.Max = GameMetrics.Instance.DefaultMeterSize * element.DefaultMeterLevel[(int)ElementType.Air];
        character.Earth.Max = GameMetrics.Instance.DefaultMeterSize * element.DefaultMeterLevel[(int)ElementType.Earth];
        character.Fire.Max = GameMetrics.Instance.DefaultMeterSize * element.DefaultMeterLevel[(int)ElementType.Fire];
        character.Water.Max = GameMetrics.Instance.DefaultMeterSize * element.DefaultMeterLevel[(int)ElementType.Water];
        character.Refresh();

        gameObject.SetActive(false);
    }
}
