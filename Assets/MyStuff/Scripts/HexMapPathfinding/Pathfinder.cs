using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pathfinder : MonoBehaviour
{
    [Tooltip("The HexGrid to find a path on")]
    public HexGrid hexGrid;
    [Tooltip("The units available for traversing")]
    public float TimeUnits = 50f;
    [Tooltip("Whether to find all cells within TimeUnits distance?")]
    public bool cellsInRange;
    [Tooltip("Using walking traversal ruleset / costs?")]
    public bool walking;
    [Header("Traversal rules / costs")]
    public Traverser Walking = Traverser.Walking();
    public Traverser RangedAttack = Traverser.RangedAttack();

    private HexCell origin;
    private HexCell destination;
    private List<HexCell> path;
    private List<Step> range;

    private void Update()
    {
        // If left mouse clicked
        if (Input.GetMouseButtonDown(0))
        {
            if (!cellsInRange)
            {
                // Set origin cell
                if (origin == null)
                    origin = GetMousedCell();

                else
                {
                    // Set destination cell
                    destination = GetMousedCell();

                    // If theres a start and end find a path between them
                    if (origin != null && destination != null)
                        path = FindQuickestPath(origin, destination, TimeUnits, Walking);
                }
            }

            else
            {
                origin = GetMousedCell();

                if (walking)
                    range = CellsInRange(origin, TimeUnits, Walking);
                else
                    range = CellsInRange(origin, TimeUnits, RangedAttack);
            }
        }

        // If right mouse clicked
        else if (Input.GetMouseButtonDown(1))
        {
            // Clear all
            origin = null;
            destination = null;
            path = null;
            range = null;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw origin
        if (origin != null)
            DrawCell(origin, Color.red);

        // Draw destination
        if (destination != null)
            DrawCell(destination, Color.red);

        // Draw path
        if (path != null)
        {
            float increment = 1f / path.Count;
            float t = 0;

            foreach (HexCell cell in path)
            {
                // Colour changes from green to red during the path
                Color colour = Color.Lerp(Color.green, Color.red, t);
                DrawCell(cell, colour);

                t += increment;
            }
        }

        if (range != null)
        {
            foreach (Step step in range)
            {
                Color colour = Color.Lerp(Color.green, Color.red, step.CostTo / TimeUnits);

                DrawCell(step.Cell, colour);
            }
        }
    }
    
    /// <summary>
    /// Draw a cell in the given colour with Gizmo lines
    /// </summary>
    private void DrawCell(HexCell cell, Color colour)
    {
        Color oldColour = Gizmos.color;
        Gizmos.color = colour;

        Vector3[] corners = cell.GetCorners();
        for (int i = 0; i < corners.Length - 1; i++)
            Gizmos.DrawLine(corners[i] + Vector3.up, corners[i + 1] + Vector3.up);
        Gizmos.DrawLine(corners.Last() + Vector3.up, corners.First() + Vector3.up);

        Gizmos.color = oldColour;
    }

    /// <summary>
    /// Get the hex cell the mouse is currently over
    /// </summary>
    private HexCell GetMousedCell()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
                return hexGrid.GetCell(hitInfo.point);
        }

        return null;
    }

    public List<Step> CellsInRange(HexCell origin, float timeUnits, Traverser traverser)
    {
        // Collection of cells that are traversable and within 'timeUnits' range
        List<Step> inRange = new List<Step>();

        // Queue of cells whose costs have been evaluated
        List<Step> evaluated = new List<Step>();

        // Queue of discovered cells that have not yet been evaluated
        List<Step> toBeEvaluated = new List<Step>();

        // Add origin cell to collection
        toBeEvaluated.Add(new Step(origin, null, 0));

        // Evaluate ever cell that has not yet been evaluated
        while (toBeEvaluated.Count > 0)
        {
            // Current cell being evaluated
            Step current = toBeEvaluated.First();

            // Remove current cell from unevaluated cells and add to evaluated cells
            toBeEvaluated.RemoveAt(0);
            evaluated.Add(current);

            // Only consider cells where there is enough time units to traverse there
            if (current.CostTo <= timeUnits)
            {
                inRange.Add(current);

                // For each neighbour of current cell
                Array directionValues = Enum.GetValues(typeof(HexDirection));
                for (int i = 0; i < directionValues.Length; i++)
                {
                    // Direction of adjacent cell
                    HexDirection direction = (HexDirection)directionValues.GetValue(i);
                    HexCell adjacent = current.Cell.GetNeighbor(direction);

                    // Check that there is a cell in that direction   
                    if (adjacent != null &&                                       // Is there an adjacent cell in the current direction?
                        !evaluated.Select(s => s.Cell).Contains(adjacent) &&      // Has the adjacent cell already been evaluated?
                        traverser.IsTraversable(current.Cell, direction))         // Is the adjacent cell traversable from current cell?
                    {
                        // Cost to move from origin to adjacent cell with current route
                        float costToAdjacent = current.CostTo + traverser.TraverseCost(current.Cell, direction);

                        // Is adjacent a newly discovered node...?
                        Step adjacentStep = toBeEvaluated.Find(s => s.Cell == adjacent);
                        if (adjacentStep == null)
                        {
                            adjacentStep = new Step(adjacent, current, costToAdjacent);
                            InsertStep(toBeEvaluated, adjacentStep);
                        }

                        // Is the current path to this already discovered node a better path?
                        else if (costToAdjacent < adjacentStep.CostTo)
                        {
                            // This path is best until now, record it.
                            adjacentStep.Previous = current;
                            adjacentStep.CostTo = costToAdjacent;
                        }
                    }
                }
            }
        }

        return inRange;
    }

    /// <summary>
    /// Find the (equally) shortest path between the cells at the given positions. Where no path exists null is returned.
    /// </summary>
    public List<HexCell> FindQuickestPath(Vector3 origin, Vector3 destination, float timeUnits, Traverser traverser)
    {
        // Convert positions to hex cells
        HexCell originCell = hexGrid.GetCell(origin);
        HexCell destinationCell = hexGrid.GetCell(destination);

        // Find path between.
        return FindQuickestPath(originCell, destinationCell, timeUnits, traverser);
    }

    /// <summary>
    /// Find the quickest path from origin to destination where traverser defines the cost of moving to each different hex and which hexes
    /// are traversable. Where no path exists null is returned.
    /// </summary>
    public List<HexCell> FindQuickestPath(HexCell origin, HexCell destination, float timeUnits, Traverser traverser)
    {
        // You're already there..
        if (origin == destination)
            return new List<HexCell>() { origin };

        // Queue of cells whose costs have been evaluated
        List<Step> evaluated = new List<Step>();

        // Queue of discovered cells that have not yet been evaluated
        List<Step> toBeEvaluated = new List<Step>();

        // Add origin cell to collection
        toBeEvaluated.Add(new Step(origin, null, 0));

        // Evaluate ever cell that has not yet been evaluated
        while (toBeEvaluated.Count > 0)
        {
            // Current cell being evaluated
            Step current = toBeEvaluated.First();

            // Remove current cell from unevaluated cells and add to evaluated cells
            toBeEvaluated.RemoveAt(0);
            evaluated.Add(current);

            // Only consider cells where there is enough time units to traverse there
            if (current.CostTo <= timeUnits)
            {
                if (current.Cell == destination)
                    return ReconstructPath(current);

                // For each neighbour of current cell
                Array directionValues = Enum.GetValues(typeof(HexDirection));
                for (int i = 0; i < directionValues.Length; i++)
                {
                    // Direction of adjacent cell
                    HexDirection direction = (HexDirection)directionValues.GetValue(i);
                    HexCell adjacent = current.Cell.GetNeighbor(direction);

                    // Check that there is a cell in that direction   
                    if (adjacent != null &&                                       // Is there an adjacent cell in the current direction?
                        !evaluated.Select(s => s.Cell).Contains(adjacent) &&      // Has the adjacent cell already been evaluated?
                        traverser.IsTraversable(current.Cell, direction))         // Is the adjacent cell traversable from current cell?
                    {
                        // Cost to move from origin to adjacent cell with current route
                        float costToAdjacent = current.CostTo + traverser.TraverseCost(current.Cell, direction);

                        // Is adjacent a newly discovered node...?
                        Step adjacentStep = toBeEvaluated.Find(s => s.Cell == adjacent);
                        if (adjacentStep == null)
                        {
                            adjacentStep = new Step(adjacent, current, costToAdjacent);
                            InsertStep(toBeEvaluated, adjacentStep);
                        }

                        // Is the current path to this already discovered node a better path?
                        else if (costToAdjacent < adjacentStep.CostTo)
                        {
                            // This path is best until now, record it.
                            adjacentStep.Previous = current;
                            adjacentStep.CostTo = costToAdjacent;
                        }
                    }
                }
            }
        }

        return null;
    }

    public void InsertStep(List<Step> steps, Step step)
    {
        int index = 0;
        for (index = 0; index < steps.Count; index++)
        {
            if (steps[index].CostTo >= step.CostTo)
                break;
        }

        steps.Insert(index, step);
    }

    public List<HexCell> ReconstructPath(Step current)
    {
        List<HexCell> path = new List<HexCell>();

        Step walker = current;
        while (walker != null)
        {
            path.Add(walker.Cell);
            walker = walker.Previous;
        }

        return path;
    }

    public class Step
    {
        public HexCell Cell;
        public Step Previous;
        public float CostTo;

        public Step(HexCell cell, Step previous, float costTo)
        {
            Cell = cell;
            Previous = previous;
            CostTo = costTo;
        }
    }
}
