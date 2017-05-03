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

        // Subtract TU
        character.Stats.CurrentTimeUnits -= ability.Cost;

        // LookAt
        character.TurnTowards(target);

        // Use
        ability.Use(character, target);

        // TODO: [PLACEHOLDER] listen for end of animation
        StartCoroutine(DoIdleAfterDelay());
    }

    IEnumerator DoIdleAfterDelay()
    {
        yield return new WaitForSeconds(1);

        // TODO: casting animation
        SetState(new IdleBehaviour(character));
    }
}
