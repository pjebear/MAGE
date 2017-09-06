using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPlacementPanel : MonoBehaviour {

    public Text UnitName;
    public Text UnitLevel;
    public Image UnitImage;

    private Color mUnhighlightColor;
    private Color mHighlightColor;

    private void Awake()
    {
        mUnhighlightColor = new Color(1, 1, 1, .5f);
        mHighlightColor = Color.white;
    }

    public void DisplayUnit(string name, int level, string imageId, bool canSelect = true)
    {
        Color highlightColor = canSelect ? mHighlightColor : mUnhighlightColor;
        UnitName.text = name;
        UnitName.color = highlightColor;
        UnitLevel.text = level.ToString();
        UnitLevel.color = highlightColor;
        UnitImage.sprite = Resources.Load<Sprite>(imageId);
        UnitImage.color = highlightColor;
    }
}
