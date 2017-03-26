using System;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public string Name;
    public int Range = 3;
    public float Cost = 0;
    public Traverser Traverser = Traverser.RangedAttack();

    public bool InRange(HexCell target)
    {
        return Pathfind.IsInRange(GetComponent<Character>().Cell, target, Range, Traverser);
    }
}
