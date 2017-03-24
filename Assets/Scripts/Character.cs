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

    public HexDirection Facing;
    public HexCell Cell;
    [Tooltip("The hex grid this character exists upon")]
    public HexGrid HexGrid;
    [Tooltip("The player that controls this character")]
    public Player Controller;
    public Attack Attack;

    private Animator animator;
    private Stats stats;
    private CharacterBehaviour state;

    public Animator Animator { get { return animator; } }
    public Stats Stats { get { return stats; } }
    public bool IsMoving { get { return state.GetType() == typeof(MoveBehaviour); } }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        stats = GetComponent<Stats>();
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
        if (state != null)
        {
            state.Closing();
            if (state is MoveBehaviour && FinishedMovement != null)
                FinishedMovement(this, new CharacterMovementEventArgs(null));
        }

        state = newState;
        newState.Init();

        if (state is MoveBehaviour && BeginMovement != null)
            BeginMovement(this, new CharacterMovementEventArgs(null));
    }

    /// <summary>
    /// Activate this character so it can have its turn. Returns true if this character is able to act.
    /// </summary>
    public bool Activate()
    {
        state.Activate();

        // Return true because nothing can prevent the character from acting
        return true;
    }

    public bool InAttackRange(HexCell target)
    {
        return Pathfind.IsInRange(Cell, target, Attack.range, Attack.traverser);
    }

    // Push character in given direction the given number of cells
    // Return the number of cells pushed, or -1 if not pushed (due to terrain etc)
    public int Move(HexDirection direction, int distance)
    {


        return -1;
    }

    // Find a path to the given cell and move along it
    public void MoveTo(HexCell cell)
    {
        // Pathfind.QuickestPath()
        // StartCoroutine(DoFollowPath())
    }

    // Find a path to the given cell and move as far along it as possible with current time units
    public void MoveTowards(HexCell cell)
    {

    }

    // Find a path to the closest adjacent cell to the given target and move as far along it as possible with 
    // current time units
    public void MoveTowards(Character target)
    {
        // Pathfind.ClosestPath()
        // StartCoroutine(DoFollowPath())
    }

    public void Move(HexPath path)
    {
        if (path != null && path.Count > 0)
            SetState(new MoveBehaviour(this, path));
    }
}
