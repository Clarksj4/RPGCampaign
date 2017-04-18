using System;
using UnityEngine;

public partial class HexCell
{
    public Character Occupant { get; set; }

    /// <summary>
    /// Returns the positions of each corner of this hex in world space
    /// </summary>
    public Vector3[] GetCorners()
    {
        Vector3[] corners = new Vector3[6];
        for (int i = 0; i < corners.Length; i++)
            corners[i] = Position + HexMetrics.corners[i];

        return corners;
    }
}
