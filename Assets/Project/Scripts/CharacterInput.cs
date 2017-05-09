﻿using System.Linq;
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
    private Ability Ability { get { return Player.Current.Abilities[0]; } }

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
                        .Condition("New cell targeted?", t => IsNewCellTargeted())
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
        movementPath = null;
        movementRange = null;

        behaviourTree.Tick(new TimeData(Time.deltaTime));
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
            highlighter.Highlight(movementPath, CurrentCharacter.Stats.CurrentTimeUnits);
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

    //private void OnDrawGizmos()
    //{
    //    // Draw destination
    //    if (targetCell != null)
    //        DrawCell(targetCell, Color.white);

    //    // Draw movementPath
    //    if (movementPath != null)
    //    {
    //        foreach (PathStep step in movementPath)
    //        {
    //            // Cell is green if in range
    //            if (step.CostTo <= CurrentCharacter.Stats.CurrentTimeUnits)
    //                DrawCell((HexCell)step.Node, Color.green);

    //            // Cell is red if out of range
    //            else
    //                DrawCell((HexCell)step.Node, Color.red);
    //        }
    //    }

    //    // Draw all cells in range
    //    if (movementRange != null)
    //    {
    //        foreach (PathStep step in movementRange)
    //            DrawCell((HexCell)step.Node, Color.green);
    //    }
    //}

    ///// <summary>
    ///// Draw a cell in the given colour with Gizmo lines
    ///// </summary>
    //private void DrawCell(HexCell cell, Color colour)
    //{
    //    // Set colour, remember old colour
    //    Color oldColour = Gizmos.color;
    //    Gizmos.color = colour;

    //    // Draw line from each vert to next vert
    //    Vector3[] corners = cell.GetCorners();
    //    for (int i = 0; i < corners.Length - 1; i++)
    //        Gizmos.DrawLine(corners[i] + Vector3.up, corners[i + 1] + Vector3.up);
    //    Gizmos.DrawLine(corners.Last() + Vector3.up, corners.First() + Vector3.up);

    //    // Reset colour
    //    Gizmos.color = oldColour;
    //}
}
