using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Earth : Element
{
    public Earth()
    {
        type = ElementType.Earth;
        Colour = Color.green;
        Strengths = new float[]
        {
            0.25f,      // Air
            -0.25f,     // Earth
            0.5f,       // Fire
            1f          // Water
        };
    }
}
