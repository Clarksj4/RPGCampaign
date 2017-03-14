using System.Collections;
using UnityEngine;

public class RockBob : MonoBehaviour
{
    public Transform Rock;
    public Transform[] Pebbles;

    public float BobTime = 1;
    public float MaxBob = 0.1f;

    void Start()
    {
        StartCoroutine(DoBob(Rock, -1, MaxBob, BobTime));

        foreach (Transform pebble in Pebbles)
            StartCoroutine(DoBob(pebble, OneOrMinusOne(), Random.Range(MaxBob / 2, MaxBob), Random.Range(BobTime / 2, BobTime)));
    }

    IEnumerator DoBob(Transform rock, float startDirection, float distance, float time)
    {
        Vector3 start = rock.localPosition;
        float direction = startDirection;
        float speed = distance / time;

        while (true)
        {
            float move = speed * Time.deltaTime * direction;
            rock.localPosition += Vector3.up * move;

            if (Vector3.Distance(start, rock.localPosition) >= distance)
            {
                rock.localPosition = start + (Vector3.up * distance * direction);
                direction *= -1;
            }

            yield return null;
        }
    }

    float OneOrMinusOne()
    {
        return Random.value > 0.5f ? 1 : -1;
    }
}
