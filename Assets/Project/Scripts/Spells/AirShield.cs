using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using TileMap;

public class AirShield : Ability
{
    private HexDirection direction;

    public override void Activate(Character user, ITile<Character> target, Action abilityComplete)
    {
        base.Activate(user, target, abilityComplete);

        transform.LookAt(target.Position);
    }
}
