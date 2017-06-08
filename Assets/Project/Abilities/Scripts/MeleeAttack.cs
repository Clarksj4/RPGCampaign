using System;
using Pathfinding;
using TileMap;

public class MeleeAttack : Ability
{
    public override void Activate(Character user, ITile<Character> target, Action abilityComplete)
    {
        base.Activate(user, target, abilityComplete);

        // Tell user to do attack animation
        user.Model.MeleeAttack(AttackApex, null);
    }

    private void AttackApex()
    {
        // Deal damage to target
        target.Contents.Hurt(Damage, HurtComplete);
    }

    private void HurtComplete()
    {
        Deactivate();
    }
}
