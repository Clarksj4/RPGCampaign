using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.PyroParticles;

public class FireBall : Ability
{
    public GameObject KneadingParticleEffect;
    public FireBallParticle ProjectileParticleEffect;
    public float Speed = 10;
    public float Damage = 1;

    private GameObject kneadingInstance;
    private FireBallParticle projectileInstance;

    private bool castComplete;
    private bool moveComplete;

    public override void Activate(Character user, HexCell target)
    {
        base.Activate(user, target);

        // Tell model to do casting animation
        user.Model.KneadingCast(KneadingBegun, KneadingComplete, CastApex, CastComplete);
    }

    private void KneadingBegun()
    {
        // Create Kneading particle effect and make it go!
        kneadingInstance = Instantiate(KneadingParticleEffect, user.Model.CastSpawn.position, Quaternion.identity, user.Model.CastSpawn);
    }

    private void KneadingComplete()
    {
        // Kill kneading particle effect
        Destroy(kneadingInstance);
    }

    private void CastApex()
    {
        // Create projectile, move towards target
        projectileInstance = Instantiate(ProjectileParticleEffect, user.Model.CastSpawn.position, Quaternion.identity);
        projectileInstance.MoveToDetonate(target.Occupant.Model.Torso, Speed, OnMoveComplete);
    }

    private void CastComplete()
    {
        castComplete = true;
        CheckExpired();
    }

    private void OnMoveComplete()
    {
        target.Occupant.Hurt(Damage);

        moveComplete = true;
        CheckExpired();
    }

    private void CheckExpired()
    {
        // Don't destroy until cast animation is complete AND fireball has reached destination
        if (castComplete && moveComplete)
        {
            castComplete = false;
            moveComplete = false;

            Deactivate();
        }
    }
}
