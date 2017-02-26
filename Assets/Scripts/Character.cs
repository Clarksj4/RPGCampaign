using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character : MonoBehaviour
{

    public Range Air;
    public Range Earth;
    public Range Fire;
    public Range Water;

    public ElementBar AirMeter;
    public ElementBar EarthMeter;
    public ElementBar FireMeter;
    public ElementBar WaterMeter;

    public ElementType Element;

    private new Renderer renderer;

    public void Attack(Character other)
    {

    }

    private void Awake()
    {
        renderer = GetComponentInChildren<Renderer>();
        renderer.material.color = GameMetrics.Instance.Elements[(int)Element].Colour;
    }

    private void OnValidate()
    {
        AirMeter.Range.Min = Air.Min;
        AirMeter.Range.Max = Air.Max;
        AirMeter.Range.Current = Air.Current;
        AirMeter.Refresh();

        EarthMeter.Range.Min = Earth.Min;
        EarthMeter.Range.Max = Earth.Max;
        EarthMeter.Range.Current = Earth.Current;
        EarthMeter.Refresh();

        FireMeter.Range.Min = Fire.Min;
        FireMeter.Range.Max = Fire.Max;
        FireMeter.Range.Current = Fire.Current;
        FireMeter.Refresh();

        WaterMeter.Range.Min = Water.Min;
        WaterMeter.Range.Max = Water.Max;
        WaterMeter.Range.Current = Water.Current;
        WaterMeter.Refresh();

        if (renderer != null)
            renderer.material.color = GameMetrics.Instance.Elements[(int)Element].Colour;
    }
}
