using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirShield : Spell
{
    private HexDirection direction;

    public override void Cast(Character caster, HexCell target)
    {
        GameObject instance = Instantiate(gameObject, caster.Cell.Position, Quaternion.identity) as GameObject;

        Spell spell = instance.GetComponent<Spell>();
        spell.caster = caster;
        spell.target = target;

        spell.transform.LookAt(target.Position);
    }
}
