using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class UIBar : UIComponentBase
    {
        public Image FillBarImg;
        public UIText NameTxt;
        public UIText FillTxt;

        public void Publish(string barName, int numerator, int denominator)
        {
            float fillRatio = numerator / (float)denominator;
            Vector3 currentScale = FillBarImg.rectTransform.localScale;
            currentScale.x = fillRatio;
            FillBarImg.rectTransform.localScale = currentScale;

            NameTxt.Publish(barName);
            FillTxt.Publish(string.Format("{0}/{1}", numerator, denominator));
        }

        public override void Publish(IDataProvider dataProvider)
        {
            // EMPTY
        }
    }
}



