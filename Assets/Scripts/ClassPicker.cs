using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassPicker : MonoBehaviour
{
    GameObject characterPrefab;

    public void PickClass(ElementType type)
    {
        GameObject characterObj = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
        Character character = characterObj.GetComponent<Character>();

        character.Element = type;
    }
}
