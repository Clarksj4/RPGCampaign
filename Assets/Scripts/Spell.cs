using UnityEngine;
using HexMapPathfinding;

public abstract class Spell : MonoBehaviour
{
    public float Cost;
    public int MinimumRange = 1;
    public int MaximumRange = 3;
    public DefaultTraverser Traverser = DefaultTraverser.RangedAttack();

    [HideInInspector]
    public Character caster;
    [HideInInspector]
    public HexCell target;

    public virtual bool InRange(HexCell origin, HexCell target)
    {
        // Target is NOT within minimum range, and is within maximum range
        return !Pathfind.InRange(origin, target, MinimumRange, Traverser) &&
                Pathfind.InRange(origin, target, MaximumRange, Traverser);
    }

    public abstract void Cast(Character caster, HexCell target);
}
