using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public string Name;
    public float Cost;
    public int MinimumRange = 1;
    public int MaximumRange = 3;
    public Traverser Traverser = Traverser.RangedAttack();

    public bool InRange(HexCell origin, HexCell target)
    {
        // Target is NOT within minimum range, and is within maximum range
        return !Pathfind.IsInRange(origin, target, MinimumRange, Traverser) &&
                Pathfind.IsInRange(origin, target, MaximumRange, Traverser);
    }

    //public void DoSpell()
    //{
    //    GameObject fireball = Instantiate(Resources.Load("Prefabs/Name")) as GameObject;
    //    fireball.GetComponent<CommonInterface>().Do();
    //}
}
