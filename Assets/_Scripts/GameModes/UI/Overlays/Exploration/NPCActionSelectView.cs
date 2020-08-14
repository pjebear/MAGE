using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.Events;

namespace MAGE.UI.Views
{
    class NPCActionSelectView
        : UIContainer
    {
        public enum ComponentId
        {
            Exit,
            ActionSelect,
            ConversationBtn,
            VendorBtn,

        }

        public class DataProvider : IDataProvider
        {
            public Optional<string> PortraitAssetName;
            public Optional<string> Name;
            public List<string> NPCActions;
        }

        public UIButton ExitBtn;
        public UIImage HeadImg;
        public UIText NameTxt;
        public UIList OptionsList;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            // Portrait
            if (dp.PortraitAssetName.HasValue) HeadImg.Publish(dp.PortraitAssetName.Value);

            // Name
            if (dp.Name.HasValue) NameTxt.Publish(dp.Name.Value);

            // OptionList
            UIList.DataProvider actionsDP = new UIList.DataProvider();
            foreach (string actorAction in dp.NPCActions)
            {
                actionsDP.Elements.Add(new UIButton.DataProvider(actorAction, true));
            }
            OptionsList.Publish(actionsDP);
        }

        protected override void InitChildren()
        {
            ExitBtn.Init((int)ComponentId.Exit, this);
            OptionsList.Init((int)ComponentId.ActionSelect, this);
        }
    }


}
