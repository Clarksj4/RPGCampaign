using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using FluentBehaviourTree;
using Pathfinding;

public class CharacterInput : MonoBehaviour
{
    [Tooltip("The player being controlled by this input")]
    public Player Player;
    [Tooltip("The hex grid the character exists on")]
    public HexGrid HexGrid;

    public Character CurrentCharacter { get { return Player.Current; } }

    private IBehaviourTreeNode behaviourTree;
    private HexHighlighter highlighter;

    // Behaviour tree variables
    private HexCell previousCell;
    private HexCell targetCell;
    private ICollection<PathStep> movementRange;
    private Path movementPath;
    private Ability Ability { get { return Player.Current.Abilities[abilityIndex]; } }
    private bool isSelectedTurn;
    private bool newTurn;
    private int abilityIndex = 0;
    private bool endTurn = false;

    private void Start()
    {
        highlighter = GetComponent<HexHighlighter>();

        // Input behaviour tree
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
        behaviourTree = builder
            .Sequence("Sequence")
                
                // If the cursor isn't on a cell, then don't do anything
                .Do("Update targeted cell", t => UpdateTargetCell())
                .Inverter("Not")
                    .Condition("Target cell is null", t => IsTargetNull())
                .End()
                .Selector("Get path or range")
                    .Sequence("Get Range")
                        .Condition("Cell occupied?", t => CellOccupied())
                        .Condition("Occupant stationary?", t => OccupantStationary())
                        .Do("Get area", t => GetArea())
                    .End()
                    .Sequence("Get Path")
                        .Inverter("Not")
                            .Condition("Cell occupied?", t => CellOccupied())
                        .End()
                        .Condition("Selected character idle?", t => IsSelectedCharacterIdle())
                        .Do("Get Path", t => GetPath())
                    .End()
                .End()

                // Highlight AND do an action (cast or move)
                .Parallel("Both", 2, 2)

                    // Highlight range or path
                    .Sequence("Highlight")
                        .Inverter("Not")
                            .Condition("Highlight valid?", t => IsHighlightValid())
                        .End()
                        .Do("Clear highlight", t => ClearHighlight())
                        .Selector("Highlight path or range")
                            .Do("Highlight move range", t => HighlightArea())
                            .Do("Highlight path", t => HighlightPath())
                        .End()
                    .End()

                    // Do a thing
                    .Sequence("Act")

                        // Do an action when the mouse is clicked
                        .Condition("Left mouse clicked?", t => MouseButtonClicked(0))
                        .Condition("Selected character's turn?", t => IsSelectedCharactersTurn())
                        .Condition("Selected character idle?", t => IsSelectedCharacterIdle())
                        .Selector("Move or Attack")

                            // Move along the affordable section of the path
                            .Sequence("Sequence")
                                .Condition("Path exists", t => PathExists())
                                .Do("Move along affordable section of path", t => Move())
                            .End()

                            // Cast spell at the targeted cell
                            .Sequence("Sequence")
                                .Condition("Cell occupied?", t => CellOccupied())
                                .Condition("Occupant stationary?", t => OccupantStationary())
                                .Condition("Enough TU?", t => EnoughTU(Ability.Cost))
                                .Condition("In range for ability", t => InRangeForAbility())
                                .Do("Cast", t => Cast())
                            .End()
                        .End()
                    .End()
                .End()
            .End()
        .Build();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SetAbility((abilityIndex + 1) % CurrentCharacter.Abilities.Length);

        if (isSelectedTurn != CurrentCharacter.IsTurn)
            newTurn = true;
        else
            newTurn = false;

        isSelectedTurn = CurrentCharacter.IsTurn;
        movementPath = null;
        movementRange = null;

        behaviourTree.Tick(new TimeData(Time.deltaTime));
    }

    public void SetAbility(int index)
    {
        abilityIndex = index;
    }

    public void EndTurn()
    {
        endTurn = true;
    }

    /// <summary>
    /// Get the hex cell the mouse is currently over
    /// </summary>
    private HexCell GetCell()
    {
        // If mouse not over UI
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // Raycast onto hexmesh (because it's the only thing in the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            // If there's a hit it must be the hexmesh
            if (Physics.Raycast(ray, out hitInfo))
                return HexGrid.GetCell(hitInfo.point);
        }

        // No cell clicked
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

    private bool IsTargetNull()
    {
        bool targetNull = targetCell == null;
        return targetNull;
    }

    private bool CellOccupied()
    {
        bool occupied = targetCell.Occupant != null;
        return occupied;
    }

    private bool OccupantStationary()
    {
        bool stationary = !targetCell.Occupant.IsMoving;
        return stationary;
    }

    private BehaviourTreeStatus GetArea()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        Character occupant = targetCell.Occupant;
        movementRange = Pathfind.Area(targetCell, occupant.Stats.CurrentTimeUnits, occupant.Stats.Traverser);

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

        movementPath = Pathfind.Between(CurrentCharacter.Cell, targetCell, -1, CurrentCharacter.Stats.Traverser);

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
            highlighter.Highlight(movementPath, CurrentCharacter.Stats.CurrentTimeUnits, CurrentCharacter.Stats.CurrentTimeUnits);
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

    private bool IsSelectedCharactersTurn()
    {
        bool isTurn = CurrentCharacter.IsTurn;
        return isTurn;
    }

    private bool IsSelectedCharacterIdle()
    {
        bool isIdle = CurrentCharacter.IsIdle;
        return isIdle;
    }

    private bool PathExists()
    {
        bool exists = movementPath != null;
        return exists;
    }

    private BehaviourTreeStatus Move()
    {
        Path affordablePath = movementPath.Truncate(CurrentCharacter.Stats.CurrentTimeUnits);
        CurrentCharacter.Move(affordablePath);
        return BehaviourTreeStatus.Success;
    }

    private BehaviourTreeStatus Cast()
    {
        // [PLACEHOLDER] TODO: pick spell to cast
        //CurrentCharacter.Cast(Spell, targetCell);
        CurrentCharacter.UseAbility(Ability, targetCell);

        return BehaviourTreeStatus.Success;
    }

    private bool EnoughTU(float cost)
    {
        bool enoughTU = CurrentCharacter.Stats.CurrentTimeUnits >= cost;
        return enoughTU;
    }

    private bool IsNewCellTargeted()
    {
        bool newCell = previousCell != targetCell;
        return newCell;
    }

    private bool InRangeForAbility()
    {
        bool inRange = Ability.InRange(CurrentCharacter.Cell, targetCell);
        return inRange;
    }

    private bool IsHighlightValid()
    {
        bool valid = !newTurn && !IsNewCellTargeted();
        return valid;
    }

    private void DoEndTurn()
    {
        endTurn = false;

        // Tell turn system to end turn
    }
}
