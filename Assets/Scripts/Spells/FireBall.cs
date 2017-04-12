using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Spell
{
    public Vector3 DetonationPosition;
    public float speed = 10;

    public override void Cast(Character caster, HexCell target)
    {
        GameObject instance = Instantiate(gameObject, caster.transform.position + caster.CastPosition, Quaternion.identity) as GameObject;

        Spell spell = instance.GetComponent<Spell>();
        spell.caster = caster;
        spell.target = target;

        spell.transform.LookAt(target.Position);
    }

    void Update ()
    {
        Vector3 movementSpeed = Vector3.MoveTowards(transform.position, target.Position + DetonationPosition, speed * Time.deltaTime);
        transform.position = movementSpeed;

        // TODO: Destroy upon arriving at destination
    }
}
