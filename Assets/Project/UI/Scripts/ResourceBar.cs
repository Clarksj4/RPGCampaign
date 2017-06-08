using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBar : MonoBehaviour
{
    public RectTransform Fill;
    public RectTransform ChangeFill;
    public float ChangeDuration = 1;

    private Coroutine levelChange;

    /// <summary>
    /// Resource level in the range 0 - 1
    /// </summary>
    public float Level
    {
        get { return level; }
        set { SetLevelClamped(value); }
    }
    private float level;

    /// <summary>
    /// Sets the resource bar level, clamped to the range 0 - 1
    /// </summary>
    public void SetLevelClamped(float newLevel)
    {
        // Level cannot exceed 0 - 1 
        float clamped = Mathf.Clamp01(newLevel);

        // Update GUI bar
        UpdateBar(level, clamped);

        level = clamped;
    }

    private void UpdateBar(float current, float updated)
    {
        // If level is decreasing, display gradual drop in bar level
        if (updated < current)
        {
            // Interrupt any current gradual drop effects
            if (levelChange != null)
            {
                // Hide change fill in case it was in the middle of gradual change
                ChangeFill.gameObject.SetActive(false);
                StopCoroutine(levelChange);
            }

            // Start again
            levelChange = StartCoroutine(DoLevelChange(current, updated));
        }

        // Otherwise, just set the amount
        else
            SetFill(updated);
    }

    /// <summary>
    /// Sets the resource level of the bar
    /// </summary>
    private void SetFill(float level)
    {
        Fill.localScale = new Vector3(level, Fill.localScale.y, Fill.localScale.z);
    }

    /// <summary>
    /// Sets the level of the change fill
    /// </summary>
    private void SetChangeFill(float level, float delta)
    { 
        ChangeFill.anchorMin = new Vector2(level, 0);
        ChangeFill.anchorMax = new Vector2(level + delta, 1);
    }

    IEnumerator DoLevelChange(float current, float next)
    {
        // Set fill to next
        SetFill(next);

        // Unhide the change fill
        ChangeFill.gameObject.SetActive(true);

        // Total change in level
        float totalDelta = current - next;

        // Gradual change over duration
        float t = 0;
        while (t < ChangeDuration)
        {
            // Level of change fill at current tick
            float currentDelta = Mathf.Lerp(totalDelta, 0, t / ChangeDuration);

            // Drain changeFill over time to next
            SetChangeFill(next, currentDelta);

            t += Time.deltaTime;
            yield return null;
        }

        // Hide change fill again
        ChangeFill.gameObject.SetActive(false);
    }
}
