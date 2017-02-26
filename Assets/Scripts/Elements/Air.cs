using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Air : Element
{
    public Air()
    {
        type = ElementType.Air;
        Colour = Color.yellow;
        Strengths = new float[4]
        {
            -0.25f,     // Air
            1f,         // Earth
            0.25f,      // Fire
            0.5f        // Water
        };
    }
}
