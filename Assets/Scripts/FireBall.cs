using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Spell
{
    public float speed = 3;

    public override bool Cast(HexCell origin, HexCell target)
    {
        if (!InRange(origin, target))
            return false;

        GameObject instance = Instantiate(gameObject, origin.Position, Quaternion.identity) as GameObject;

        Spell spell = instance.GetComponent<Spell>();
        spell.origin = origin;
        spell.target = target;

        spell.transform.LookAt(target.Position);
        return true;
    }

    private bool InRange(HexCell origin, HexCell target)
    {
        // Target is NOT within minimum range, and is within maximum range
        return !Pathfind.IsInRange(origin, target, MinimumRange, Traverser) &&
                Pathfind.IsInRange(origin, target, MaximumRange, Traverser);
    }

    void Update ()
    {
        Vector3 movementSpeed = Vector3.MoveTowards(transform.position, target.Position, speed * Time.deltaTime);
        transform.position = movementSpeed;
    }
}
