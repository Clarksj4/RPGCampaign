using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexPath : IEnumerable<Step>
{
    /// <summary>
    /// Gets the step at the given index in this path
    /// </summary>
    public Step this[int index]
    {
        get { return Steps[index]; }
        set { Steps[index] = value; }
    }

    /// <summary>
    /// The collection of steps in this path.
    /// </summary>
    public List<Step> Steps { get; private set; }

    /// <summary>
    /// The first step in this path
    /// </summary>
    public HexCell Origin { get { return Steps.First().Cell; } }

    /// <summary>
    /// The last step in this path
    /// </summary>
    public HexCell Destination { get { return Steps.Last().Cell; } }

    /// <summary>
    /// The total cost of moving along this path
    /// </summary>
    public float Cost { get { return Steps.Select(s => s.CostTo).Sum(); } }

    /// <summary>
    /// The number of steps in this path
    /// </summary>
    public int Count { get { return Steps.Count; } }

    /// <summary>
    /// The position of each step in this path
    /// </summary>
    public Vector3[] Points
    {
        get
        {
            // Lazy initialisation of point collection
            if (points == null)
                points = Steps.Select(s => s.Cell.Position).ToArray();
            return points;
        }
    }
    private Vector3[] points;

    /// <summary>
    /// A new empty path
    /// </summary>
    public HexPath()
    {
        Steps = new List<Step>();
    }

    /// <summary>
    /// A new path from the collection of steps
    /// </summary>
    public HexPath(List<Step> steps)
    {
        Steps = steps;
    }

    /// <summary>
    /// Creates a new path by walking backwards from the given step
    /// </summary>
    public HexPath(Step final)
        : this()
    {
        // Iterate through steps to obtain cell reference
        Step walker = final;
        while (walker != null)
        {
            // Add to list of cells
            Steps.Add(walker);
            walker = walker.Previous;
        }

        // Reverse order of collection
        Steps.Reverse();
    }

    /// <summary>
    /// Creates a new path by walking backwards from the given step while they are within the given cost
    /// maximum
    /// </summary>
    public HexPath(Step final, float maximumCost)
    : this()
    {
        // Iterate through steps to obtain cell reference
        Step walker = final;
        while (walker != null)
        {
            // Add to list of cells if within cost
            if (walker.CostTo <= maximumCost)
                Steps.Add(walker);

            walker = walker.Previous;
        }

        // Reverse order of collection
        Steps.Reverse();
    }

    /// <summary>
    /// Add a step to the path
    /// </summary>
    /// <param name="step"></param>
    public void AddStep(Step step)
    {
        Steps.Add(step);
    }

    /// <summary>
    /// Returns a new path from origin to the given cell in this path
    /// </summary>
    public HexPath To(HexCell cell)
    {
        // Index of steph that contains cell in this path
        int index = IndexOf(cell);

        // Steps from beginning until index cell
        List<Step> subSteps = Steps.GetRange(0, index);

        // Turn into new path
        return new HexPath(subSteps);
    }

    /// <summary>
    /// Returns a new path beginning at origin and containing all steps in this path less than the 
    /// given time units
    /// </summary>
    public HexPath To(float timeUnits)
    {
        // Iterate through steps adding those that are less that given time units
        HexPath subPath = new HexPath();
        foreach (Step step in Steps)
        {
            if (step.CostTo <= timeUnits)
                subPath.AddStep(step);

            // Stop once steps are out of time units range
            else
                break;
        }

        // All steps that are within cost
        return subPath;
    }

    /// <summary>
    /// Returns a new path from the given cell in this path until the end of this path
    /// </summary>
    public HexPath From(HexCell cell)
    {
        // Index of step that contains the cell
        int index = IndexOf(cell);

        // Number of steps from index cell to end of path
        int remaining = Steps.Count - index;

        List<Step> subSteps = Steps.GetRange(index, remaining);
        return new HexPath(subSteps);
    }

    /// <summary>
    /// Returns the index of the step that contains the given cell. Returns -1 when the cell is not
    /// contained in this path
    /// </summary>
    public int IndexOf(HexCell cell)
    {
        // Iterate until encountering the given cell
        for (int i = 0; i < Steps.Count; i++)
        {
            if (Steps[i].Cell == cell)
                return i;
        }

        // Cell not in path
        return -1;
    }

    public IEnumerator<Step> GetEnumerator()
    {
        return Steps.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Steps.GetEnumerator();
    }
}
