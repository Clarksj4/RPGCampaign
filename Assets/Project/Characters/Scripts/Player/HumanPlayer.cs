using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TurnBased;
using FluentBehaviourTree;
using TileMap;

using Pathfinding;
using System;

public class HumanPlayer : Player
{
    private IBehaviourTreeNode behaviourTree;
    private HexHighlighter highlighter;

    // Behaviour tree variables
    private ITile<Character> previousCell;
    private ITile<Character> targetCell;
    private ICollection<PathStep> movementRange;
    private Path movementPath;
    private Ability Ability { get { return Current.Abilities[abilityIndex]; } }
    private bool isCurrentCharactersTurn;
    private bool newTurn;
    private int abilityIndex = 0;
    private bool endTurn = false;

    public void SetAbility(int index)
    {
        abilityIndex = index;
    }

    public void EndTurn()
    {
        if (IsCurrentCharactersTurn())
            endTurn = true;
    }

    public override void PawnDie(Character pawn)
    {
        turnSystem.Paused = true;

        // Fade out
        pawn.Model.Fade(() => CleanUpPawn(pawn));
    }

    private void CleanUpPawn(Character pawn)
    {
        bool progressToNextTurn = Allies.Count > 1;

        turnSystem.Remove(pawn.GetComponent<TurnBasedEntity>(), progressToNextTurn);
        Destroy(pawn.gameObject);
    }

    protected override void Awake()
    {
        base.Awake();

        highlighter = GetComponent<HexHighlighter>();
        Current = Allies[0];

        // Input behaviour tree
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
        behaviourTree = builder
            .Parallel("Parallel", 2, 2)
                .Splice(EndTurnTree())
                .Sequence("Input")
                    .Splice(TargetCellTree())
                    .Parallel("Parallel", 2, 2)
                        .Splice(HighlightTree())
                        .Splice(ActTree())
                    .End()
                .End()
            .End()
        .Build();
    }

    private IBehaviourTreeNode EndTurnTree()
    {
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();

        IBehaviourTreeNode tree = builder
            .Sequence("End turn")
                .Condition("Is current character's turn?", t => IsCurrentCharactersTurn())
                .Condition("End turn button clicked?", t => endTurn)
                .Condition("Is current character idle?", t => IsCurrentCharacterIdle())
                .Do("End turn", t => DoEndTurn())
            .End()
        .Build();

        return tree;
    }

    private IBehaviourTreeNode TargetCellTree()
    {
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();

        IBehaviourTreeNode tree = builder
            .Sequence("Target Cell")
                .Do("Update target cell", t => UpdateTargetCell())
                .Inverter("[not]")
                    .Condition("Target cell is [not] null", t => IsTargetCellNull())
                .End()
                .Selector("Get path or area")
                    .Sequence("Area")
                        .Condition("Target cell occupied?", t => IsTargetCellOccupied())
                        .Condition("Occupant stationary?", t => IsTargetedCellOccupantStationary())
                        .Do("Get area", t => GetArea())
                    .End()
                    .Sequence("Path")
                        .Inverter("[not]")
                            .Condition("Target cell [not] occupied?", t => IsTargetCellOccupied())
                        .End()
                        .Condition("Is current character idle?", t => IsCurrentCharacterIdle())
                        .Do("Get path", t => GetPath())
                    .End()
                .End()
            .End()
        .Build();

        return tree;
    }

    private IBehaviourTreeNode HighlightTree()
    {
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();

        IBehaviourTreeNode tree = builder
            .Sequence("Highlight")
                .Inverter("[not]")
                    .Condition("Is current highlight [not] valid?", t => IsHighlightValid())
                .End()
                .Do("Clear highlight", t => ClearHighlight())
                .Selector("Highlight area or path")
                    .Do("Highlight area", t => HighlightArea())
                    .Do("Highlight path", t => HighlightPath())
                .End()
            .End()
        .Build();

        return tree;
    }

    private IBehaviourTreeNode ActTree()
    {
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();

        IBehaviourTreeNode tree = builder
            .Sequence("Act")
                .Condition("Is current character's turn?", t => IsCurrentCharactersTurn())
                .Condition("Is current character idle?", t => IsCurrentCharacterIdle())
                .Condition("Is left mouse clicked?", t => MouseButtonClicked(0))
                .Selector("Move or use ability")
                    .Sequence("Move")
                        .Condition("Path exists?", t => PathExists())
                        .Do("Move as far as possible along path", t => Move())
                    .End()
                    .Sequence("Ability")
                        .Condition("Is target cell occupied?", t => IsTargetCellOccupied())
                        .Condition("Enough time units to use ability?", t => EnoughTU(Ability.Cost))
                        .Condition("Target is within abilities range?", t => InRangeForAbility())
                        .Do("Use ability", t => UseAbility())
                    .End()
                .End()
            .End()
        .Build();

        return tree;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SetAbility((abilityIndex + 1) % Current.Abilities.Length);

        if (isCurrentCharactersTurn != Current.IsTurn)
            newTurn = true;
        else
            newTurn = false;

        isCurrentCharactersTurn = Current.IsTurn;
        movementPath = null;
        movementRange = null;

        behaviourTree.Tick(new TimeData(Time.deltaTime));
    }

