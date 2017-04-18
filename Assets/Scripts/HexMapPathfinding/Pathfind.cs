using System;
using System.Collections.Generic;
using System.Linq;

public static class Pathfind
{
    /// <summary>
    /// Finds all traversable cells within cost of the given cell.
    /// </summary>
    /// <param name="target">The cell to find cells within range of.</param>
    /// <param name="maximumCost">The maximum cost of traversing to any cell in range</param>
    /// <param name="traverser">The ruleset for which tiles can be traversed and the cost of doing so</param>
    /// <returns> Steps that are traversable and within range of origin</returns>
    public static ICollection<Step> Area(HexCell target, float maximumCost, ITraversable traverser)
    {
        List<Step> inRange = new List<Step>();                    

        // Enumerate over each step that is in range, add it to the collection
        foreach (Step step in Enumerate(target, traverser, s => s.CostTo < maximumCost))
            inRange.Add(step);

        return inRange;
    }

    // TODO: Only needs ITraversable NOT ITraverseCost
    
    /// <summary>
    /// Finds the quickest, traversable path from origin to destination.
    /// </summary>
    /// <param name="origin">The beginning cell of the path</param>
    /// <param name="destination">The end cell of the path</param>
    /// <param name="traverser">The ruleset for which cells can be crossed and the cost for doing so</param>
    /// <returns>The quickest, traversable path from origin to destination. Null if no traversable path exists</returns>
    public static HexPath To(HexCell origin, HexCell destination, ITraversable traverser)
    {
        return To(origin, destination, -1, traverser);
    }

    /// <summary>
    /// Finds the quickest, traversable path from origin to destination within the given timeUnits.
    /// </summary>
    /// <param name="origin">The beginning cell of the path</param>
    /// <param name="destination">The end cell of the path</param>
    /// <param name="maximumCost">The maximum cost allowed in order to find a path. -1 for no cost limit</param>
    /// <param name="traverser">The ruleset for which cells can be crossed and the cost for doing so</param>
    /// <returns>A collection of each step of the path from origin to destination OR null if no path could be found 
    /// within the given parameters</returns>
    public static HexPath To(HexCell origin, HexCell destination, float maximumCost, ITraversable traverser)
    {
        return To(origin, traverser,
                s => maximumCost < 0 || s.CostTo <= maximumCost,
                s => s.Cell == destination);
    }

    /// <summary>
    /// Finds the quickest traversable path from the origin to the cell that is within the given criteria and matches the 
    /// given condition.
    /// </summary>
    /// <param name="origin">The cell to begin the search from</param>
    /// <param name="traverser">The ruleset for which cells can be crossed and the cost for doing so</param>
    /// <param name="withinParameters">Predicate that defines whether a cells neighbours will be examined during pathfinding</param>
    /// <param name="isTarget">Predicate that determines which cell is the desired cell</param>
    /// <returns>The quickest traversable path from the origin to a cell within the given criteria and matches the given 
    /// condition. Returns null if no such path exists</returns>
    public static HexPath To(HexCell origin, ITraversable traverser, Func<Step, bool> withinParameters, Func<Step, bool> isTarget)
    {
        // Enumerate through cells that meet the criteria
        foreach (Step step in Enumerate(origin, traverser, withinParameters))
        {
            // If the step matches the condition, return a path from origin to this cell
            if (isTarget(step))
                return new HexPath(step);
        }

        // No traversable path
        return null;
    }

    /// <summary>
    /// Finds a path from origin to the cheapest, traversable cell in the given collection of cells
    /// </summary>
    /// <param name="origin">The cell to find a path from</param>
    /// <param name="traverser">The ruleset for which cells can be crossed and the cost for doing so from the origin cell</param>
    /// <returns>A path from the origin to the cheapest, traversable cell within maximum cost of the target cell</returns>
    public static HexPath ToArea(HexCell origin, ITraversable traverser, ICollection<Step> area)
    {
        // Convert area to cells
        IEnumerable<HexCell> cells = area.Select(s => s.Cell);

        // Pathfind to cheapest of the cells
        HexPath path = To(origin, traverser,
                        s => true,
                        s => cells.Contains(s.Cell));
        return path;
    }

    /// <summary>
    /// Checks if the origin cell is within range of the target cell
    /// </summary>
    /// <param name="origin">The cell to check the range from</param>
    /// <param name="target">The cell to check for inclusion within the range</param>
    /// <param name="maximumCost">The size of the area to check</param>
    /// <param name="traverser">The ruleset for which cell can be crossed and the cost for doing so from the origin</param>
    /// <returns>Wether the target cell is within range of the target cell</returns>
    public static bool InRange(HexCell origin, HexCell target, float maximumCost, ITraversable traverser)
    {
        // Find path from origin to target
        HexPath path = To(origin, traverser, 
                        s => s.CostTo <= maximumCost, 
                        s => s.Cell == target);

        // Null = no path to target
        bool inRange = false;
        if (path != null)
            inRange = path.Cost <= maximumCost; // Is the path to target within cost?

        return inRange;
    }

    // TODO: Enumerate without conditions. Starts at origin. Enumerates based on step cost - i.e. cheapest steps iterated over first

