using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public HexGrid hexGrid;

    private HexCell origin;
    private HexCell destination;

    private List<HexCell> path;

    private void Update()
    {
        // If left mouse clicked
        if (Input.GetMouseButtonDown(0))
        {
            if (origin == null)
                origin = GetMousedCell();
            else
            {
                destination = GetMousedCell();
                if (origin != null && destination != null)
                    path = FindPath(origin, destination);
            }
        }

        else if (Input.GetMouseButtonDown(1))
        {
            origin = null;
            destination = null;
            path = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (origin != null)
            DrawCell(origin);

        if (destination != null)
            DrawCell(destination);

        if (path != null)
        {
            foreach (HexCell cell in path)
                DrawCell(cell);
        }

        Gizmos.color = Color.white;
    }

    private void DrawCell(HexCell cell)
    {
        Vector3[] corners = cell.GetCorners();
        for (int i = 0; i < corners.Length - 1; i++)
            Gizmos.DrawLine(corners[i], corners[i + 1]);
        Gizmos.DrawLine(corners.Last(), corners.First());
    }

    private HexCell GetMousedCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
            return hexGrid.GetCell(hitInfo.point);

        return null;
    }

    public List<HexCell> FindPath(Vector3 origin, Vector3 destination)
    {
        HexCell originCell = hexGrid.GetCell(origin);
        HexCell destinationCell = hexGrid.GetCell(destination);

        return FindPath(originCell, destinationCell);
    }

    public List<HexCell> FindPath(HexCell origin, HexCell destination)
    {
        // First, create a list of coordinates, which we will use as a queue.
        List<Step> steps = new List<Step>();

        if (origin == destination)
            return new List<HexCell>() { origin };

        // The queue will be initialized with one coordinate, the end coordinate. Each coordinate will also have a counter variable 
        // attached (the purpose of this will soon become evident). Thus, the queue starts off as ((3,8,0)).
        steps.Add(new Step(destination, 0));

        //Then, go through every element in the queue, including elements added to the end over the course of the algorithm, and to 
        // each element, do the following:
        int i = 0;
        while (i < steps.Count && steps.Last().Cell != origin)
        {
            HexCell cell = steps[i].Cell;
            int count = steps[i].Counter;

            // Create a list of the adjacent cells, with a counter variable of the current element's counter variable + 1
            Array directionValues = Enum.GetValues(typeof(HexDirection));
            for (int j = 0; j < directionValues.Length; j++)
            {
                HexDirection direction = (HexDirection)directionValues.GetValue(j);
                HexCell adjacent = cell.GetNeighbor(direction);

                // Check all cells in each list for the following two conditions: 
                if (adjacent != null &&
                    cell.GetElevationDifference(direction) <= 1 &&                          // Cell is a wall
                    !steps.Where(s => s.Cell == adjacent && s.Counter <= count + 1).Any())  // Main list contains cell with lesser or equal counter
                {
                    steps.Add(new Step(adjacent, count + 1));
                    if (adjacent == origin)
                        break;                                              // Cell is origin cell, we are done here
                }
            }   

            // Proceed to next cell in list
            i++;
        }

        // Path was not unable to reach origin cell
        if (steps.Last().Cell != origin) return null;

        // Start at end of list 
        List<HexCell> path = new List<HexCell>();

        path.Add(steps.Last().Cell);
        HexCell pathCell = steps.Last().Cell;
        int counter = steps.Last().Counter;

        foreach (Step step in steps.AsEnumerable().Reverse().Skip(1))
        {
            // Go to the nearby cell with the lowest number
            if (step.Cell.IsNeighbour(pathCell) &&
                step.Counter == counter - 1)
            {
                path.Add(step.Cell);
                pathCell = step.Cell;
                counter = step.Counter;
            }
        }

        return path;
    }

    public class Step
    {
        public HexCell Cell;
        public int Counter;

        public Step(HexCell cell, int counter)
        {
            Cell = cell;
            Counter = counter;
        }
    }
}
