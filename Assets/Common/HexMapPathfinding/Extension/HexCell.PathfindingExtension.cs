using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public partial class HexCell : IGraphNode
{
    /// <summary>
    /// Adjacent cells that are not null, as nodes. Used for pathfinding.
    /// </summary>
    public IEnumerable<IGraphNode> Nodes { get { return GetNeighbours().Where(n => n != null).Select(n => (IGraphNode)n); } }

    /// <summary>
    /// Gets the 6 cells adjacent to this cell. Null is returned where there is no cell in a direction
    /// </summary>
    public HexCell[] GetNeighbours()
    {
        // Six neighbours, or null where there is no neighbour
        HexCell[] neighbours = new HexCell[6];

        // Get cell in each direction
        Array directionValues = Enum.GetValues(typeof(HexDirection));
        for (int i = 0; i < directionValues.Length; i++)
        {
            // Get each direction, get neighbour in that direction
            HexDirection direction = (HexDirection)directionValues.GetValue(i);
            neighbours[i] = GetNeighbor(direction);
        }

        return neighbours;
    }

    /// <summary>
    /// Gets the direction that the other cell lies in from this cell.
    /// </summary>
    public HexDirection GetDirection(HexCell other)
    {
        // Check if other cell is a neighbouring cell
        int index = Array.IndexOf(neighbors, other);
        if (index > -1)
        {
            HexDirection[] directions = (HexDirection[])Enum.GetValues(typeof(HexDirection));
            return directions[index];
        }

        return GetDirection(other.Position);
    }

    /// <summary>
    /// Gets the direction that the point lies in from this cell
    /// </summary>
    public HexDirection GetDirection(Vector3 point)
    {
        throw new NotImplementedException("Method needs bug fixing!");

        HexDirection[] directions = (HexDirection[])Enum.GetValues(typeof(HexDirection));

        // Ignore cell height so that angle is based on x and z only
        Vector3 cellNoHeightPosition = Position;
        cellNoHeightPosition.y = 0;
        point.y = 0;

        // Angle from forward vector  converted to an index
        float angle = Vector3.Angle(Vector3.forward, point - cellNoHeightPosition);
        int index = (int)(angle / 60);

        HexDirection direction = directions[index];
        return direction;
    }
}
