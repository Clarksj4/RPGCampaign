using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementBar : MonoBehaviour
{
    public Slider Slider;
    public Character character;
    public ElementType element;

    public float Min
    {
        get { return Slider.minValue; }
        set { Slider.minValue = value; }
    }

    public float Current
    {
        get { return Slider.value; }
        set { Slider.value = value; }
    }

    public float Max
    {
        get { return Slider.maxValue; }
        set
        {
            Slider.maxValue = value;
            UpdateBarWidth();
        }
    }

    public float UnitWidth = 1;

    private void Awake()
    {
        character.ElementValueChanged += Character_ElementValueChanged;
        character.ElementCapacityChanged += Character_ElementCapacityChanged;
    }

    private void Character_ElementCapacityChanged(object sender, ElementMeterEventArgs e)
    {
        if (e.Type == element)
            Max = character.GetElementCapacity(e.Type) * UnitWidth;
    }

    private void Character_ElementValueChanged(object sender, ElementMeterEventArgs e)
    {
        if (e.Type == element)
            Current = character.GetElementValue(e.Type) * UnitWidth;
    }

    private void UpdateBarWidth()
    {
        RectTransform sliderRect = Slider.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(10 + Range.Size(Min, Max) * UnitWidth, sliderRect.sizeDelta.y);
    }
}
