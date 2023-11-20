using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeRatio : MonoBehaviour
{
    private RectTransform parent;
    private RectTransform rectTransform;
    void Start()
    {
        parent = transform.parent.parent.parent.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(parent.rect.height * 0.98392f, parent.rect.height);
    }
}
