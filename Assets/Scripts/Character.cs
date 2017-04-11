using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(Stats))]
public class Character : MonoBehaviour
{
    public event CharacterMovementEventHandler BeginMovement;
    public event CharacterMovementEventHandler FinishedMovement;
    public event EventHandler BeginAttack;
    public event EventHandler FinishedAttack;

    public HexDirection Facing;
    public HexCell Cell;
    [Tooltip("The hex grid this character exists upon")]
    public HexGrid HexGrid;
    [Tooltip("The player that controls this character")]
    public Player Controller;
    [Tooltip("The spells this character can cast")]
    public Spell[] Spells;

    private Animator animator;
    private Stats stats;
    private CharacterBehaviour state;
    private GameManager gameManger;

    //public Attack[] Attacks { get { return GetComponents<Attack>(); } }
    public Animator Animator { get { return animator; } }
    public Stats Stats { get { return stats; } }
    public bool IsAttacking { get { return state.GetType() == typeof(AttackBehaviour); } }
    public bool IsMoving { get { return state.GetType() == typeof(MoveBehaviour); } }
    public bool CanMove { get { return !IsMoving; } }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        stats = GetComponent<Stats>();
        gameManger = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
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

            else if (oldState is AttackBehaviour && FinishedAttack != null)
                FinishedAttack(this, new EventArgs());
        }

        if (state is MoveBehaviour && BeginMovement != null)
            BeginMovement(this, new CharacterMovementEventArgs(null));

        else if (state is AttackBehaviour && BeginAttack != null)
            BeginAttack(this, new EventArgs());
    }

    /// <summary>
    /// Activate this character so it can have its turn. Returns true if this character is able to act.
    /// </summary>
    public bool Activate()
    {
        state.Activate();

        Stats.RefreshTimeUnits();

        // Return true because nothing can prevent the character from acting
        return true;
    }

    public void Cast(Spell spell, HexCell target)
    {
        if (spell == null || target == null)
           throw new ArgumentException("Invalid spell and / or target");

        SetState(new CastBehaviour(this, target, spell));
    }

    /// <summary>
    /// Moves the character along the given path regardless of its time units. Returns true if the character moves atleast one cell along
    /// the path
    /// </summary>
    public bool Move(HexPath path)
    {
        if (path == null || path.Count < 2)
            return false;

        SetState(new MoveBehaviour(this, path));
        return true;
    }
}
