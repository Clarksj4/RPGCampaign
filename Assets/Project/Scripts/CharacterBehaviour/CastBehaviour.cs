using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CastBehaviour : CharacterBehaviour
{
    private HexCell target;

    public CastBehaviour(Character character, HexCell target, Spell spell)
        : base(character)
    {
        this.target = target;

        // Create attack
        spell.Cast(character, target);

        StartCoroutine(DoIdleAfterDelay());
    }

    IEnumerator DoIdleAfterDelay()
    {
        yield return new WaitForSeconds(1);

        // TODO: casting animation
        SetState(new IdleBehaviour(character));
    }
}
