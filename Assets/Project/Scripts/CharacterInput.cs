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
    [Tooltip("The layer the hex grid is on. Used for raycasting")]
    public string HexGridLayer = "HexGrid";

    public Character CurrentCharacter { get { return Player.Current; } }

    private IBehaviourTreeNode behaviourTree;
    private HexHighlighter highlighter;

    // Behaviour tree variables
    private HexCell targetCell;
    private bool newCellTargeted;
    private ICollection<PathStep> movementRange;
    private Path movementPath;
    private Spell Spell { get { return Player.Current.Spells[0]; } }

    private void Start()
    {
        highlighter = GetComponent<HexHighlighter>();

        // Set all cell to hex grid layer
        foreach (Transform child in HexGrid.GetComponentsInChildren<Transform>())
            child.gameObject.layer = LayerMask.NameToLayer(HexGridLayer);

        // Input behaviour tree
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
        behaviourTree = builder
            .Sequence("Sequence")
                
                // If the cursor isn't on a cell, then don't do anything
                .Do("Get cell under cursor", t => GetCellBehaviour())

                // Highlight AND do an action (cast or move)
                .Parallel("Both", 2, 2)

                    .Sequence("Highlight")
                        .Condition("New cell targeted?", t => newCellTargeted)
                        .Do("Clear highlight", t => ClearHighlight())
                        .Selector("Selector")

                            // Highlight the movement range of the character in the targeted cell
                            .Sequence("Sequence")
                                .Condition("Cell occupied?", t => CellOccupied())
                                .Condition("Occupant stationary?", t => OccupantStationary())
                                .Do("Highlight move range", t => HighlightArea())
                            .End()

                            // Highlight the movement path of the selected character
                            .Sequence("Sequence")
                                .Inverter("Not")
                                    .Condition("Cell occupied?", t => CellOccupied())
                                .End()
                                .Condition("Selected character's turn?", t => IsSelectedCharactersTurn())
                                .Condition("Selected character idle?", t => IsSelectedCharacterIdle())
                                .Do("Highlight path", t => HighlightPath())
                            .End()
                        .End()
                    .End()
                    .Sequence("Act")

                        // Do an action when the mouse is clicked
                        .Condition("Left mouse clicked?", t => Input.GetMouseButtonDown(0))
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
                                .Condition("Enough TU?", t => EnoughTU(Spell.Cost))
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
        targetCell = null;
        movementPath = null;
        movementRange = null;

        behaviourTree.Tick(new TimeData(Time.deltaTime));
    }

    private void OnDrawGizmos()
    {
        // Draw destination
        if (targetCell != null)
            DrawCell(targetCell, Color.white);

        // Draw movementPath
        if (movementPath != null)
        {
            foreach (PathStep step in movementPath)
            {
                // Cell is green if in range
                if (step.CostTo <= CurrentCharacter.Stats.CurrentTimeUnits)
                    DrawCell((HexCell)step.Node, Color.green);

                // Cell is red if out of range
                else
                    DrawCell((HexCell)step.Node, Color.red);
            }
        }

        // Draw all cells in range
        if (movementRange != null)
        {
            foreach (PathStep step in movementRange)
                DrawCell((HexCell)step.Node, Color.green);
        }
    }

    /// <summary>
    /// Draw a cell in the given colour with Gizmo lines
    /// </summary>
    private void DrawCell(HexCell cell, Color colour)
    {
        // Set colour, remember old colour
        Color oldColour = Gizmos.color;
        Gizmos.color = colour;

        // Draw line from each vert to next vert
        Vector3[] corners = cell.GetCorners();
        for (int i = 0; i < corners.Length - 1; i++)
            Gizmos.DrawLine(corners[i] + Vector3.up, corners[i + 1] + Vector3.up);
        Gizmos.DrawLine(corners.Last() + Vector3.up, corners.First() + Vector3.up);

        // Reset colour
        Gizmos.color = oldColour;
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

            int layer = 1 << LayerMask.NameToLayer(HexGridLayer);

            // If there's a hit it must be the hexmesh
            if (Physics.Raycast(ray, out hitInfo, layer))
                return HexGrid.GetCell(hitInfo.point);
        }

        // No cell clicked
        return null;
    }

    private BehaviourTreeStatus GetCellBehaviour()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        // Get the cell currently under the cursor
        HexCell current = GetCell();
        if (current != targetCell)
        {
            newCellTargeted = true;
            targetCell = current;
        }

        else
            newCellTargeted = false;


        // If there is one, success!
        if (targetCell != null)
            result = BehaviourTreeStatus.Success;

        return result;
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

    private BehaviourTreeStatus HighlightArea()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        Character occupant = targetCell.Occupant;
        movementRange = Pathfind.Area(targetCell, occupant.Stats.CurrentTimeUnits, occupant.Stats.Traverser);

        if (movementRange.Count > 0)
        {
            highlighter.Highlight(movementRange);
            result = BehaviourTreeStatus.Success;
        }

        return result;
    }

    private BehaviourTreeStatus HighlightPath()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        movementPath = Pathfind.Between(CurrentCharacter.Cell, targetCell, -1, CurrentCharacter.Stats.Traverser);

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
        CurrentCharacter.Cast(Spell, targetCell);

        return BehaviourTreeStatus.Success;
    }

    private bool EnoughTU(float cost)
    {
        bool enoughTU = CurrentCharacter.Stats.CurrentTimeUnits >= cost;
        return enoughTU;
    }
}
