using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPercentWidth : MonoBehaviour
{
    private RectTransform rect;
    private RectTransform parentRect;

    public float percent = 0.5f;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        parentRect = rect.parent.GetComponent<RectTransform>();

        var parentWidth = parentRect.rect.width;

        // rect.rect.Set(rect.rect.x,rect.rect.y, 50f, rect.rect.height);
        rect.sizeDelta = new Vector2(parentWidth * percent, 0f);
        
    }

}
