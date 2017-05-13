using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Pathfinding;
using TurnBased;

[RequireComponent(typeof(Stats))]
public class Character : MonoBehaviour, IPawn<float>
{
    public event CharacterMovementEventHandler BeginMovement;
    public event CharacterMovementEventHandler FinishedMovement;

    public HexDirection Facing;
    public HexCell Cell;
    [Tooltip("The hex grid this character exists upon")]
    public HexGrid HexGrid;
    [Tooltip("The abilities this character can use")]
    public Ability[] Abilities;
    public Transform Head;
    public Transform Torso;
    public Transform LeftHand;
    public Transform RightHand;

    private Animator animator;
    private Stats stats;
    private CharacterBehaviour state;
    private AnimationEvents animEvents;

    public Vector3 CastPosition { get; private set; }
    public AnimationEvents AnimEvents { get { return animEvents; } }
    public Animator Animator { get { return animator; } }
    public Stats Stats { get { return stats; } }
    public Player Controller { get; private set; }
    public float Priority { get { return Stats.Initiative; } }
    public bool IsUsingAbility { get { return state.GetType() == typeof(AbilityBehaviour); } }
    public bool IsMoving { get { return state.GetType() == typeof(MoveBehaviour); } }
    public bool IsIdle { get { return state.GetType() == typeof(IdleBehaviour); } }
    public bool IsTurn { get; private set; }

    private void Awake()
    {
        Controller = GetComponentInParent<Player>();
        stats = GetComponent<Stats>();

        animator = GetComponentInChildren<Animator>();
        animEvents = GetComponentInChildren<AnimationEvents>();
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
        if (LeftHand != null && RightHand != null)
            CastPosition = (LeftHand.position + RightHand.position) / 2;

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

    public void UseAbility(Ability ability, HexCell target)
    {
        SetState(new AbilityBehaviour(this, target, ability));
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

    public void TurnTowards(Character target)
    {
        TurnTowards(target.Cell);
    }

    public void TurnTowards(HexCell target)
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

        Controller.PawnStart(this);
    }

    public void TurnEnd()
    {
        IsTurn = false;

        state.EndTurn();
    }
}
