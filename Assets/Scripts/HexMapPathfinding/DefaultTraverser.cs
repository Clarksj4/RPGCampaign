using System;
using UnityEngine;

namespace HexMapPathfinding
{
    /// <summary>
    /// Default implementation of the rules for which cells can be crossed and the cost for doing so
    /// </summary>
    [Serializable]
    public class DefaultTraverser : ITraversable
    {
        [Header("Obstacles")]
        public bool blockedByWater = false;
        public bool blockedByWall = false;
        public bool blockedByCharacters = false;
        public int maximumAscendingStep = -1;
        public int maximumDescendingStep = -1;

        [Header("Cost")]
        public float roadCost = 1;
        public float riverCrossingCost = 1;
        public float uphillCost = 1;
        public float downhillCost = 1;
        public float defaultCost = 1;

        /// <summary>
        /// Default ruleset for a walking traverser 
        /// </summary>
        public static DefaultTraverser Walking()
        {
            DefaultTraverser traverser = new DefaultTraverser();
            traverser.blockedByWater = true;
            traverser.blockedByWall = true;
            traverser.blockedByCharacters = true;
            traverser.maximumAscendingStep = 1;
            traverser.maximumDescendingStep = 1;

            traverser.roadCost = 0.5f;
            traverser.riverCrossingCost = 2f;
            traverser.uphillCost = 2f;
            traverser.downhillCost = 1f;
            traverser.defaultCost = 1f;

            return traverser;
        }

        /// <summary>
        /// Default ruleset for a ranged attack
        /// </summary>
        public static DefaultTraverser RangedAttack()
        {
            return new DefaultTraverser();
        }

        /// <summary>
        /// Calculates whether a traverser can move in the given direction from the given cell
        /// </summary>
        /// <param name="from">The cell that the traversing object is moving from</param>
        /// <param name="direction">The direction that the traversing object is moving in</param>
        /// <returns>True if the traversing object is able to move in the given direction from the given cell</returns>
        public virtual bool IsTraversable(HexCell cell, HexDirection direction)
        {
            HexCell neighbour = cell.GetNeighbor(direction);

            // If blockable by other characters
            if (blockedByCharacters)
            {
                // Check if there is a character in the next cell
                if (neighbour.Occupant != null)
                    return false;
            }

            // If blockable by water...
            if (blockedByWater)
            {
                // Check if there is water in the way
                if (neighbour.IsUnderwater)
                    return false;
            }

            // If blockable by walls...
            if (blockedByWall)
            {
                // Check if there is a wall in the way
                if (cell.BorderWall(direction) && !cell.HasRoadThroughEdge(direction))
                    return false;
            }

            // Is there a cliff?
            int elevationDifference = neighbour.Elevation - cell.Elevation;
            if (elevationDifference > 0)                            // Ascending cliff
            {
                if (maximumAscendingStep > -1)                      // If blockable by ascending cliffs...
                {
                    if (elevationDifference > maximumAscendingStep) // Check if ascending height is traversable
                        return false;
                }
            }

            else if (elevationDifference < 0)                                   // Descending cliff
            {
                if (maximumDescendingStep > -1)                                 // If blockable by descending cliffs...
                {
                    if (Mathf.Abs(elevationDifference) > maximumDescendingStep) // Check if descending height is traversable
                        return false;
                }
            }

            // Not blocked
            return true;
        }

        /// <summary>
        /// Calculates the cost of moving in the given direction from the given cell
        /// </summary>
        /// <param name="from">The cell that the traversing object is moving from</param>
        /// <param name="direction">The direction that the traversing object is moving in</param>
        /// <returns>The cost of moving from the given cell in the given direction</returns>
        public virtual float Cost(HexCell cell, HexDirection direction)
        {
            HexCell neighbour = cell.GetNeighbor(direction);

            // Moving along road
            if (cell.HasRoadThroughEdge(direction))
                return roadCost;

            // Moving uphill
            else if (neighbour.Elevation - cell.Elevation > 0)
                return uphillCost;

            // Moving downhill
            else if (neighbour.Elevation - cell.Elevation < 0)
                return downhillCost;

            // Crossing a river
            else if (CrossesRiver(cell, direction))
                return riverCrossingCost;

            // Default move cost
            else
                return defaultCost;
        }

        // TODO: Special case: entering a river hex and leaving it by the same direction it was entered by should not count
        // as crossing the river - need to consider third cell
        protected virtual bool CrossesRiver(HexCell cell, HexDirection direction)
        {
            // No need to cross river if there isn't one, or if can go around (in case of a river end), or there is a bridge
            if (cell.HasRiver && !cell.HasRiverBeginOrEnd && !cell.HasRoadThroughEdge(direction))
            {
                // For each neighbour of current cell
                Array directionValues = Enum.GetValues(typeof(HexDirection));
                for (int i = 0; i < directionValues.Length; i++)
                {
                    // Direction of adjacent cell
                    HexDirection adjacentDirection = (HexDirection)directionValues.GetValue(i);

                    // Except for when moving along river
                    if (adjacentDirection != direction || adjacentDirection != direction.Opposite())
                    {
                        // If there is a river crossing path, we must cross it
                        if (cell.HasRiverThroughEdge(adjacentDirection))
                            return true;
                    }
                }
            }

            // No river crossing
            return false;
        }
    }
}