using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class ActionSelector : UIContainer
    {
        private string TAG = "ActionSelector";

        public enum ComponentId
        {

        }

        public class DataProvider : IDataProvider
        {
            public string AssetName;
            public string Name;
            public string CastType;
            public string Cost;
            public bool HasResources;
        }

        public UIImage IconImg;
        public UIText NameTxt;
        public UIText CastTypeTxt;
        public UIText CostTxt;
        public Image UIMask;
        

        public override void Publish(IDataProvider dataProvider)
        {

            DataProvider dp = (DataProvider)dataProvider;

            // Icon Asset
            IconImg.Publish("Actions", dp.AssetName);

            // Name
            NameTxt.Publish(dp.Name);

            // Cast Type
            CastTypeTxt.Publish(dp.CastType);

            // Cost 
            CostTxt.Publish(dp.Cost);
            CostTxt.TMPro.color = dp.HasResources ? Color.white : Color.red;

            IsClickable = dp.HasResources;

            UIMask.raycastTarget = IsClickable || IsHoverable;
        }

        protected override void InitChildren()
        {
            // empty
        }
    }
}


