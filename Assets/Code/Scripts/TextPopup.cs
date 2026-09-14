using TMPro;
using UnityEngine;
using System;

public class TextPopup : MonoBehaviour
{
    [SerializeField] public TextMeshPro text;

    private float lifetime;
    private float fadeTime;
    private float floatSpeed;
    private Action onFinished;
    private Color startColor;

    public bool finished;

    public void Setup(
        Vector3 position,
        Color color,
        float size,
        float fade,
        float speed)
    {
        finished = false;

        text.color = color;
        text.fontSize = size;

        startColor = color;

        fadeTime = fade;
        floatSpeed = speed;
        lifetime = 0f;

        transform.position = position;
    }

    public void Tick()
    {
        lifetime += Time.deltaTime;

        float t = Mathf.Clamp01(lifetime / fadeTime);

        Color color = startColor;
        color.a = Mathf.Lerp(startColor.a, 0f, t);
        text.color = color;

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (lifetime >= fadeTime)
        {
            finished = true;
            ObjectPool.instance.ReturnObject(gameObject);
        }
    }
}
