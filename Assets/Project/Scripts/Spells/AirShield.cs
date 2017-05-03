using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirShield : Ability
{
    private HexDirection direction;

    protected override void Activate()
    {
        transform.LookAt(target.Position);
    }
}
