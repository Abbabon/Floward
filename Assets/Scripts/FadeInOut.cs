using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut  : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private CanvasGroup _canvasGroup;
    public bool turnedOn;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeIn()
    {
        if (!turnedOn)
        {
            turnedOn = true;
            StartCoroutine(FadeTo(1f, 0.5f));
        }
    }

    public void FadeOut()
    {
        if (turnedOn)
        {
            turnedOn = false;
            StartCoroutine(FadeTo(-0.5f, 0.5f));
        }
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        Color color = _spriteRenderer != null ? _spriteRenderer.color : Color.white;
        float alpha = _spriteRenderer != null ? _spriteRenderer.color.a : _canvasGroup.alpha;
        
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = new Color(color.r, color.g, color.b, Mathf.Lerp(alpha, aValue, t));
            }
            else
            {
                _canvasGroup.alpha = Mathf.Lerp(alpha, aValue, t);
            }
            
            yield return null;
        }
    }
}
