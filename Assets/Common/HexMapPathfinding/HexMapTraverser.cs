using System;
using UnityEngine;
using Pathfinding;
using System.Linq;

/// <summary>
/// Default implementation of the rules for which cells can be crossed and the cost for doing so
/// </summary>
[Serializable]
public class HexMapTraverser : ITraversable
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
    public static HexMapTraverser Walking()
    {
        HexMapTraverser traverser = new HexMapTraverser();
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
    public static HexMapTraverser RangedAttack()
    {
        return new HexMapTraverser();
    }

    /// <summary>
    /// Calculates whether a traverser can move in the given direction from the given cell
    /// </summary>
    /// <param name="from">The cell that the traversing object is moving from</param>
    /// <param name="direction">The direction that the traversing object is moving in</param>
    /// <returns>True if the traversing object is able to move in the given direction from the given cell</returns>
    public bool IsTraversable(IPathNode cell, IPathNode neighbour)
    {
        HexCell hexCell = (HexCell)cell;
        HexCell hexNeighbour = (HexCell)neighbour;

        HexDirection direction = hexCell.GetDirection(hexNeighbour);

        // If blockable by other characters
        if (blockedByCharacters)
        {
            // Check if there is a character in the next cell
            if (hexNeighbour.Occupant != null)
                return false;
        }

        // If blockable by water...
        if (blockedByWater)
        {
            // Check if there is water in the way
            if (hexNeighbour.IsUnderwater)
                return false;
        }

        // If blockable by walls...
        if (blockedByWall)
        {
            // Check if there is a wall in the way
            if (BorderWall(hexCell, hexNeighbour) && !hexCell.HasRoadThroughEdge(direction))
                return false;
        }

        // Is there a cliff?
        int elevationDifference = hexNeighbour.Elevation - hexCell.Elevation;
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
    public float Cost(IPathNode cell, IPathNode neighbour)
    {
        HexCell hexCell = (HexCell)cell;
        HexCell hexNeighbour = (HexCell)neighbour;

        HexDirection direction = hexCell.GetDirection(hexNeighbour);

        // Moving along road
        if (hexCell.HasRoadThroughEdge(direction))
            return roadCost;

        // Moving uphill
        else if (hexNeighbour.Elevation - hexCell.Elevation > 0)
            return uphillCost;

        // Moving downhill
        else if (hexNeighbour.Elevation - hexCell.Elevation < 0)
            return downhillCost;

        // Crossing a river
        else if (CrossesRiver(hexCell, direction))
            return riverCrossingCost;

        // Default move cost
        else
            return defaultCost;
    }

    // TODO: Special case: entering a river hex and leaving it by the same direction it was entered by should not count
    // as crossing the river - need to consider third cell
    protected bool CrossesRiver(HexCell cell, HexDirection direction)
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

    /// <summary>
    /// Checks if there is a wall separating this cell from the cell in the given direction
    /// </summary>
    protected bool BorderWall(HexCell cell, HexCell neighbour)
    {
        return (cell.Walled && !neighbour.Walled) ||    // If this cell is walled and the neighbour isn't
                (!cell.Walled && neighbour.Walled);     // If the other cell is walled and this one isn't
    }
}