    /// <summary>
    /// Get the tile cell the mouse is currently over
    /// </summary>
    private ITile<Character> GetCell()
    {
        // If mouse not over UI
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // Raycast onto tileMap (because it's the only thing with a collider in the scene)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            // If there's a hit it must be the tileMap
            if (Physics.Raycast(ray, out hitInfo))
                return grid.GetTile(hitInfo.point);
        }

        // No tile clicked
        return null;
    }

    private BehaviourTreeStatus UpdateTargetCell()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Success;

        // Get the cell currently under the cursor
        previousCell = targetCell;
        targetCell = GetCell();

        // Always return success
        return result;
    }

    private bool IsTargetCellNull()
    {
        bool targetNull = targetCell == null;
        return targetNull;
    }

    private bool IsTargetCellOccupied()
    {
        bool occupied = targetCell.Contents != null;
        return occupied;
    }

    private bool IsTargetedCellOccupantStationary()
    {
        bool stationary = !targetCell.Contents.IsMoving;
        return stationary;
    }

    private BehaviourTreeStatus GetArea()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        Character occupant = targetCell.Contents;
        movementRange = Pathfind.Area(targetCell, occupant.Stats.CurrentTimeUnits, occupant.Traverser);

        if (movementRange.Count > 0)
            result = BehaviourTreeStatus.Success;

        return result;
    }

    private BehaviourTreeStatus HighlightArea()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        if (movementRange != null)
        {
            highlighter.Highlight(movementRange);
            result = BehaviourTreeStatus.Success;
        }

        return result;
    }

    private BehaviourTreeStatus GetPath()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        movementPath = Pathfind.Between(Current.Tile, targetCell, -1, Current.Traverser);

        if (movementPath != null)
            result = BehaviourTreeStatus.Success;

        return result;
    }

    private BehaviourTreeStatus HighlightPath()
    {
        // Always return success
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        if (movementPath != null)
        {
            highlighter.Highlight(movementPath, Current.Stats.CurrentTimeUnits, Current.Stats.CurrentTimeUnits);
            result = BehaviourTreeStatus.Success;
        }

        return result;
    }

    private BehaviourTreeStatus ClearHighlight()
    {
        highlighter.Clear();

        return BehaviourTreeStatus.Success;
    }

    private bool MouseButtonClicked(int button)
    {
        bool pressed = Input.GetMouseButtonDown(button);
        return pressed;
    }

    private bool IsCurrentCharactersTurn()
    {
        bool isTurn = Current.IsTurn;
        return isTurn;
    }

    private bool IsCurrentCharacterIdle()
    {
        bool isIdle = Current.IsIdle;
        return isIdle;
    }

    private bool PathExists()
    {
        bool exists = movementPath != null;
        return exists;
    }

    private BehaviourTreeStatus Move()
    {
        Path affordablePath = movementPath.Truncate(Current.Stats.CurrentTimeUnits);
        Current.Move(affordablePath);
        return BehaviourTreeStatus.Success;
    }

    private BehaviourTreeStatus UseAbility()
    {
        // [PLACEHOLDER] TODO: pick spell to cast
        Current.UseAbility(Ability, targetCell);

        return BehaviourTreeStatus.Success;
    }

    private bool EnoughTU(float cost)
    {
        bool enoughTU = Current.Stats.CurrentTimeUnits >= cost;
        return enoughTU;
    }

    private bool IsNewCellTargeted()
    {
        bool newCell = previousCell != targetCell;
        return newCell;
    }

    private bool InRangeForAbility()
    {
        bool inRange = Ability.InRange(Current.Tile, targetCell);
        return inRange;
    }

    private bool IsHighlightValid()
    {
        bool valid = !newTurn && !IsNewCellTargeted();
        return valid;
    }

    private BehaviourTreeStatus DoEndTurn()
    {
        endTurn = false;

        turnSystem.EndTurn();

        return BehaviourTreeStatus.Success;
    }
}
