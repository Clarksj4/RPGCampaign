using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class Character : MonoBehaviour
{
    /// <summary>
    /// The current value of one of this character's elements has changed
    /// </summary>
    public event ElementMeterEventHandler ElementValueChanged;

    /// <summary>
    /// The capacity of one of this character's elements has changed
    /// </summary>
    public event ElementMeterEventHandler ElementCapacityChanged;

    [Tooltip("The element that this character is spec'd in. Determines the capacity this character has for each of the elements, as well " +
        "as which elements are strong against this character.")]
    [SerializeField]
    private ElementType element;
    public ElementType Element
    {
        get { return element; }
        set
        {
            element = value;
            UpdateModelColour();
        }
    }

    /// <summary>
    /// The capacity and current level of each of this characters elements
    /// </summary>
    public Range[] Elements { get; private set; }

    private new Renderer renderer;

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

    private void Awake()
    {
        renderer = GetComponentInChildren<Renderer>();
    }

    private void OnValidate()
    {
        UpdateModelColour();
    }

    /// <summary>
    /// Update the character model to reflect its element type
    /// </summary>
    private void UpdateModelColour()
    {
        // null checks for when executed in edit mode
        if (renderer != null && GameMetrics.Instance != null)
            renderer.material.color = GameMetrics.Instance.Elements[(int)Element].Colour;
    }
}
