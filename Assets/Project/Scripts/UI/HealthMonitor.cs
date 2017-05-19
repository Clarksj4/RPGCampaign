using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthMonitor : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float Percent;
    public Transform Fill;

    private Vector3 startLocalPosition;
    private Vector3 startScale;
    private float barLeft;

    private void Awake()
    {
        startLocalPosition = Fill.localPosition;
        startScale = Fill.lossyScale;
        barLeft = startLocalPosition.x - (startScale.x / 2);
    }

    IEnumerator Start()
    {
        while (true)
        {
            SetPercent(Random.Range(0f, 1f));

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetPercent(float percent)
    {
        Fill.localScale = new Vector3(percent, Fill.localScale.y, Fill.localScale.z);
        Fill.localPosition = new Vector3(barLeft + (Fill.lossyScale.x / 2), startLocalPosition.y, startLocalPosition.z);
        Percent = percent;
    }
}
