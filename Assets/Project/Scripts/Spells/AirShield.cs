using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using TileMap;

public class AirShield : Ability
{
    private HexDirection direction;

    public override void Activate(Character user, ITile<Character> target)
    {
        base.Activate(user, target);

        transform.LookAt(target.Position);
    }
}
