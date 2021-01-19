using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public static HealthBar instance { get; private set; }

    public Image Mask;

    public Image Avatar;

    public Text healthText;

    float originalSize;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originalSize = Mask.rectTransform.rect.width;
    }

    public void SetValue(int currentHealth, int maxHealth)
    {
        Mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * (currentHealth/ (float)maxHealth));
        healthText.text = currentHealth.ToString() + '/' + maxHealth.ToString();
    }

    public void SetAvatar(Sprite avatar)
    {
        Avatar.sprite = avatar;
    }
}
