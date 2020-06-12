using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class UIImage : UIComponentBase
{
    public Image Image;

    public void Publish(string assetName)
    {
        Image.sprite = UIManager.Instance.LoadSprite(assetName);
    }

    public override void Publish(IDataProvider dataProvider)
    {
        // TODO:
    }
}
