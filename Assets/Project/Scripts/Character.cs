using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Pathfinding;
using TurnBased;

[RequireComponent(typeof(Stats))]
public class Character : MonoBehaviour, IPawn
{
    public event CharacterMovementEventHandler BeginMovement;
    public event CharacterMovementEventHandler FinishedMovement;

    public HexDirection Facing;
    public HexCell Cell;
    [Tooltip("The hex grid this character exists upon")]
    public HexGrid HexGrid;
    [Tooltip("The abilities this character can use")]
    public Ability[] Abilities;
    [Tooltip("The local position at which spells will spawn")]
    public Vector3 CastPosition;

    private Animator animator;
    private Stats stats;
    private CharacterBehaviour state;

    public Animator Animator { get { return animator; } }
    public Stats Stats { get { return stats; } }
    public IPawnController Controller { get; private set; }
    public bool IsUsingAbility { get { return state.GetType() == typeof(AbilityBehaviour); } }
    public bool IsMoving { get { return state.GetType() == typeof(MoveBehaviour); } }
    public bool IsIdle { get { return state.GetType() == typeof(IdleBehaviour); } }
    public bool IsTurn { get; private set; }

    private void Awake()
    {
        Controller = GetComponentInParent<IPawnController>();
        stats = GetComponent<Stats>();

        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (HexGrid != null)
        {
            Cell = HexGrid.GetCell(transform.position);
            Cell.Occupant = this;
            transform.position = Cell.Position;
            transform.LookAt(Facing);
        }

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
        {
            oldState.Closing();

            if (oldState is MoveBehaviour && FinishedMovement != null)
                FinishedMovement(this, new CharacterMovementEventArgs(null));
        }

        if (state is MoveBehaviour && BeginMovement != null)
            BeginMovement(this, new CharacterMovementEventArgs(null));
    }

    public void Attack(Character target, float damage)
    {
        SetState(new AttackBehaviour(this, target, damage));
    }

    public void UseAbility(Ability ability, HexCell target)
    {
        ability.Use(this, target);
    }

    public void Cast(Character target, float damage)
    {

    }

    /// <summary>
    /// Moves the character along the given path regardless of its time units. Returns true if the character moves atleast one cell along
    /// the path
    /// </summary>
    public void Move(Path path)
    {
        SetState(new MoveBehaviour(this, path));
    }

    public void TakeDamage(float damage)
    {
        Stats.TakeDamage(damage);
        animator.SetTrigger("Hurt");
    }

    public void LookAt(Character target)
    {
        LookAt(target.Cell);
    }

    public void LookAt(HexCell target)
    {
        Vector3 lookPosition = target.transform.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);
    }

    public void TurnStart()
    {
        IsTurn = true;

        state.BeginTurn();

        // TODO: Stats.BeginTurn()
        Stats.RefreshTimeUnits();
    }

    public void TurnEnd()
    {
        IsTurn = false;

        state.EndTurn();
    }

    public int CompareTo(IPawn other)
    {
        Character otherCharacter = (Character)other;
        return Stats.Initiative.CompareTo(otherCharacter.Stats.Initiative);
    }
}
