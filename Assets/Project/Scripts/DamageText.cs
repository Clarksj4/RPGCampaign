using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public GameObject TextPrefab;
    public Vector3 Movement = Vector3.one;
    public float VerticalOffset = 5;
    public float DisplayTime = 5;

    public void Display(string text)
    {
        GameObject instance = Instantiate(TextPrefab, transform.position + (Vector3.up * VerticalOffset), TextPrefab.transform.rotation, transform);
        instance.GetComponent<TextMesh>().text = text;
        StartCoroutine(MoveAndDestroy(instance.transform));
    }

    IEnumerator MoveAndDestroy(Transform transform)
    {
        float t = 0;
        while (t < DisplayTime)
        {
            Vector3 step = Movement * Time.deltaTime;
            transform.Translate(step);

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(transform.gameObject);
    }
}
