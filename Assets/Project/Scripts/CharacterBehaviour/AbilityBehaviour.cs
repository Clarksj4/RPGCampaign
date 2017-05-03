using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class AbilityBehaviour : CharacterBehaviour
{
    private HexCell target;

    public AbilityBehaviour(Character character, HexCell target, Ability ability)
        : base(character)
    {
        this.target = target;

        // Create attack
        ability.Use(character, target);

        character.transform.LookAt(target.Position);

        if (Animator != null)
            Animator.SetTrigger("Attack");

        StartCoroutine(DoIdleAfterDelay());
    }

    IEnumerator DoIdleAfterDelay()
    {
        yield return new WaitForSeconds(1);

        // TODO: casting animation
        SetState(new IdleBehaviour(character));
    }
}
