using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WaterElementalTraversable : ITraverser
{
    public bool IsTraversable(HexCell cell, HexDirection direction)
    {
        bool isBlockedByWall = cell.BorderWall(direction) && !cell.HasRoadThroughEdge(direction);
        bool isCliff = cell.GetElevationDifference(direction) > 1;

        return (!isBlockedByWall &&
                !isCliff);
    }
}
