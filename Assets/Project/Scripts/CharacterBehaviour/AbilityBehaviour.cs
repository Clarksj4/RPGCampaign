using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class AbilityBehaviour : CharacterBehaviour
{
    private Ability instance;
    private HexCell target;

    public AbilityBehaviour(Character character, HexCell target, Ability abilityPrefab)
        : base(character)
    {
        this.target = target;

        // Subtract TU
        character.Stats.CurrentTimeUnits -= abilityPrefab.Cost;

        // LookAt
        character.TurnTowards(target);

        // Use
        instance = GameObject.Instantiate(abilityPrefab, character.Cell.Position, abilityPrefab.transform.rotation) as Ability;
        instance.Activate(character, target);

        // Listen for ability finished
        instance.AbilityComplete += Ability_AbilityComplete;
    }

    private void Ability_AbilityComplete(object sender, EventArgs e)
    {
        GameObject.Destroy(instance.gameObject);

        SetState(new IdleBehaviour(character));
    }
}
