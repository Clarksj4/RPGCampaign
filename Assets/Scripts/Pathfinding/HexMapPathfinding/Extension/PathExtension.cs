using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;

public static class PathExtension
{
    /// <summary>
    /// Allows iteration over the position of each cell in the path
    /// </summary>
    /// <returns>The position of each cell in the path</returns>
    public static Vector3[] GetPoints(this Path path)
    {
        return path.Select(s => ((HexCell)s.Node).Position).ToArray();
    }
}
