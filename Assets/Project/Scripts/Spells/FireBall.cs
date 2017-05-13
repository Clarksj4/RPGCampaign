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
        user.AnimEvents.CastKneadingBegun -= AnimEvents_CastKneadingBegun;

        // Create Kneading particle effect and make it go!
        kneadingInstance = Instantiate(KneadingParticleEffect, user.CastPosition, Quaternion.identity);
        StartCoroutine(CastInHands());
    }

    private void AnimEvents_CastKneadingComplete(object sender, EventArgs e)
    {
        user.AnimEvents.CastKneadingComplete -= AnimEvents_CastKneadingComplete;

        // Kill kneading particle effect
        Destroy(kneadingInstance);
    }

    private void AnimEvents_CastApex(object sender, EventArgs e)
    {
        user.AnimEvents.CastApex -= AnimEvents_CastApex;

        // Create projectile, move towards target
        projectileInstance = Instantiate(ProjectileParticleEffect, user.CastPosition, Quaternion.identity);
        projectileInstance.MoveToDetonate(target.Occupant.Torso, Speed, () => OnMoveComplete(), () => OnExplosionComplete());
    }

    private void OnMoveComplete()
    {
        target.Occupant.TakeDamage(Damage);
    }

    private void OnExplosionComplete()
    {
        Deactivate();
    }

    IEnumerator CastInHands()
    {
        while (kneadingInstance != null)
        {
            kneadingInstance.transform.position = user.CastPosition;
            yield return null;
        }
    }
}
