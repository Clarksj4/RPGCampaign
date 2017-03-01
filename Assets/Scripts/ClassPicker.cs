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
        character.SetElementCapacity(ElementType.Air, GameMetrics.Instance.ElementCapacity * element.ElementCapacityModifiers[(int)ElementType.Air]);
        character.SetElementCapacity(ElementType.Earth, GameMetrics.Instance.ElementCapacity * element.ElementCapacityModifiers[(int)ElementType.Earth]);
        character.SetElementCapacity(ElementType.Fire, GameMetrics.Instance.ElementCapacity * element.ElementCapacityModifiers[(int)ElementType.Fire]);
        character.SetElementCapacity(ElementType.Water, GameMetrics.Instance.ElementCapacity * element.ElementCapacityModifiers[(int)ElementType.Water]);

        gameObject.SetActive(false);
    }
}
