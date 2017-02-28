using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Element
{
    public string Name { get { return type.ToString(); } }
    public ElementType Type { get { return type; } }

    [SerializeField]
    protected ElementType type;
    public Color Colour;

    /// <summary>
    /// Damage modifiers applied to attacks from this element to each of the other elements.
    /// </summary>
    [Tooltip("Damage modifiers applied to attacks from this element to each of the other elements")]
    public float[] Strengths;

    /// <summary>
    /// Default size modifiers for each elemental meter when spec'ed in this element. 
    /// This element's largest meter will be of its own type. 
    /// The smallest meter will be the element that this element is strong against.
    /// The other two meters will be equal size and half that of the largest meter
    /// </summary>
    [Tooltip("Default modifiers for the size of each elemental meter")]
    public float[] DefaultMeterLevel;

    public float Vs(ElementType defender)
    {
        return Strengths[(int)defender];
    }
}
