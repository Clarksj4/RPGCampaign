using System;
using UnityEngine;
using TileMap;

public class AbilityBehaviour : CharacterBehaviour
{
    private Ability instance;
    private ITile<Character> target;

    public AbilityBehaviour(Character character, ITile<Character> target, Ability abilityPrefab)
        : base(character)
    {
        this.target = target;

        // Subtract TU
        character.Stats.CurrentTimeUnits -= abilityPrefab.Cost;

        // LookAt
        character.TurnTowards(target);

        // Use
        instance = GameObject.Instantiate(abilityPrefab, character.Tile.Position, abilityPrefab.transform.rotation) as Ability;
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
