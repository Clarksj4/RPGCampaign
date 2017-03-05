using System;
using UnityEngine;

[Serializable]
public class Traverser
{
    public bool blockedByWater = true;
    public bool blockedByWall = true;
    public int maximumAscendingStep = 1;
    public int maximumDescendingStep = 1;

    public float roadCost = 0.25f;
    public float riverCrossingCost = 2f;
    public float uphillCost = 2f;
    public float defaultCost = 1f;

    public static Traverser Walking()
    {
        return new Traverser();
    }

    public static Traverser RangedAttack()
    {
        Traverser traverser = new Traverser();
        traverser.blockedByWater = false;
        traverser.blockedByWall = false;
        traverser.maximumAscendingStep = -1;
        traverser.maximumDescendingStep = -1;

        traverser.roadCost = 1;
        traverser.riverCrossingCost = 1;
        traverser.uphillCost = 1;
        traverser.defaultCost = 1;

        return traverser;
    }

    public virtual bool IsTraversable(HexCell cell, HexDirection direction)
    {
        HexCell neighbour = cell.GetNeighbor(direction);

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

    public virtual float TraverseCost(HexCell cell, HexDirection direction)
    {
        HexCell neighbour = cell.GetNeighbor(direction);

        // Moving along road
        if (cell.HasRoadThroughEdge(direction))
            return roadCost;

        // Moving uphill
        else if (neighbour.Elevation - cell.Elevation == 1)
            return uphillCost;

        // Crossing a river
        else if (CrossesRiver(cell, direction))
            return riverCrossingCost;

        // Default move cost
        else
            return defaultCost;
    }

    protected virtual bool CrossesRiver(HexCell cell, HexDirection direction)
    {
        HexCell neighbour = cell.GetNeighbor(direction);

        // No need to cross river if there isn't one, or if can go around (in case of a river end), or there is a bridge
        if (neighbour.HasRiver && !neighbour.HasRiverBeginOrEnd && !neighbour.HasRoadThroughEdge(direction))
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
                    if (neighbour.HasRiverThroughEdge(adjacentDirection))
                        return true;
                }
            }
        }

        return false;   
    }
}
