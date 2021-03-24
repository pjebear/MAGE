using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.Events;

namespace MAGE.UI.Views
{
    class VendorView
        : UIContainer
    {
        public enum ComponentId
        {
            Exit,
            BuyBtn,
            SellBtn,
            ItemBtns,
        }

        public class DataProvider : IDataProvider
        {
            public List<string> ItemNames = new List<string>();
            public List<string> ItemIconAssetNames = new List<string>();
            public List<int> ItemValues = new List<int>();
            public List<bool> ItemSelectability = new List<bool>();
            public Optional<int> Currency;
        }

        public UIButton ExitBtn;
        public UIButton BuyBtn;
        public UIButton SellBtn;
        public IconTextBase CurrencyIconTxt;
        public UIList ItemsList;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            // OptionList
            UIList.DataProvider itemsDP = new UIList.DataProvider();
            for (int i = 0; i < dp.ItemNames.Count; ++i)
            {
                IconTextBase.DataProvider itemDP = new IconTextBase.DataProvider();
                itemDP.Text = dp.ItemNames[i];
                itemDP.IconAssetName = dp.ItemIconAssetNames[i];
                itemsDP.Elements.Add(itemDP);
            }

            ItemsList.Publish(itemsDP);

            // Currency
            if (dp.Currency.HasValue) CurrencyIconTxt.Publish(new IconTextBase.DataProvider() { Text = dp.Currency.Value.ToString() });
        }

        protected override void InitChildren()
        {
            ExitBtn.Init((int)ComponentId.Exit, this);
            BuyBtn.Init((int)ComponentId.BuyBtn, this);
            SellBtn.Init((int)ComponentId.SellBtn, this);
            ItemsList.Init((int)ComponentId.ItemBtns, this);
        }
    }
}
