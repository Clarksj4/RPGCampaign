using System;
using UnityEngine;

[Serializable]
public class DefaultTraverser : ITraverser
{
    public bool blockedByWater = true;
    public bool blockedByWall = true;
    public bool blockedByCliff = true;

    public float roadCost = 0.25f;
    public float riverCrossingCost = 2f;
    public float uphillCost = 2f;
    public float defaultCost = 1f;

    public static DefaultTraverser Walking()
    {
        return new DefaultTraverser();
    }

    public static DefaultTraverser RangedAttack()
    {
        DefaultTraverser traverser = new DefaultTraverser();
        traverser.blockedByWater = false;
        traverser.blockedByWall = false;
        traverser.blockedByCliff = false;

        traverser.roadCost = 1;
        traverser.riverCrossingCost = 1;
        traverser.uphillCost = 1;
        traverser.defaultCost = 1;

        return traverser;
    }

    public virtual bool IsTraversable(HexCell cell, HexDirection direction)
    {
        HexCell neighbour = cell.GetNeighbor(direction);

        if (blockedByWater)
        {
            if (neighbour.IsUnderwater)
                return false;
        }

        if (blockedByWall)
        {
            if (cell.BorderWall(direction) && !cell.HasRoadThroughEdge(direction))
                return false;
        }

        if (blockedByCliff)
        {
            if (cell.GetElevationDifference(direction) > 1)
                return false;
        }

        return true;
    }

    public virtual float TraverseCost(HexCell cell, HexDirection direction)
    {
        HexCell neighbour = cell.GetNeighbor(direction);

        if (cell.HasRoadThroughEdge(direction))
            return roadCost;

        else if (neighbour.Elevation - cell.Elevation == 1)
            return uphillCost;

        else if (CrossesRiver(cell, direction))
            return riverCrossingCost;

        else return defaultCost;
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
