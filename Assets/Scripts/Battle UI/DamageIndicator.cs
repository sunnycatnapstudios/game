using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DamageIndicator : MonoBehaviour
{
    public float moveSpeed = 1f, fadeDuration = 1f;
    private TextMeshProUGUI textMesh;
    private Color textColor;
    public float t;
    Vector3 initialPosition;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textColor = textMesh.color;
        initialPosition = transform.position;
    }

    public void SetText(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        textColor = color;
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.unscaledDeltaTime;

        if (transform.position.y > initialPosition.y+25f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.5f, 0, 0), t*.5f);
            t+= Time.unscaledDeltaTime;
        } if (transform.localScale.y<=0)
        {
            t=0;
            Destroy(gameObject);
        }

        textMesh.characterSpacing += 100f * Time.unscaledDeltaTime;
        textMesh.color = textColor;
    }
}