using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Element
{
    public string Name { get { return type.ToString(); } }
    public ElementType Type { get { return type; } }

    [SerializeField]
    protected ElementType type;
    public Color Colour;
    public float[] Strengths;

    public float Vs(ElementType defender)
    {
        return Strengths[(int)defender];
    }
}
