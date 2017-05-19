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
    public HexDirection Facing;
    public HexCell Cell;
    [Tooltip("The hex grid this character exists upon")]
    public HexGrid HexGrid;
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
            oldState.Closing();
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

    public void Hurt(float damage, Action hurtComplete = null)
    {
        Model.Hurt(hurtComplete);
        Stats.TakeDamage(damage);
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
