using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class IconTextBase : UIContainer
    {
        enum ComponentId
        {
            Icon,
            Text,
            NUM
        }

        public class DataProvider : IDataProvider
        {
            public Optional<string> IconAssetName;
            public Optional<bool> DisplayIcon;
            public Optional<string> Text;
        }

        public UIImage IconImg;
        public UIText Text;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            if (dp.IconAssetName.HasValue) IconImg.Publish(dp.IconAssetName.Value);
            if (dp.DisplayIcon.HasValue) IconImg.gameObject.SetActive(dp.DisplayIcon.Value);
            if (dp.Text.HasValue) Text.Publish(dp.Text.Value.ToString());
        }

        protected override void InitChildren()
        {
            IconImg.Init((int)ComponentId.Icon, this);
            Text.Init((int)ComponentId.Text, this);
        }

        protected override UIInteractionInfo ModifyInteractionInfo(UIInteractionInfo interactionInfo)
        {
            return new UIInteractionInfo(mId, interactionInfo.InteractionType);
        }
    }
}


