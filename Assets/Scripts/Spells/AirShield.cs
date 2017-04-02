using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirShield : Spell
{
    private HexDirection direction;

    public override bool Cast(HexCell origin, HexCell target)
    {
        GameObject instance = Instantiate(gameObject, origin.Position, Quaternion.identity) as GameObject;

        Spell spell = instance.GetComponent<Spell>();
        spell.origin = origin;
        spell.target = target;

        spell.transform.LookAt(target.Position);
        return true;
    }
}
