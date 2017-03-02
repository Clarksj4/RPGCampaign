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
                    path = FindPath(origin, destination);
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
    public List<HexCell> FindPath(Vector3 origin, Vector3 destination)
    {
        // Convert positions to hex cells
        HexCell originCell = hexGrid.GetCell(origin);
        HexCell destinationCell = hexGrid.GetCell(destination);

        // Find path between.
        return FindPath(originCell, destinationCell);
    }

    /// <summary>
    /// Find the (equally) shortest path between the given cells. Where no path exists null is returned.
    /// </summary>
    public List<HexCell> FindPath(HexCell origin, HexCell destination)
    {
        // You're already there..
        if (origin == destination)                      
            return new List<HexCell>() { origin };

        // Queue of traversed cells including the number of cells crossed in order to reach each one
        List<Step> steps = new List<Step>();

        // Start from end coordinate and work backwards
        steps.Add(new Step(destination, 0));

        // Go through every element in the queue, including elements added to the end over the course of the algorithm
        int i = 0;
        while (i < steps.Count && steps.Last().Cell != origin)
        {
            // Current cell and the number of steps to reach it
            HexCell cell = steps[i].Cell;
            int count = steps[i].Counter;

            // Go through each adjacent cell (by checking for a cell in each HexDirection)
            Array directionValues = Enum.GetValues(typeof(HexDirection));
            for (int j = 0; j < directionValues.Length; j++)
            {
                // Direction of adjacent cell
                HexDirection direction = (HexDirection)directionValues.GetValue(j);
                HexCell adjacent = cell.GetNeighbor(direction);

                if (adjacent != null &&                                                    // Check that there is a cell in that direction
                    !steps.Where(s => s.Cell == adjacent && s.Counter <= count + 1).Any()) // Check that the cell has not already be added to the queue at a lower index
                {
                    // If a character is able to traverse from cell to adjacent
                    if (cell.Traversable(direction))
                    {
                        // Add it as a possible step, add one to count to get the number of cells crossed to get here
                        steps.Add(new Step(adjacent, count + 1));

                        // If the adjacent cell if the origin cell (the cell we are trying to reach) then this part of the algorithm is complete
                        if (adjacent == origin)
                            break;
                    }
                }
            }   

            // Proceed to next cell in list
            i++;
        }

        // Path was unable to reach origin cell
        if (steps.Last().Cell != origin)
            return null;

        // Create a new queue to contain the viable path
        List<HexCell> path = new List<HexCell>();

        // Add origin cell and remember number of steps crossed to get there
        path.Add(steps.Last().Cell);
        HexCell pathCell = steps.Last().Cell;
        int counter = steps.Last().Counter;

        // Iterate backwards through queue (so path goes from origin to destination)
        foreach (Step step in steps.AsEnumerable().Reverse().Skip(1))
        {
            // Get the next neighbouring step that is traverable 
            if (step.Cell.IsNeighbour(pathCell) &&
                step.Counter == counter - 1 &&
                step.Cell.Traversable(pathCell))
            {
                // Add to path, remember number of steps crossed to get there
                path.Add(step.Cell);
                pathCell = step.Cell;
                counter = step.Counter;
            }
        }

        // Return the completed path
        return path;
    }

    /// <summary>
    /// Container for a hex cell and the number of cells crossed to reach the cell during the course of finding a path.
    /// </summary>
    public class Step
    {
        public HexCell Cell;
        public int Counter;

        public Step(HexCell cell, int counter)
        {
            Cell = cell;
            Counter = counter;
        }

        public override string ToString()
        {
            return Cell.ToString() + " : " + Counter.ToString();
        }
    }
}
