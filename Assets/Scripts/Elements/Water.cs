using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Water : Element
{
    public Water()
    {
        type = ElementType.Water;
        Colour = Color.blue;
        Strengths = new float[]
        {
            0.5f,       // Air
            0.25f,      // Earth
            1f,         // Fire
            -0.25f      // Water
        };
    }
}