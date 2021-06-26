using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class UIImage : UIComponentBase
    {
        public Image Image;

        public void Publish(string assetName)
        {
            Image.sprite = UIManager.Instance.LoadSprite(assetName);
            Image.raycastTarget = IsClickable || IsHoverable;
        }

        public void Publish(string assetPath, string assetName)
        {
            Image.sprite = Instantiate(Resources.Load<Sprite>("UI/Sprites/" + assetPath + "/" + assetName));
            Image.raycastTarget = IsClickable || IsHoverable;
        }

        public override void Publish(IDataProvider dataProvider)
        {
            // TODO:
        }
    }
}

