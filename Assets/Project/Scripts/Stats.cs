using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Stats : MonoBehaviour
{
    /// <summary>
    /// The current value of one of this character's elements has changed
    /// </summary>
    public event ElementMeterEventHandler ElementValueChanged;

    /// <summary>
    /// The capacity of one of this character's elements has changed
    /// </summary>
    public event ElementMeterEventHandler ElementCapacityChanged;

    public FloatEvent HealthChanged;
    public FloatEvent TimeUnitsChanged;

    public float CurrentHP;
    public float MaxHP;
    [Tooltip("The element that this character is spec'd in. Determines the capacity this character has for each of the elements, as well " +
             "as which elements are strong against this character.")]
    public ElementType Element;
    [Tooltip("Determines how quickly this character acts in combat")]
    public float Initiative = 25;
    [Tooltip("The speed at which this character moves")]
    public float Speed;
    [Tooltip("Which cells can be crossed by this character and the cost of doing so")]
    public HexMapTraverser Traverser = HexMapTraverser.Walking();
    [Header("Time Units")]
    public float CurrentTimeUnits;
    public float MaxTimeUnits;

    /// <summary>
    /// The capacity and current level of each of this characters elements
    /// </summary>
    public Range[] Elements { get; private set; }

    private void Awake()
    {
        Elements = new Range[4];
        for (int i = 0; i < Elements.Length; i++)
            Elements[i] = new Range();
    }

    public void TakeDamage(float damage)
    {
        CurrentHP -= damage;

        HealthChanged.Invoke(CurrentHP / MaxHP, -damage);
    }

    public void RefreshTimeUnits()
    {
        float deficit = MaxTimeUnits - CurrentTimeUnits;
        CurrentTimeUnits = MaxTimeUnits;

        TimeUnitsChanged.Invoke(1f, deficit);
    }

    public void SpendTimeUnits(float amount)
    {
        CurrentTimeUnits -= amount;

        TimeUnitsChanged.Invoke(CurrentTimeUnits / MaxTimeUnits, -amount);
    }

    /// <summary>
    /// Get this character's capacity for the given element
    /// </summary>
    public float GetElementCapacity(ElementType type)
    {
        return Elements[(int)type].Max;
    }

    /// <summary>
    /// Get this character's current level of the given element
    /// </summary>
    public float GetElementValue(ElementType type)
    {
        return Elements[(int)type].Current;
    }

    /// <summary>
    /// Set this character's capacity for the given element type
    /// </summary>
    public void SetElementCapacity(ElementType type, float capacity)
    {
        Elements[(int)type].Max = capacity;

        if (ElementCapacityChanged != null)
            ElementCapacityChanged(this, new ElementMeterEventArgs(type));
    }

    /// <summary>
    /// Set this character's current level of the given element type
    /// </summary>
    public void SetElementValue(ElementType type, float value)
    {
        Elements[(int)type].Current = value;

        if (ElementValueChanged != null)
            ElementValueChanged(this, new ElementMeterEventArgs(type));
    }
}
