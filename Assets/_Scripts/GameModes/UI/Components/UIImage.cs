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
        }

        public void Publish(string assetPath, string assetName)
        {
            Image.sprite = Instantiate(Resources.Load<Sprite>("UI/Sprites/" + assetPath + "/" + assetName));
        }

        public override void Publish(IDataProvider dataProvider)
        {
            // TODO:
        }
    }
}

