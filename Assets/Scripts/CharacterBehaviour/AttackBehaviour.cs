using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

public class AttackBehaviour : CharacterBehaviour
{
    private Character target;

    public AttackBehaviour(Character character, Character target, Attack spell)
        : base(character)
    {
        this.target = target;

        Debug.Log("ATTACK!");
        StartCoroutine(DoIdleAfterDelay());

        // Create attack
        spell.Create(character.Cell, target.Cell);

        // Wait for attack to resolve
        // Go to idle state
    }

    IEnumerator DoIdleAfterDelay()
    {
        yield return new WaitForSeconds(1);

        // TODO: 
        SetState(new IdleBehaviour(character));
    }
}
