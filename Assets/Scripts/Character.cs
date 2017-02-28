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

    public void Refresh()
    {
        OnValidate();
    }

    private void Awake()
    {
        renderer = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        renderer.material.color = GameMetrics.Instance.Elements[(int)Element].Colour;
    }

    private void OnValidate()
    {
        if (AirMeter != null)
        {
            AirMeter.Min = Air.Min;
            AirMeter.Max = Air.Max;
            AirMeter.Current = Air.Current;
            AirMeter.Refresh();
        }

        if (EarthMeter != null)
        {
            EarthMeter.Min = Earth.Min;
            EarthMeter.Max = Earth.Max;
            EarthMeter.Current = Earth.Current;
            EarthMeter.Refresh();
        }

        if (FireMeter != null)
        {
            FireMeter.Min = Fire.Min;
            FireMeter.Max = Fire.Max;
            FireMeter.Current = Fire.Current;
            FireMeter.Refresh();
        }

        if (WaterMeter != null)
        {
            WaterMeter.Min = Water.Min;
            WaterMeter.Max = Water.Max;
            WaterMeter.Current = Water.Current;
            WaterMeter.Refresh();
        }

        if (renderer != null)
            renderer.material.color = GameMetrics.Instance.Elements[(int)Element].Colour;
    }
}
