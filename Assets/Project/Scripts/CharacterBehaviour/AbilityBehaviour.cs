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
        character.Stats.SpendTimeUnits(abilityPrefab.Cost);

        // LookAt
        character.TurnTowards(target);

        // Use
        instance = GameObject.Instantiate(abilityPrefab, character.Tile.Position, abilityPrefab.transform.rotation) as Ability;
        instance.Activate(character, target, AbilityComplete);
    }

    private void AbilityComplete()
    {
        GameObject.Destroy(instance.gameObject);

        SetState(new IdleBehaviour(character));
    }
}
