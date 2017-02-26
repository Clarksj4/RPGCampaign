using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Fire : Element
{
    public Fire()
    {
        type = ElementType.Fire;
        Colour = Color.red;
        Strengths = new float[]
        {
            1f,         // Air
            0.5f,       // Earth
            -0.25f,     // Fire
            0.25f       // Water
        };
    }
}