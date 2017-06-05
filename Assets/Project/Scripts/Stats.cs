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
    [Tooltip("Affects the damage that this character receives when being attacked")]
    [SerializeField]
    private float defense = 1;

    [Header("Health")]
    [Tooltip("The amount of HP this character currently has. When this reaches zero, the character dies.")]
    [SerializeField]
    private float currentHP = 50;
    [Tooltip("The maximum amount this character's HP can reach")]
    [SerializeField]
    private float maxHP = 50;

    [Header("Time Units")]
    [Tooltip("The amount of time units this character currently has available to perform actions")]
    [SerializeField]
    private float currentTimeUnits = 10;
    [Tooltip("The maximum amount this characters Time Units can reach. The character's time units are refilled to this amount each turn")]
    [SerializeField]
    private float maxTimeUnits = 10;

    [Tooltip("Which cells can be crossed by this character and the cost of doing so")]
    public HexGridTraverser Traverser = HexGridTraverser.Walking();

    [Header("Events")]
    public ResourceEvent HealthChanged;
    public ResourceEvent TimeUnitsChanged;

    /// <summary>
    /// This character's current health points level. HP determines how much damage a character can receive before dying
    /// </summary>
    public float CurrentHP { get { return currentHP; } }

    /// <summary>
    /// This character's current time units level. TUs determine the amount of actions a character can complete in a turn
    /// </summary>
    public float CurrentTimeUnits { get { return currentTimeUnits; } }

    /// <summary>
    /// The maximum amount that this characters HP can reach
    /// </summary>
    public float MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    /// <summary>
    /// The maximum amount that this character's time units can reach. A character's time units are refilled to 
    /// this amount each turn
    /// </summary>
    public float MaxTimeUnits
    {
        get { return maxTimeUnits; }
        set { maxTimeUnits = value; }
    }

    /// <summary>
    /// Determines how quickly this character acts in combat
    /// </summary>
    public float Initiative
    {
        get { return initiative; }
        set { initiative = value; }
    }

    /// <summary>
    /// Affects the damage that this character receives when being attacked
    /// </summary>
    public float Defense
    {
        get { return defense; }
        set { defense = value; }
    }

    /// <summary>
    /// Refills this character's pool of available time units to its maximum amount.
    /// </summary>
    public void RefillTimeUnits()
    {
        // Increase by maximum amount, the actual level will be clamped to maximum
        GainTimeUnits(maxTimeUnits);
    }

    /// <summary>
    /// Refills this character's pool of available time units by the given amount. The time units pool
    /// cannot be filled to above this character's maximum time units.
    /// </summary>
    /// <param name="amount">The amount to increase this character's time units pool by</param>
    public void GainTimeUnits(float amount)
    {
        // Increase current time units, but not above maximum
        float clamped = Mathf.Min(currentTimeUnits + amount, maxTimeUnits);
        currentTimeUnits = clamped;

        // Notify listeners
        TimeUnitsChanged.Invoke(this, amount);
    }

    /// <summary>
    /// Reduces this character's pool of available time units by the given amount. The time units pool
    /// cannot be reduced to less than 0.
    /// </summary>
    /// <param name="amount">The amount to reduce this character's time units pool by</param>
    public void SpendTimeUnits(float amount)
    {
        // Reduce current time units, but not below 0
        float clamped = Mathf.Max(currentTimeUnits - amount, 0);
        currentTimeUnits = clamped;

        // Add sign to change in Time Units, and notify listeners
        float change = -amount;
        TimeUnitsChanged.Invoke(this, change);
    }

    /// <summary>
    /// Reduces currentHP by the given amount. Returns true if currentHP is above zero after the reduction.
    /// </summary>
    /// <param name="damage">The amount to reduce HP by.</param>
    public bool ReceiveDamage(float damage)
    {
        // Cannot receive less than zero damage
        float received = Mathf.Max(damage - defense, 0f);
        currentHP -= received;

        // Add sign to change in HP (same event is raised when HP is healed)
        float change = -received;
        HealthChanged.Invoke(this, change);

        // Check is character is still alive
        return IsAlive();
    }

    /// <summary>
    /// Is this character currently alive?
    /// </summary>
    public bool IsAlive()
    {
        return currentHP > 0;
    }
}
