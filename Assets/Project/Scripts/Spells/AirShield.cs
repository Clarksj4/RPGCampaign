using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirShield : Ability
{
    private HexDirection direction;

    public override void Use(Character user, HexCell target)
    {
        Ability instance = Instantiate(this, user.Cell.Position, Quaternion.identity) as Ability;
        instance.user = user;
        instance.target = target;
        instance.transform.LookAt(target.Position);
    }
}
