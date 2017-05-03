using System;

public class MeleeAttack : Ability
{
    public float Damage;

    protected override void Activate()
    {
        // Tell user to do attack animation
        user.Animator.SetTrigger("Attack");

        // Listen for when attack connects
        user.AnimEvents.AttackApex += AnimEvents_AttackApex;
    }

    private void AnimEvents_AttackApex(object sender, EventArgs e)
    {
        target.Occupant.TakeDamage(Damage);
        Destroy(gameObject);
    }
}
