using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashboardBoostBar : MonoBehaviour {

    private RectTransform rect;
    private Image image;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void SetSize(float sizeNormalized) {
        rect.localScale = new Vector3(sizeNormalized, 1f);
    }

    public void SetColor(Color color) {
        image.color = color;
    }
}
