using System;
using Pathfinding;
using TileMap;

public class MeleeAttack : Ability
{
    public float Damage;

    public override void Activate(Character user, ITile<Character> target)
    {
        base.Activate(user, target);

        // Tell user to do attack animation
        user.Model.MeleeAttack(AttackApex, AttackComplete);
    }

    private void AttackApex()
    {
        // Deal damage to target
        target.Contents.Hurt(Damage);
    }

    private void AttackComplete()
    {
        Deactivate();
    }
}
