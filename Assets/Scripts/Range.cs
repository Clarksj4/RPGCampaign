using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Range
{
    public float Min
    {
        get { return min; }
        set
        {
            min = value;
            if (min > Max) throw new ArgumentException("Minimum range bound cannot exceed maximum");
            if (current < min) current = min;
        }
    }

    public float Current
    {
        get { return current; }
        set
        {
            current = value;
            if (!Contains(current)) throw new ArgumentException("Current value is outside of range");
        }
    }

    public float Max
    {
        get { return max; }
        set
        {
            max = value;
            if (min > Max) throw new ArgumentException("Maximum range bound cannot be less than minimum");
            if (current > max) current = max;
        }
    }

    [SerializeField]
    [HideInInspector]
    private float min;
    [SerializeField]
    [HideInInspector]
    private float current;
    [SerializeField]
    [HideInInspector]
    private float max;

    public bool Contains(float x)
    {
        return Contains(min, max, x);
    }

    public float Size()
    {
        return Size(min, max);
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
