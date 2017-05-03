using System;
using UnityEngine;

public class AttackBehaviour : CharacterBehaviour
{
    private Character target;
    private AnimationEvents animEvents;
    private float damage;

    public AttackBehaviour(Character character, Character target, float damage) 
        : base(character)
    {
        this.target = target;
        this.damage = damage;

        Animator.SetTrigger("Attack");

        animEvents = character.GetComponentInChildren<AnimationEvents>();
        animEvents.AttackApex += AnimEvents_AttackApex;

        character.LookAt(target);
    }

    private void AnimEvents_AttackApex(object sender, EventArgs e)
    {
        target.TakeDamage(damage);

        SetState(new IdleBehaviour(character));
    }
}
