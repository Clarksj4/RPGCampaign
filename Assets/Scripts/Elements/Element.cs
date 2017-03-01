using System;
using UnityEngine;

[Serializable]
public class Element
{
    /// <summary>
    /// The name of this element
    /// </summary>
    public string Name { get { return type.ToString(); } }

    /// <summary>
    /// The type of this element
    /// </summary>
    public ElementType Type { get { return type; } }

    [SerializeField]
    protected ElementType type;

    /// <summary>
    /// The colour associated with this element
    /// </summary>
    public Color Colour;

    /// <summary>
    /// Damage modifiers applied to attacks from this element to each of the other elements.
    /// </summary>
    [Tooltip("Damage modifiers applied to attacks from this element to each of the other elements")]
    public float[] StrengthModifiers;

    /// <summary>
    /// The capacity modifier for each element when spec'd in this element
    /// </summary>
    [Tooltip("The capacity modifier for each element when spec'd in this element")]
    public float[] ElementCapacityModifiers;

    /// <summary>
    /// Gets the damage modifier applied to attacks from this element versus the given element
    /// </summary>
    /// <param name="defender">The element that is being attacked</param>
    /// <returns>The percent damage modifier</returns>
    public float Vs(ElementType defender)
    {
        return StrengthModifiers[(int)defender];
    }
}
