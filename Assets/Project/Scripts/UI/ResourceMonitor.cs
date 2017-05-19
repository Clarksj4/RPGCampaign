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

    public void UpdateHealthLevel(object sender, float changeInUnits)
    {
        Stats stats = (Stats)sender;

        HealthBar.Level = stats.CurrentHP / stats.MaxHP;

        Color colour = changeInUnits < 0 ? DamageColour : HealColour;
        Text.Display(changeInUnits.ToString(), colour);
    }

    public void UpdateTimeUnitsLevel(object sender, float changeInUnits)
    {
        Stats stats = (Stats)sender;

        TimeUnitsBar.Level = stats.CurrentTimeUnits / stats.MaxTimeUnits;

        Text.Display(changeInUnits.ToString(), TimeUnitsColour);
    }
}
