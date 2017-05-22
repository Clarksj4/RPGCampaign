using System;
using System.Collections;
using UnityEngine;
using Pathfinding;
using TileMap;

public abstract class Ability : MonoBehaviour
{
    public event EventHandler AbilityComplete;

    public float Cost;
    public int MinimumRange;
    public int MaximumRange;
    public HexGridTraverser Traverser = HexGridTraverser.RangedAttack();

    protected Character user;
    protected ITile<Character> target;

    public virtual bool InRange(ITile<Character> origin, ITile<Character> target)
    {
        // Target is NOT within minimum range, and is within maximum range
        return !Pathfind.InRange(origin, target, MinimumRange - 1, Traverser) &&
                Pathfind.InRange(origin, target, MaximumRange, Traverser);
    }

    public virtual void Activate(Character user, ITile<Character> target)
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