    ///// <summary>
    ///// Iterates over all traversable cells that meet the given criteria, beginning at the origin cell.
    ///// </summary>
    ///// <param name="origin">The cell to begin iterating from</param>
    ///// <param name="traverser">The ruleset for which cells can be crossed and the cost for doing so</param>
    ///// <param name="withinParameters">Function that determines whether a cells neighbous will be iterated over</param>
    //public static IEnumerable<Step> Enumerate(HexCell origin, ITraversable traverser, Func<Step, bool> withinParameters)
    //{
    //    HashSet<Step> evaluated = new HashSet<Step>();                  // Cells whose cost has been evaluated
    //    LinkedList<Step> toBeEvaluated = new LinkedList<Step>();        // Discovered cells that have not yet been evaluated

    //    // Add origin cell to collection and iterate
    //    toBeEvaluated.AddFirst(new Step(origin, null, 0));
    //    while (toBeEvaluated.Count > 0)
    //    {
    //        // Remove current cell from unevaluated cells and add to evaluated cells
    //        Step current = toBeEvaluated.PopFirst();
    //        evaluated.Add(current);

    //        yield return current;

    //        if (withinParameters(current))
    //        {
    //            // Checks each cell adjacent to current, adds it to toBeEvaluated cells or updates it 
    //            // if travelling through current is a better route
    //            EvaluateAdjacent(current, toBeEvaluated, evaluated, traverser);
    //        }
    //    }
    //}

    /// <summary>
    /// Iterates over all traversable cells beginning at the origin cell. Cells are traversed in order of cost
    /// </summary>
    /// <param name="origin">The cell to begin iterating from</param>
    /// <param name="traverser">The ruleset for which cells can be crossed and the cost for doing so</param>
    public static IEnumerable<Step> Enumerate(HexCell origin, ITraversable traverser, float maximumCost)
    {
        HashSet<Step> evaluated = new HashSet<Step>();                  // Cells whose cost has been evaluated
        LinkedList<Step> toBeEvaluated = new LinkedList<Step>();        // Discovered cells that have not yet been evaluated

        // Add origin cell to collection and iterate
        toBeEvaluated.AddFirst(new Step(origin, null, 0));
        while (toBeEvaluated.Count > 0)
        {
            // Remove current cell from unevaluated cells and add to evaluated cells
            Step current = toBeEvaluated.PopFirst();
            evaluated.Add(current);

            yield return current;

            // Don't bother evaluating cells that are out of cost range
            if (maximumCost < 0 || current.CostTo <= maximumCost)
            {
                // Checks each cell adjacent to current, adds it to toBeEvaluated cells or updates it 
                // if travelling through current is a better route
                EvaluateAdjacent(current, toBeEvaluated, evaluated, traverser);
            }
        }
    }

    /// <summary>
    /// Finds each of the given cells neighbours, adding them to the queue of cells to be evaluated OR updating their
    /// cost and path to information should they already exist in the queue.
    /// </summary>
    /// <param name="current">The cell whose neighbous are to be checked</param>
    /// <param name="toBeEvaluated">Queue of cells that are due to be evaluated</param>
    /// <param name="evaluated">Set of cells that have already been evaluated</param>
    /// <param name="traverser">The ruleset that defines which cells can be crossed and the cost for doing so</param>
    private static void EvaluateAdjacent(Step current, LinkedList<Step> toBeEvaluated, HashSet<Step> evaluated, ITraversable traverser)
    {
        // For each neighbour of current cell
        foreach (HexDirection direction in Directions())
        {
            HexCell adjacent = current.Cell.GetNeighbor(direction);
            if (adjacent != null &&                                       // Is there an adjacent cell in the given direction?
                !evaluated.Select(s => s.Cell).Contains(adjacent))        // Has the adjacent cell already been evaluated?
            {
                // Able to traverse to adjacent cell from current?
                bool traversable = traverser != null ? traverser.IsTraversable(current.Cell, direction) : true;
                if (!traversable)
                    continue;

                // Cost to move from origin to adjacent cell with current route
                float costIncrement = traverser != null ? traverser.Cost(current.Cell, direction) : 1;
                float costToAdjacent = current.CostTo + costIncrement;

                InsertOrUpdate(current, adjacent, toBeEvaluated, costToAdjacent);
            }
        }
    }

    /// <summary>
    /// Adds the adjacent cell to the queue of cells to be evaluated OR updates its
    /// cost and path to information should it already exist in the queue.
    /// </summary>
    private static void InsertOrUpdate(Step current, HexCell adjacent, LinkedList<Step> toBeEvaluated, float costToAdjacent)
    {
        // Is adjacent a newly discovered node...?
        Step adjacentStep = toBeEvaluated.Where(s => s.Cell == adjacent).SingleOrDefault();
        if (adjacentStep == null)
        {
            adjacentStep = new Step(adjacent, current, costToAdjacent);
            InsertStepByCost(toBeEvaluated, adjacentStep);
        }

        // Is the current path to this already discovered node a better path?
        else if (costToAdjacent < adjacentStep.CostTo)
        {
            // This path is best until now, record it.
            adjacentStep.Previous = current;
            adjacentStep.CostTo = costToAdjacent;
        }
    }

    /// <summary>
    /// Inserts the given step into the step list ordered based upon the cost to move to each step
    /// </summary>
    private static void InsertStepByCost(LinkedList<Step> toBeEvaluated, Step step)
    {
        // Iterate until finding a larger cost step, or running out of elements to iterate
        LinkedListNode<Step> walker = toBeEvaluated.First;
        while (walker != null && walker.Value.CostTo < step.CostTo)
            walker = walker.Next;

        // If at end of list, step is largest cost step so add to end of list
        if (walker == null)
            toBeEvaluated.AddLast(step);

        // Otherwise, insert in order according to cost
        else
            toBeEvaluated.AddBefore(walker, step);
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
