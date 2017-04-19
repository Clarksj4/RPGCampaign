using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class TransformExtension
{
    public static void LookAt(this Transform transform, HexDirection direction)
    {
        transform.rotation = Quaternion.identity;
        transform.Rotate(transform.up, 30 + 60 * (int)direction);
    }
}
