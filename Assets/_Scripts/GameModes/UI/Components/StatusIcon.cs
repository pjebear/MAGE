using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class StatusIcon : UIComponentBase
    {
        public class DataProvider : IDataProvider
        {
            public Optional<string> AssetName;
            public Optional<bool> IsBeneficial;
            public Optional<int> Count;
        }

        public const int PERMANENT_COUNT = -1;
        public Image BackingImg;
        public UIImage IconImg;
        public UIText CountTxt;
        public GameObject CountContainer;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            if (dp.AssetName.HasValue) IconImg.Publish(dp.AssetName.Value);
            if (dp.Count.HasValue)
            {
                CountTxt.Publish(dp.Count.Value.ToString());
                CountContainer.gameObject.SetActive(dp.Count.Value != PERMANENT_COUNT);
            }
            if (dp.IsBeneficial.HasValue) BackingImg.color = dp.IsBeneficial.Value ? Color.green : Color.red;
        }
    }
}


