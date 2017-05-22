using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using TileMap;

public partial class HexCell : ITile<Character>
{
    /// <summary>
    /// Adjacent cells that are not null, as nodes. Used for pathfinding.
    /// </summary>
    public IEnumerable<IGraphNode> Nodes { get { return GetNeighbours().Where(n => n != null).Select(n => (IGraphNode)n); } }

    /// <summary>
    /// The character occupying the cell
    /// </summary>
    public Character Contents { get; set; }

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

        throw new ArgumentException("HexCell is not a neighbour");
    }
}
