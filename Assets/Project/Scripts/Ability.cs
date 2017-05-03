using UnityEngine;
using Pathfinding;

public abstract class Ability : MonoBehaviour
{
    public string AbilityType;
    public float Cost;
    public int MinimumRange;
    public int MaximumRange;
    public HexMapTraverser Traverser = HexMapTraverser.RangedAttack();

    [HideInInspector]
    public Character user;
    [HideInInspector]
    public HexCell target;

    public virtual bool InRange(HexCell origin, HexCell target)
    {
        // Target is NOT within minimum range, and is within maximum range
        return !Pathfind.InRange(origin, target, MinimumRange, Traverser) &&
                Pathfind.InRange(origin, target, MaximumRange, Traverser);
    }

    public abstract void Use(Character user, HexCell target);
}
