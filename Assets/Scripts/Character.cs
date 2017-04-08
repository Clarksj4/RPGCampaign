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

    private Animator animator;
    private Stats stats;
    private CharacterBehaviour state;
    private GameManager gameManger;

    public Attack[] Attacks { get { return GetComponents<Attack>(); } }
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

        Cast("AirShield");
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

    public float TraverseCost(HexDirection direction)
    {
        return Stats.Traverser.Cost(Cell, direction);
    }

    // Push character in given direction the given number of cells
    // Return the number of cells pushed, or -1 if not pushed (due to terrain etc)
    public int Push(HexDirection direction, int distance)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Finds a path to the given cell if one exists within this character's current time units, moves the character to the destination.
    /// Returns true if a path was found within current time units
    /// </summary>
    public bool MoveTo(HexCell destination)
    {
        // If character is already moving, or is already at the destination
        if (!CanMove || destination == Cell)
            return false;

        // Find a path
        HexPath path = Pathfind.QuickestPath(Cell, destination, Stats.CurrentTimeUnits, Stats.Traverser);
        if (path == null)   // No path to destination within current time units
            return false;

        // Move to destination
        SetState(new MoveBehaviour(this, path));
        return true;
    }

    /// <summary>
    /// Fins a path to the given cell and moves as far along the path as possible with this character's current time units. Returns true if
    /// a path to the destination was found and the character was able to move atleast one cell.
    /// </summary>
    public bool MoveTowards(HexCell destination)
    {
        // If character is already moving, or is already at the destination
        if (!CanMove || destination == Cell)
            return false;

        // Find a path
        HexPath path = Pathfind.QuickestPath(Cell, destination, Stats.Traverser);
        if (path == null)   // No path to destination within current time units
            return false;

        // Get the portion of the path that the character can move along with its current time units
        HexPath inRangePath = path.To(Stats.CurrentTimeUnits);
        if (path.Count < 2)
            return false;

        // Move along in range portion of path
        SetState(new MoveBehaviour(this, inRangePath));
        return true;
    }

    public bool MoveToAttackRange(Character target, Attack attack)
    {
        if (!CanMove)                           // If unable to move...
            return false;

        // Get quickest path to a cell within range of target
        HexPath path = Pathfind.ToWithinRange(Cell, target.Cell, attack.Range, Stats.Traverser, attack.Traverser);
        if (path == null || path.Count < 2)     // If there is no path or already at destination...
            return false;

        // Get the portion of path that this character can afford with its current time units
        HexPath affordablePath = path.To(Stats.CurrentTimeUnits);
        if (affordablePath.Count < 2)           // If can't afford to move...
            return false;

        // Move along affordable portion of path
        SetState(new MoveBehaviour(this, affordablePath));
        return true;
    }

    public void Cast(string spell)
    {
        gameManger.SpellBook[spell].Cast(Cell, Cell.GetNeighbor(HexDirection.NE));
    }

    public bool Attack(Character target, Attack attack)
    {
        // If not in range, can't attack
        if (!attack.InRange(target.Cell))
            return false;

        // Attack target with given attack
        SetState(new AttackBehaviour(this, target, attack));
        return true;
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
