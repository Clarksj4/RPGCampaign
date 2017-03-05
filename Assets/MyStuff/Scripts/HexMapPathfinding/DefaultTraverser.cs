using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DefaultTraverser : ITraverser
{
    public bool IsTraversable(HexCell cell, HexDirection direction)
    {
        bool isUnderWater = cell.IsUnderwater;
        bool isBlockedByWall = cell.BorderWall(direction) && !cell.HasRoadThroughEdge(direction);
        bool isCliff = cell.GetElevationDifference(direction) > 1;

        return (!isUnderWater &&
                !isBlockedByWall &&
                !isCliff);
    }

    public float TraverseCost(HexCell cell, HexDirection direction)
    {
        HexCell neighbour = cell.GetNeighbor(direction);

        if (cell.HasRoadThroughEdge(direction))
        {
            if (neighbour.Elevation - cell.Elevation == 1)
                return 0.5f;
            else
                return 0.25f;
        }

        else if (neighbour.Elevation - cell.Elevation == 1)
            return 2f;

        else if (CrossesRiver(cell, direction))
            return 2f;

        else return 1f;
    }

    public bool CrossesRiver(HexCell cell, HexDirection direction)
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
