using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FloatingText : MonoBehaviour
{
    public Vector2 Velocity = Vector2.up * 10;
    public float DisplayTime = 5;

    private Text TextComponent { get { return GetComponent<Text>(); } }
    private string Text
    {
        get { return GetComponent<Text>().text; }
        set { GetComponent<Text>().text = value; }
    }

    public void Display(string text, Color colour)
    {
        FloatingText instance = Instantiate(this, transform);
        instance.transform.localScale = Vector3.one;
        instance.Text = text;
        instance.TextComponent.color = colour;
        instance.gameObject.SetActive(true);
        instance.MoveAndDestroy();
    }

    private void MoveAndDestroy()
    {
        StartCoroutine(DoMoveAndDestroy());
    }

    IEnumerator DoMoveAndDestroy()
    {
        RectTransform rectTransfrom = GetComponent<RectTransform>();

        Color initialColour = TextComponent.color;
        Color fadeOut = new Color(initialColour.r, initialColour.g, initialColour.b, 0);

        float t = 0;
        while (t < DisplayTime)
        {
            Vector2 step = Velocity * Time.deltaTime;
            rectTransfrom.anchoredPosition += step;
            TextComponent.color = Color.Lerp(initialColour, fadeOut, t / DisplayTime);

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
