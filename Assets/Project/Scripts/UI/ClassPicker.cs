//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ClassPicker : MonoBehaviour
//{
//    public Character character;

//    public void PickClass(string elementType)
//    {
//        ElementType type = (ElementType)Enum.Parse(typeof(ElementType), elementType);

//        Element element = GameMetrics.Instance[type];

//        character.Stats.Element = type;
//        character.Stats.SetElementCapacity(ElementType.Air, GameMetrics.Instance.ElementCapacity * element.ElementCapacityModifiers[(int)ElementType.Air]);
//        character.Stats.SetElementCapacity(ElementType.Earth, GameMetrics.Instance.ElementCapacity * element.ElementCapacityModifiers[(int)ElementType.Earth]);
//        character.Stats.SetElementCapacity(ElementType.Fire, GameMetrics.Instance.ElementCapacity * element.ElementCapacityModifiers[(int)ElementType.Fire]);
//        character.Stats.SetElementCapacity(ElementType.Water, GameMetrics.Instance.ElementCapacity * element.ElementCapacityModifiers[(int)ElementType.Water]);

//        gameObject.SetActive(false);
//    }
//}
