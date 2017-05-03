using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Ability
{
    public Vector3 DetonationPosition;
    public float Speed = 10;
    public float Damage = 1;

    protected override void Activate()
    {
        transform.LookAt(target.Position);
    }

    void Update ()
    {
        Vector3 destination = target.Position + DetonationPosition;

        Vector3 movementSpeed = Vector3.MoveTowards(transform.position, destination, Speed * Time.deltaTime);
        transform.position = movementSpeed;

        // TODO: Destroy upon arriving at destination
        if (transform.position == destination)
        {
            target.Occupant.TakeDamage(Damage);

            Destroy(gameObject);
        }
    }
}
