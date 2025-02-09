using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DamageIndicator : MonoBehaviour
{
    public float moveSpeed = 1f, fadeDuration = 1f;
    private TextMeshProUGUI textMesh;
    private Color textColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textColor = textMesh.color;
    }

    public void SetText(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        textColor = color;
        Destroy(gameObject, fadeDuration);
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.unscaledDeltaTime;
        textColor.a -= Time.unscaledDeltaTime / fadeDuration;
        textMesh.color = textColor;
    }
}