using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class StatusEffectIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Image IconImage;
    public Image IconSkrim;
    public Text StackText;
    public Text ToolTipText;
    public GameObject ToolTip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.SetActive(false);
    }

    private void Awake()
    {
        Debug.Assert(ToolTip != null);
        Debug.Assert(IconImage != null);
        Debug.Assert(IconSkrim != null);
        Debug.Assert(StackText != null);
        Debug.Assert(ToolTipText != null);
        ToolTip.SetActive(false);
    }

    public void Initialize(float iconSize)
    {
        IconImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);
        IconImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
    }

    public void SetIcon(Sprite iconSprite, string toolTipText, int numStacks, bool isBeneficial)
    {
        Color fadeColor = new Color(0, 0, 0, .5f);
        gameObject.SetActive(true);
        IconImage.sprite = iconSprite;
        IconSkrim.color = (isBeneficial ? Color.green : Color.red) - new Color(0, 0, 0, .5f); 
        ToolTipText.text = toolTipText;
        if (numStacks > 1)
        { 
            StackText.color = Color.white;
            StackText.text = (numStacks + 1).ToString();
        }
        else
        {
            StackText.color = Color.clear;
        }
    }
}
