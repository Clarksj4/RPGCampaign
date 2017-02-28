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

        Strengths = new float[4];
        Strengths[(int)ElementType.Air] = 0.25f;
        Strengths[(int)ElementType.Earth] = -0.25f;
        Strengths[(int)ElementType.Fire] = 0.5f;
        Strengths[(int)ElementType.Water] = 1f;

        DefaultMeterLevel = new float[4];
        DefaultMeterLevel[(int)ElementType.Air] = 0.5f;
        DefaultMeterLevel[(int)ElementType.Earth] = 1f;
        DefaultMeterLevel[(int)ElementType.Fire] = 0.5f;
        DefaultMeterLevel[(int)ElementType.Water] = 0.25f;
    }
}
