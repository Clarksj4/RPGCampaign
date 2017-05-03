using System;

public class MeleeAttack : Ability
{
    public float Damage;

    public override void Activate(Character user, HexCell target)
    {
        base.Activate(user, target);

        // Tell user to do attack animation
        user.Animator.SetTrigger("Attack");

        // Listen for when attack connects
        user.AnimEvents.AttackApex += AnimEvents_AttackApex;
    }

    private void AnimEvents_AttackApex(object sender, EventArgs e)
    {
        target.Occupant.TakeDamage(Damage);
        Deactivate();
    }
}
