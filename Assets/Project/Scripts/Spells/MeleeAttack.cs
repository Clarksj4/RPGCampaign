using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Ability
{
    public float Damage;

    public override void Use(Character user, HexCell target)
    {
        user.Attack(target.Occupant, Damage);
    }
}
