using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    public float Cost;
    public int MinimumRange = 1;
    public int MaximumRange = 3;
    public Traverser Traverser = Traverser.RangedAttack();

    [HideInInspector]
    public HexCell origin;
    [HideInInspector]
    public HexCell target;

    //private bool InRange(HexCell origin, HexCell target)
    //{
    //    // Target is NOT within minimum range, and is within maximum range
    //    return !Pathfind.IsInRange(origin, target, MinimumRange, Traverser) &&
    //            Pathfind.IsInRange(origin, target, MaximumRange, Traverser);
    //}

    public abstract bool Cast(HexCell origin, HexCell target);
}
