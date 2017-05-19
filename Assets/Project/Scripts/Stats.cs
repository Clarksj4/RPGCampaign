using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Stats : MonoBehaviour
{
    [Tooltip("The element that this character is spec'd in. Determines the capacity this character has for each of the elements, as well " +
         "as which elements are strong against this character.")]
    public ElementType Element;

    [Tooltip("Determines how quickly this character acts in combat")]
    [SerializeField]
    private float initiative = 25;

    [Header("Health")]
    [SerializeField]
    private float currentHP;
    [SerializeField]
    private float maxHP;

    [Header("Time Units")]
    [SerializeField]
    private float currentTimeUnits;
    [SerializeField]
    private float maxTimeUnits;

    [Tooltip("Which cells can be crossed by this character and the cost of doing so")]
    public HexMapTraverser Traverser = HexMapTraverser.Walking();

    [Header("Events")]
    public ResourceEvent HealthChanged;
    public ResourceEvent TimeUnitsChanged;

    public float CurrentHP
    {
        get { return currentHP; }
        set
        {
            float change = value - currentHP;
            currentHP = value;
            HealthChanged.Invoke(this, change);
        }
    }

    public float MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    public float CurrentTimeUnits
    {
        get { return currentTimeUnits; }
        set
        {
            float change = value - currentTimeUnits;
            currentTimeUnits = value;
            TimeUnitsChanged.Invoke(this, change);
        }
    }

    public float MaxTimeUnits
    {
        get { return maxTimeUnits; }
        set { maxTimeUnits = value; }
    }

    public float Initiative
    {
        get { return initiative; }
        set { initiative = value; }
    }
}
