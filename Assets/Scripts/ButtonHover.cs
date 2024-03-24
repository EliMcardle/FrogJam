using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite nonHoverSprite;
    [SerializeField] private Image buttonImage;
    private void Start()
    {
        buttonImage.sprite = nonHoverSprite;
    }

    public void HoverIn()
    {
        buttonImage.sprite = hoverSprite;
    }

    public void HoverOut()
    {
        buttonImage.sprite = nonHoverSprite;

    }
}
