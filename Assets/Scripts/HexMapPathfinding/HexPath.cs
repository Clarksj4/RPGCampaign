using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexPath : IEnumerable<Step>
{
    public Step this[int index]
    {
        get { return Steps[index]; }
        set { Steps[index] = value; }
    }

    public Vector3[] Points { get { return Steps.Select(s => s.Cell.Position).ToArray(); } }
    public List<Step> Steps { get; private set; }
    public HexCell Origin { get { return Steps.First().Cell; } }
    public HexCell Destination { get { return Steps.Last().Cell; } }
    public float Cost { get { return Steps.Select(s => s.CostTo).Sum(); } }
    public int Cells { get { return Steps.Count; } }

    public HexPath()
    {
        Steps = new List<Step>();
    }

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

        Steps.Reverse();
    }

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

        Steps.Reverse();
    }

    public void AddStep(Step step)
    {
        Steps.Add(step);
    }

    /// <summary>
    /// Returns all the steps from this path that are within the given timeUnits range of the origin cell
    /// </summary>
    public HexPath Truncate(float timeUnits)
    {
        HexPath inRangePath = new HexPath();
        foreach (Step step in Steps)
        {
            if (step.CostTo <= timeUnits)
                inRangePath.AddStep(step);
        }

        return inRangePath;
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
