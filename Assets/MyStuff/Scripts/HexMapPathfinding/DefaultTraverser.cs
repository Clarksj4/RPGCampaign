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
        if (cell.HasRoadThroughEdge(direction))
            return 0f;

        return 1f;
    }
}
