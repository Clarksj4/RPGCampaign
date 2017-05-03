using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirShield : Ability
{
    private HexDirection direction;

    public override void Activate(Character user, HexCell target)
    {
        base.Activate(user, target);

        transform.LookAt(target.Position);
    }
}
