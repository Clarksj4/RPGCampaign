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

        // Listen for when the attack is complete
        user.AnimEvents.AttackComplete += AnimEvents_AttackComplete;
    }

    private void AnimEvents_AttackApex(object sender, EventArgs e)
    {
        target.Occupant.TakeDamage(Damage);
    }

    private void AnimEvents_AttackComplete(object sender, EventArgs e)
    {
        // Stop listening for when the attack is complete
        user.AnimEvents.AttackComplete -= AnimEvents_AttackComplete;

        Deactivate();
    }
}
