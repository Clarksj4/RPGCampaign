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

    public List<Step> Steps { get; private set; }
    public HexCell Origin { get { return Steps.First().Cell; } }
    public HexCell Destination { get { return Steps.Last().Cell; } }
    public float Cost { get { return Steps.Select(s => s.CostTo).Sum(); } }
    public int Cells { get { return Steps.Count; } }

    public Vector3[] Points
    {
        get
        {
            if (points == null)
                points = Steps.Select(s => s.Cell.Position).ToArray();
            return points;
        }
    }

    private Vector3[] points;

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

    public HexPath To(HexCell cell)
    {
        int index = IndexOf(cell);

        List<Step> subSteps = Steps.GetRange(0, index);

        return new HexPath(subSteps);
    }

    public HexPath To(float timeUnits)
    {
        List<Step> subSteps = Steps.Where(s => s.CostTo <= timeUnits).ToList();
        return new HexPath(subSteps);
    }

    public HexPath From(HexCell cell)
    {
        int index = IndexOf(cell);
        int remaining = Steps.Count - index;

        List<Step> subSteps = Steps.GetRange(index, remaining);

        return new HexPath(subSteps);
    }

    public int IndexOf(HexCell cell)
    {
        for (int i = 0; i < Steps.Count; i++)
        {
            if (Steps[i].Cell == cell)
                return i;
        }

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
