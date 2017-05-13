using System;
using System.Collections;
using UnityEngine;
using Pathfinding;

public abstract class Ability : MonoBehaviour
{
    public event EventHandler AbilityComplete;

    public float Cost;
    public int MinimumRange;
    public int MaximumRange;
    public HexMapTraverser Traverser = HexMapTraverser.RangedAttack();

    protected Character user;
    protected HexCell target;

    public virtual bool InRange(HexCell origin, HexCell target)
    {
        // Target is NOT within minimum range, and is within maximum range
        return !Pathfind.InRange(origin, target, MinimumRange - 1, Traverser) &&
                Pathfind.InRange(origin, target, MaximumRange, Traverser);
    }

    public virtual void Activate(Character user, HexCell target)
    {
        this.user = user;
        this.target = target;
    }

    protected virtual void Deactivate()
    {
        if (AbilityComplete != null)
            AbilityComplete(this, new EventArgs());
    }
}
