using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Pathfinding;
using TurnBased;
using TileMap;

[RequireComponent(typeof(Stats))]
public class Character : MonoBehaviour
{
    public HexDirection Facing;
    public ITile<Character> Tile;
    [Tooltip("The abilities this character can use")]
    public Ability[] Abilities;

    private CharacterBehaviour state;

    public Stats Stats { get; private set; }
    public Model Model { get; private set; }
    public Player Controller { get; private set; }
    public float Priority { get { return Stats.Initiative; } }
    public bool IsUsingAbility { get { return state.GetType() == typeof(AbilityBehaviour); } }
    public bool IsMoving { get { return state.GetType() == typeof(MoveBehaviour); } }
    public bool IsIdle { get { return state.GetType() == typeof(IdleBehaviour); } }
    public bool IsTurn { get; private set; }

    private void Awake()
    {
        Controller = GetComponentInParent<Player>();
        Stats = GetComponent<Stats>();
        Model = GetComponentInChildren<Model>();
    }

    private void Start()
    {
        SetState(new IdleBehaviour(this));
    }

    private void Update()
    {
        state.Update();
    }

    public void SetState(CharacterBehaviour newState)
    {
        CharacterBehaviour oldState = state;

        state = newState;
        newState.Init();

        if (oldState != null)
            oldState.Closing();
    }

    public void UseAbility(Ability ability, ITile<Character> target)
    {
        SetState(new AbilityBehaviour(this, target, ability));
    }

    public void Move(Path path)
    {
        SetState(new MoveBehaviour(this, path));
    }

    /// <summary>
    /// Deals damage to this character. The character will play a hurt or death animation depending on
    /// whether they survive the damage. The callback is executed at the conclusion of the animation,
    /// </summary>
    /// <param name="damage">The amount of damage to deal this character</param>
    /// <param name="hurtComplete">The callback to be raised when the animation is complete</param>
    public void Hurt(float damage, Action hurtComplete = null)
    {
        // Reduce HP, check whether the character will survive the attack
        bool alive = Stats.ReceiveDamage(damage);

        // If the character will survive, play the hurt animation
        if (alive)
            Model.Hurt(() => HurtComplete(false, hurtComplete));

        // Otherwise, play the character's death animation
        else
            Model.Die(() => HurtComplete(true, hurtComplete));
    }

    public void TurnTowards(ITile<Character> target)
    {
        Vector3 lookPosition = target.Position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);
    }

    /// <summary>
    /// Notifies this character that it is its turn to act.
    /// </summary>
    public void TurnStart()
    {
        // Its your turn
        IsTurn = true;

        // Refill time units so character is able to perform actions
        Stats.RefillTimeUnits();

        // Tell controller this character is able to act
        Controller.PawnStart(this);
    }

    /// <summary>
    /// Notifies this character that it is no longer its turn to act
    /// </summary>
    public void TurnEnd()
    {
        // No longer this character's turn
        IsTurn = false;
    }

    private void HurtComplete(bool dead, Action hurtComplete = null)
    {
        hurtComplete();

        if (dead)
            Controller.PawnDie(this);
    }
}
