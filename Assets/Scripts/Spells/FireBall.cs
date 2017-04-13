using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Spell
{
    public Vector3 DetonationPosition;
    public float Speed = 10;
    public float Damage = 1;

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
        Vector3 destination = target.Position + DetonationPosition;

        Vector3 movementSpeed = Vector3.MoveTowards(transform.position, destination, Speed * Time.deltaTime);
        transform.position = movementSpeed;

        // TODO: Destroy upon arriving at destination
        if (transform.position == destination)
        {
            target.Occupant.Stats.TakeDamage(Damage);

            Destroy(gameObject);
        }
            
    }
}
