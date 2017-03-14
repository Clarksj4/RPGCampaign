using System.Collections;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [Header("Main Hovering Object")]
    public Transform Main;
    public float PingPongTime = 1;
    public float PingPongDistance = 0.1f;
    public float InitialDirection = 1f;

    [Header("Perihperal Hovering Objects")]
    public Transform[] Peripherals;
    public float PeripheralPingPongTime = 0.5f;
    public float PeripheralPingPongDistance = 0.1f;
    public float PeripheralInitialDirection = -1f;

    void Start()
    {
        StartCoroutine(DoHover(Main, InitialDirection, PingPongDistance, PingPongTime));

        foreach (Transform peripheral in Peripherals)
            StartCoroutine(DoHover(peripheral, PeripheralInitialDirection, PeripheralPingPongDistance, PeripheralPingPongTime));
    }

    IEnumerator DoHover(Transform hoverer, float startDirection, float distance, float time)
    {
        Vector3 start = hoverer.localPosition;
        float direction = startDirection;
        float speed = distance / time;

        while (true)
        {
            float move = speed * Time.deltaTime * direction;
            hoverer.localPosition += Vector3.up * move;

            if (Vector3.Distance(start, hoverer.localPosition) >= distance)
            {
                hoverer.localPosition = start + (Vector3.up * distance * direction);
                direction *= -1;
            }

            yield return null;
        }
    }
}
