using System;
using UnityEngine;

[Serializable]
public class Range
{
    public float Min;
    public float Current;
    public float Max;

    public bool Contains(float x)
    {
        return Contains(Min, Max, x);
    }

    public float Size()
    {
        return Size(Min, Max);
    }

    public static bool Contains(float min, float max, float x)
    {
        return min <= x && x <= max;
    }

    public static float Size(float min, float max)
    {
        return Math.Abs(min - max);
    }
}
