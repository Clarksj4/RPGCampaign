using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [Tooltip("The HexGrid to find a path on")]
    public HexGrid hexGrid;

    private HexCell origin;
    private HexCell destination;
    private List<HexCell> path;

    private void Update()
    {
        // If left mouse clicked
        if (Input.GetMouseButtonDown(0))
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
                    path = FindPath(origin, destination, new WaterElementalTraversable());
            }
        }

        // If right mouse clicked
        else if (Input.GetMouseButtonDown(1))
        {
            // Clear all
            origin = null;
            destination = null;
            path = null;
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
            Gizmos.DrawLine(corners[i], corners[i + 1]);
        Gizmos.DrawLine(corners.Last(), corners.First());

        Gizmos.color = oldColour;
    }

    /// <summary>
    /// Get the hex cell the mouse is currently over
    /// </summary>
    private HexCell GetMousedCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
            return hexGrid.GetCell(hitInfo.point);

        return null;
    }

    /// <summary>
    /// Find the (equally) shortest path between the cells at the given positions. Where no path exists null is returned.
    /// </summary>
    public List<HexCell> FindPath(Vector3 origin, Vector3 destination, ITraverser traverser)
    {
        // Convert positions to hex cells
        HexCell originCell = hexGrid.GetCell(origin);
        HexCell destinationCell = hexGrid.GetCell(destination);

        // Find path between.
        return FindPath(originCell, destinationCell, traverser);
    }

    /// <summary>
    /// Find the (equally) shortest path between the given cells. Where no path exists null is returned.
    /// </summary>
    public List<HexCell> FindPath(HexCell origin, HexCell destination, ITraverser traverser)
    {
        // You're already there..
        if (origin == destination)                      
            return new List<HexCell>() { origin };

        // Queue of cells whose costs have been evaluated
        List<HexCell> evaluatedCells = new List<HexCell>();

        // Queue of discovered cells that have not yet been evaluated
        SortedList<float, HexCell> toBeEvaluatedCells = new SortedList<float, HexCell>();

        // Map of which cell each cell is most easily reached from
        Dictionary<HexCell, HexCell> cameFromDirectory = new Dictionary<HexCell, HexCell>();

        // Map of costs expended to reach each cell from the origin
        Dictionary<HexCell, float> costToCell = new Dictionary<HexCell, float>();

        // Map of estimated costs to reach the destination cell from each cell
        Dictionary<HexCell, float> costToDesinationViaCellEstimates = new Dictionary<HexCell, float>();

        // Add origin cell to collections
        float distanceEstimate = Vector3.Distance(origin.Position, destination.Position);
        toBeEvaluatedCells.Add(distanceEstimate, origin);
        costToCell.Add(origin, 0);
        costToDesinationViaCellEstimates.Add(origin, distanceEstimate);

        // Evaluate ever cell that has not yet been evaluated
        while (toBeEvaluatedCells.Count > 0)
        {
            // Current cell and the number of steps to reach it
            HexCell current = toBeEvaluatedCells.First().Value;

            if (current == destination)
                return ReconstructPath(cameFromDirectory, current);

            // Remove current cell from unevaluated cells and add to evaluated cells
            toBeEvaluatedCells.RemoveAt(0);
            evaluatedCells.Add(current);

            // For each neighbour of current cell
            Array directionValues = Enum.GetValues(typeof(HexDirection));
            for (int i = 0; i < directionValues.Length; i++)
            {
                // Direction of adjacent cell
                HexDirection direction = (HexDirection)directionValues.GetValue(i);
                HexCell adjacent = current.GetNeighbor(direction);

                // Check that there is a cell in that direction   
                if (adjacent != null)                                                    
                {
                    // Ignore neighbours that have already been evaluated
                    if (evaluatedCells.Contains(adjacent))
                        continue;

                    // Ignore neighbours that cannot be traversed to from the current cell
                    if (!traverser.IsTraversable(current, direction))
                        continue;

                    // Cost to move from origin to adjacent cell with current route
                    float costToAdjacent = costToCell[current] + traverser.TraverseCost(current, direction);

                    // Estimated cost to move from origin to destination via 'adjacent'
                    float costToDestinationViaAdjacentEstimate = costToAdjacent + Vector3.Distance(adjacent.Position, destination.Position);

                    // Is adjacent a newly discovered node...?
                    if (!toBeEvaluatedCells.ContainsValue(adjacent))
                    {
                        // Add to list of nodes to be examined
                        toBeEvaluatedCells.Add(costToDestinationViaAdjacentEstimate, adjacent);
                        cameFromDirectory.Add(adjacent, current);       // Easiest cell to reach 'adjacent' from
                        costToCell.Add(adjacent, costToAdjacent);       // Cost to move from origin to 'adjacent'
                        costToDesinationViaCellEstimates.Add(adjacent, costToDestinationViaAdjacentEstimate);   
                    }


                    // If the current path to this already discovered node a better path?
                    else if (costToAdjacent > costToCell[adjacent])
                        continue;           // Nope, don't update route

                    else
                    {
                        // This path is best until now, record it.
                        cameFromDirectory[adjacent] = current;
                        costToCell[adjacent] = costToAdjacent;
                        costToDesinationViaCellEstimates[adjacent] = costToDestinationViaAdjacentEstimate;
                    }
                }
            }   
        }

        return null;
    }

    public List<HexCell> ReconstructPath(Dictionary<HexCell, HexCell> cameFromDirectory, HexCell current)
    {
        List<HexCell> path = new List<HexCell>();

        HexCell walker = current;
        while (cameFromDirectory.ContainsKey(walker))
        {
            path.Add(walker);

            walker = cameFromDirectory[walker];
        }

        return path;
    }
}
