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

        Strengths = new float[4];
        Strengths[(int)ElementType.Air] = -0.25f;
        Strengths[(int)ElementType.Earth] = 1f;
        Strengths[(int)ElementType.Fire] = 0.25f;
        Strengths[(int)ElementType.Water] = 0.5f;

        DefaultMeterLevel = new float[4];
        DefaultMeterLevel[(int)ElementType.Air] = 1f;
        DefaultMeterLevel[(int)ElementType.Earth] = 0.25f;
        DefaultMeterLevel[(int)ElementType.Fire] = 0.5f;
        DefaultMeterLevel[(int)ElementType.Water] = 0.5f;
    }
}
