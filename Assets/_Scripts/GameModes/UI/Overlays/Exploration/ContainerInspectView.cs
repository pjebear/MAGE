using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.Events;

namespace MAGE.UI.Views
{
    class ContainerInspectView
        : UIContainer
    {
        public enum ComponentId
        {
            Exit,
            ItemBtns,
        }

        public class DataProvider : IDataProvider
        {
            public List<string> ItemNames = new List<string>();
            public List<string> ItemIconAssetNames = new List<string>();
        }

        public UIButton ExitBtn;
        public UIList ItemsList;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            // OptionList
            UIList.DataProvider itemsDP = new UIList.DataProvider();
            foreach (string itemName in dp.ItemNames)
            {
                itemsDP.Elements.Add(new UIButton.DataProvider(itemName, true));
            }
            ItemsList.Publish(itemsDP);
        }

        protected override void InitChildren()
        {
            ExitBtn.Init((int)ComponentId.Exit, this);
            ItemsList.Init((int)ComponentId.ItemBtns, this);
        }
    }
}
