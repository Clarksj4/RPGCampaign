using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using TileMap;

public class FireBall : Ability
{
    public GameObject KneadingParticleEffect;
    public FireBallParticle ProjectileParticleEffect;
    public float Speed = 10;

    private GameObject kneadingInstance;
    private FireBallParticle projectileInstance;

    public override void Activate(Character user, ITile<Character> target)
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
        projectileInstance.MoveToDetonate(target.Contents.Model.Torso, Speed, OnMoveComplete);
    }

    private void CastComplete()
    {
        
    }

    private void OnMoveComplete()
    {
        target.Contents.Hurt(Damage, OnHurtComplete);
    }

    private void OnHurtComplete()
    {
        Deactivate();
    }
}
