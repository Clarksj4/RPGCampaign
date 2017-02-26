using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementBar : MonoBehaviour
{
    public Slider Slider;
    public RectTransform ZeroMark;

    public Range Range;
    public float UnitWidth = 1;

    public void Refresh()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        Slider.minValue = Range.Min;
        Slider.value = Range.Current;
        Slider.maxValue = Range.Max;

        RectTransform sliderRect = Slider.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(10 + Range.Size() * UnitWidth, sliderRect.sizeDelta.y);

        ZeroMark.anchoredPosition = new Vector2(-Range.Min * UnitWidth, 0);
    }
}
