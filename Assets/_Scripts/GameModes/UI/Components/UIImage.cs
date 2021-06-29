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
            Sprite sprite = Resources.Load<Sprite>("UI/Sprites/" + assetPath + "/" + assetName);
            if (sprite == null)
            {
                sprite = Resources.Load<Sprite>("UI/Sprites/INVALID");
            }
            Image.sprite = Instantiate(sprite);
            Image.raycastTarget = IsClickable || IsHoverable;
        }

        public override void Publish(IDataProvider dataProvider)
        {
            // TODO:
        }
    }
}

