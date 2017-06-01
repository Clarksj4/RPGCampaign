using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public HexGridTraverser Traverser = HexGridTraverser.Walking();

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

    /// <summary>
    /// Reduces currentHP by the given amount. Returns true if currentHP is above zero after the reduction.
    /// </summary>
    /// <param name="amount">The amount to reduce HP by.</param>
    public bool ReceiveDamage(float amount)
    {
        bool alive = true;

        // TODO: damage calculation including armour and such things
        float change = -amount;

        currentHP -= amount;
        HealthChanged.Invoke(this, change);

        // Still have some HP remaining?
        if (currentHP <= 0)
            alive = false;

        return alive;
    }
}
