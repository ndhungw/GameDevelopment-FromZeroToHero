using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public static HealthBar instance { get; private set; }

    public Image Mask;

    public Image Avatar;

    float originalSize;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originalSize = Mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {
        Mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }
}
