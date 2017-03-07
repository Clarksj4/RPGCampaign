﻿using System;
using System.Collections.Generic;
using System.Linq;

public static class Pathfind
{
    /// <summary>
    /// Finds all cells within 'timeUnits' range of the given cell. The 'traverser' defines the ruleset for which tiles can
    /// be crossed and the cost of doing so.
    /// </summary>
    /// <param name="origin">The cell to find cells within range of.</param>
    /// <param name="maximumCost">The maximum cost of traversing to any cell in range</param>
    /// <param name="traverser">The ruleset for which tiles can be traversed and the cost of doing so</param>
    /// <returns>A collection of each step from origin towards the maximum distance</returns>
    public static List<Step> CellsInRange(HexCell origin, float maximumCost, Traverser traverser)
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
            if (current.CostTo <= maximumCost)
            {
                inRange.Add(current);

                // For each neighbour of current cell
                foreach (HexDirection direction in Directions())
                {
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
    /// Finds the quickest path from origin to destination within the given timeUnits. The 'traverser' defines the ruleset
    /// for which cells can be crossed and the cost for doing so.
    /// </summary>
    /// <param name="origin">The beginning cell of the path</param>
    /// <param name="destination">The end cell of the path</param>
    /// <param name="maximumCost">The maximum cost allowed in order to find a path</param>
    /// <param name="traverser">The ruleset for which cells can be crossed and the cost for doing so</param>
    /// <returns>A collection of each step of the path from origin to destination OR null if no path could be found 
    /// within the given parameters</returns>
    public static List<Step> QuickestPath(HexCell origin, HexCell destination, float maximumCost, Traverser traverser)
    {
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

            // Only consider cells that can be reached within cost
            if (maximumCost < 0 || current.CostTo <= maximumCost)
            {
                if (current.Cell == destination)
                    return ReconstructPath(current);

                // For each neighbour of current cell
                foreach (HexDirection direction in Directions())
                {
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

        // No path
        return null;
    }

    /// <summary>
    /// Inserts the given step into the step list ordered based upon the cost to move to each step
    /// </summary>
    private static void InsertStep(List<Step> steps, Step step)
    {
        int index = 0;
        // Loop until end of collection or finding a larger cost step
        for (index = 0; index < steps.Count; index++)
        {
            if (steps[index].CostTo >= step.CostTo)
                break;
        }

        // Insert in front of larger step (or end of collection)
        steps.Insert(index, step);
    }

    /// <summary>
    /// Recreate the path from origin to destination by following the given step's previous steps
    /// </summary>
    private static List<Step> ReconstructPath(Step current)
    {
        List<Step> path = new List<Step>();

        // Iterate through steps to obtain cell reference
        Step walker = current;
        while (walker != null)
        {
            // Add to list of cells
            path.Add(walker);
            walker = walker.Previous;
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// Enumerable of all the different hex directions
    /// </summary>
    private static IEnumerable<HexDirection> Directions()
    {
        // Get all directions
        Array directionValues = Enum.GetValues(typeof(HexDirection));

        // Iterate through collection returning each one
        for (int i = 0; i < directionValues.Length; i++)
            yield return (HexDirection)directionValues.GetValue(i);
    }
}