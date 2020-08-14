using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace MAGE.UI.Views
{
    class ConversationView
        : UIContainer
    {
        public enum ComponentId
        {
            ContinueBtn
        }

        public class DataProvider : IDataProvider
        {
            public Optional<string> PortraitAssetName;
            public Optional<string> Name;
            public Optional<string> Content;
        }

        public UIButton ContinueBtn;
        public UIImage HeadImg;
        public UIText NameTxt;
        public UIText ContentTxt;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            // Portrait
            if (dp.PortraitAssetName.HasValue) HeadImg.Publish(dp.PortraitAssetName.Value);

            // Name
            if (dp.Name.HasValue) NameTxt.Publish(dp.Name.Value);

            // Content
            if (dp.Content.HasValue) ContentTxt.Publish(dp.Content.Value);
        }

        protected override void InitChildren()
        {
            ContinueBtn.Init((int)ComponentId.ContinueBtn, this);
        }
    }


}
