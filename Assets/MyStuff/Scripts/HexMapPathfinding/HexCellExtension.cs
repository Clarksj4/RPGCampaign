using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
