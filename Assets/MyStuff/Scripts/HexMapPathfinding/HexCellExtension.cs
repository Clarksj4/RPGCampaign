using System;
using System.Linq;
using UnityEngine;

public static class HexCellExtension
{
    public static Vector3[] GetCorners(this HexCell cell)
    {
        Vector3[] corners = new Vector3[6];
        for (int i = 0; i < corners.Length; i++)
            corners[i] = cell.Position + HexMetrics.corners[i];

        return corners;
    }

    public static HexCell[] GetNeighbours(this HexCell cell)
    {
        HexCell[] neighbours = new HexCell[6];

        Array directionValues = Enum.GetValues(typeof(HexDirection));
        for (int i = 0; i < directionValues.Length; i++)
        {
            HexDirection direction = (HexDirection)directionValues.GetValue(i);
            neighbours[i] = cell.GetNeighbor(direction);
        }

        return neighbours;
    }

    public static bool IsNeighbour(this HexCell cell, HexCell other)
    {
        return cell.GetNeighbours().Contains(other);
    }
}
