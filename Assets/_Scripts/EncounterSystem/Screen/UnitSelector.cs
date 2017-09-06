using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelector : MonoBehaviour {

    private Text UnitName;
    private Image UnitImage;

    private void Awake()
    {
        UnitName = GetComponentInChildren<Text>();
        Debug.Assert(UnitName != null);
        UnitImage = GetComponent<Image>();
        Debug.Assert(UnitImage != null);
    }

    public void Initialize(string unitName, string imageAssetPath)
    {
        gameObject.SetActive(true);
        UnitName.text = unitName;
        UnitImage.sprite = Resources.Load<Sprite>(imageAssetPath);
    }
}
