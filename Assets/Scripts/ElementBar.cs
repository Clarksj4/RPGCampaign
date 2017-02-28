using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementBar : MonoBehaviour
{
    public Slider Slider;
    public RectTransform ZeroMark;

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
        set { Slider.maxValue = value; }
    }

    public float UnitWidth = 1;

    public void Refresh()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        RectTransform sliderRect = Slider.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(10 + Range.Size(Min, Max) * UnitWidth, sliderRect.sizeDelta.y);

        ZeroMark.anchoredPosition = new Vector2(-Min * UnitWidth, 0);
    }
}
