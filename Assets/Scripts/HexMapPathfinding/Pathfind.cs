using System;
using System.Collections.Generic;
using System.Linq;

namespace HexMapPathfinding
{
    /// <summary>
    /// Utility methods for finding paths, areas, and ranges on a hex map
    /// </summary>
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
            foreach (Step step in Enumerate(target, maximumCost, traverser))
                inRange.Add(step);

            return inRange;
        }

        /// <summary>
        /// Finds the cheapest, traversable path from origin to destination. Returns null if no path can be found within the given cost
        /// </summary>
        /// <param name="origin">The beginning cell of the path</param>
        /// <param name="destination">The end cell of the path</param>
        /// <param name="maximumCost">The maximum cost allowed in order to find a path. -1 for no cost limit</param>
        /// <param name="traverser">The ruleset for which cells can be crossed and the cost for doing so</param>
        /// <returns>A collection of each step of the path from origin to destination OR null if no path could be found 
        /// within the given parameters</returns>
        public static HexPath To(HexCell origin, HexCell destination, float maximumCost, ITraversable traverser = null)
        {
            return To(origin,
                      s => s.Cell == destination,       // Search until the returned step is the destination cell
                      maximumCost,
                      traverser);
        }

        /// <summary>
        /// Finds the cheapest, traversable path from the origin to the cell that matches the given criteria. Returns null if no cell matches
        /// the criteria, or if no path can be found within cost
        /// </summary>
        /// <param name="origin">The cell to begin the search from</param>
        /// <param name="isTarget">Predicate that determines which cell is the desired cell</param>
        /// <param name="maximumCost">The maximum cost allowed in order to find a path. -1 for no cost limit</param>
        /// <param name="traverser">The ruleset for which cells can be crossed and the cost for doing so</param>
        /// <returns>The quickest traversable path from the origin to a cell within the given criteria and matches the given 
        /// condition. Returns null if no such path exists</returns>
        public static HexPath To(HexCell origin, Func<Step, bool> isTarget, float maximumCost, ITraversable traverser = null)
        {
            // Enumerate through cells that meet the criteria
            foreach (Step step in Enumerate(origin, maximumCost, traverser))
            {
                // If the step matches the condition, return a path from origin to this cell
                if (isTarget(step))
                    return new HexPath(step);
            }

            // No traversable path
            return null;
        }

        /// <summary>
        /// Finds the cheapest, traversable path from the origin to any cell in the collection of cells. Returns null if no path can be found
        /// to any of the cells
        /// </summary>
        /// <param name="origin">The cell to begin the search from</param>
        /// <param name="area">The collection of cells to find a path to</param>
        /// <param name="traverser">The ruleset for which cells can be crossed and the cost for doing so</param>
        /// <returns>The cheapest, traversable path from the origin to any of the cells in the collection of cells. Returns null if
        /// no such path exists</returns>
        public static HexPath ToArea(HexCell origin, IEnumerable<HexCell> area, ITraversable traverser = null)
        {
            // Pathfind to cheapest of the cells
            HexPath path = To(origin,
                                s => area.Contains(s.Cell),     // Search until the returned step is any of the cells in the area
                                -1,                             // Cost doesn't matter
                                traverser);
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
        public static bool InRange(HexCell origin, HexCell target, float maximumCost, ITraversable traverser = null)
        {
            // Find path from origin to target
            HexPath path = To(origin,
                              s => s.Cell == target,        // Search until the returned step is the target cell
                              maximumCost,
                              traverser);

            // Null = no path to target
            bool inRange = false;
            if (path != null)
                inRange = path.Cost <= maximumCost; // Is the path to target within cost?

            return inRange;
        }

        /// <summary>
        /// Iterates over all traversable cells beginning at the origin cell. Cells are traversed in order of cost
        /// </summary>
        /// <param name="origin">The cell to begin iterating from</param>
        /// <param name="maximumCost">The maximum cost</param>
        /// <param name="traverser">The ruleset for which cells can be crossed and the cost for doing so</param>
        public static IEnumerable<Step> Enumerate(HexCell origin, float maximumCost, ITraversable traverser = null)
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

                // Don't bother evaluating cells that are out of cost range
                if (maximumCost < 0 || current.CostTo <= maximumCost)
                {
                    yield return current;

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
            foreach (HexDirection direction in HexDirectionExtension.All())
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
    }
}