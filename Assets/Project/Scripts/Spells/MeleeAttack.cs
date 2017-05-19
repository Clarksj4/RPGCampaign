using System;

public class MeleeAttack : Ability
{
    public float Damage;

    public override void Activate(Character user, HexCell target)
    {
        base.Activate(user, target);

        // Tell user to do attack animation
        user.Model.MeleeAttack(AttackApex, AttackComplete);
    }

    private void AttackApex()
    {
        // Deal damage to target
        target.Occupant.Hurt(Damage);
    }

    private void AttackComplete()
    {
        Deactivate();
    }
}
