using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Spell
{
    public float speed = 3;

    public override void Cast(HexCell origin, HexCell target)
    {
        GameObject instance = Instantiate(gameObject, origin.Position, Quaternion.identity) as GameObject;

        Spell spell = instance.GetComponent<Spell>();
        spell.origin = origin;
        spell.target = target;

        spell.transform.LookAt(target.Position);
    }

    void Update ()
    {
        Vector3 movementSpeed = Vector3.MoveTowards(transform.position, target.Position, speed * Time.deltaTime);
        transform.position = movementSpeed;
    }
}
