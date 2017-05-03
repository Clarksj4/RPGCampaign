using UnityEngine;
using Pathfinding;

public abstract class Ability : MonoBehaviour
{
    public float Cost;
    public int MinimumRange;
    public int MaximumRange;
    public HexMapTraverser Traverser = HexMapTraverser.RangedAttack();

    protected Character user;
    protected HexCell target;

    public virtual bool InRange(HexCell origin, HexCell target)
    {
        // Target is NOT within minimum range, and is within maximum range
        return !Pathfind.InRange(origin, target, MinimumRange, Traverser) &&
                Pathfind.InRange(origin, target, MaximumRange, Traverser);
    }

    public virtual void Use(Character user, HexCell target)
    {
        Ability instance = Instantiate(this, user.Cell.Position, transform.rotation) as Ability;
        instance.user = user;
        instance.target = target;
        instance.Activate();
    }

    protected abstract void Activate();
}
