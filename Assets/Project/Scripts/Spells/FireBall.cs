using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.PyroParticles;

public class FireBall : Ability
{
    public GameObject KneadingParticleEffect;
    public GameObject ProjectileParticleEffect;
    public GameObject DetonationParticleEffect;
    public Vector3 DetonationPosition;
    public float Speed = 10;
    public float Damage = 1;

    private GameObject kneadingInstance;
    private GameObject projectileInstance;

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
        kneadingInstance = Instantiate(KneadingParticleEffect, user.CastPosition, Quaternion.identity);
        StartCoroutine(CastInHands());
    }

    private void AnimEvents_CastKneadingComplete(object sender, EventArgs e)
    {
        // Kill kneading particle effect
        Destroy(kneadingInstance);
    }

    private void AnimEvents_CastApex(object sender, EventArgs e)
    {
        // Create projectile, move towards target
        projectileInstance = Instantiate(ProjectileParticleEffect, user.CastPosition, Quaternion.identity);
        projectileInstance.transform.LookAt(target.Occupant.transform.position + DetonationPosition);

        // Deactivate upon reaching target!
    }

    IEnumerator CastInHands()
    {
        while (kneadingInstance != null)
        {
            kneadingInstance.transform.position = user.CastPosition;
            yield return null;
        }
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
