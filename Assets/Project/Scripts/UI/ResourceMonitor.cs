using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceMonitor : MonoBehaviour
{
    public Color DamageColour;
    public Color HealColour;
    public Color TimeUnitsColour;

    public ResourceBar HealthBar;
    public ResourceBar TimeUnitsBar;
    public FloatingText Text;

    public void UpdateHealthLevel(float newPercent, float changeInUnits)
    {
        HealthBar.Level = newPercent;

        Color colour = changeInUnits < 0 ? DamageColour : HealColour;
        Text.Display(changeInUnits.ToString(), colour);
    }

    public void UpdateTimeUnitsLevel(float newPercent, float changeInUnits)
    {
        TimeUnitsBar.Level = newPercent;

        Text.Display(changeInUnits.ToString(), TimeUnitsColour);
    }
}
