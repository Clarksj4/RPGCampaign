using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public GameObject TextPrefab;
    public float Speed = 10;
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
            transform.forward = Camera.main.transform.forward;
            Vector3 step = transform.up * Speed * Time.deltaTime;
            transform.Translate(step, Space.World);

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(transform.gameObject);
    }
}
