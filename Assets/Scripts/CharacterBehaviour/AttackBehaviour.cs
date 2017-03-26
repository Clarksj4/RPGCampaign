using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

public class AttackBehaviour : CharacterBehaviour
{
    private Character target;

    public AttackBehaviour(Character character, Character target, Attack attack)
        : base(character)
    {
        this.target = target;

        Debug.Log("ATTACK!");
        StartCoroutine(DoIdleAfterDelay());
    }

    IEnumerator DoIdleAfterDelay()
    {
        yield return new WaitForSeconds(1);

        // TODO: 
        SetState(new IdleBehaviour(character));
    }
}
