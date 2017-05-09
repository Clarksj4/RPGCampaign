using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Ability
{
    public GameObject KneadingParticleEffect;
    public GameObject ProjectileParticleEffect;
    public GameObject DetonationParticleEffect;
    public Vector3 DetonationPosition;
    public float Speed = 10;
    public float Damage = 1;

    public override void Activate(Character user, HexCell target)
    {
        base.Activate(user, target);

        // Tell user to do attack animation
        user.Animator.SetTrigger("Cast");

        user.AnimEvents.CastKneadingBegun += AnimEvents_CastKneadingBegun;
        user.AnimEvents.CastKneadingComplete += AnimEvents_CastKneadingComplete;
        user.AnimEvents.CastApex += AnimEvents_CastApex;
    }

    private void AnimEvents_CastKneadingBegun(object sender, EventArgs e)
    {
        // Create Kneading particle effect and make it go!
    }

    private void AnimEvents_CastKneadingComplete(object sender, EventArgs e)
    {
        // Kill kneading particle effect
    }

    private void AnimEvents_CastApex(object sender, EventArgs e)
    {
        // Create projectile, move towards target

        // Deactivate upon reaching target!
    }

    IEnumerator ProjectileMoveTowards()
    {
        Vector3 destination = target.Position + DetonationPosition;
        while (transform.position != destination)
        {
            Vector3 movementSpeed = Vector3.MoveTowards(transform.position, destination, Speed * Time.deltaTime);
            transform.position = movementSpeed;

            yield return null;
        }

        target.Occupant.TakeDamage(Damage);

        yield return StartCoroutine(Detonate());
    }

    IEnumerator Detonate()
    {
        // Create detonation effect

        yield return null;

        Deactivate();
    }
}
