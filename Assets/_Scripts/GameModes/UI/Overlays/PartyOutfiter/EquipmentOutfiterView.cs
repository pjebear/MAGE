using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.Events;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class EquipmentOutfiterView
        : UIContainer
    {
        public enum ComponentId
        {
            LeftHeldSlotBtn,
            WornSlotBtn,
            RightHeldSlotBtn,
            UnEquipBtn,

            EquipableSelectionBtns
        }

        public class EquipableDP
        {
            public string Name;
            public int Count;
            public bool CanEquip;
        }

        public class DataProvider : IDataProvider
        {
            public Optional<EquipableDP> LeftHeld;
            public bool LeftSelected;
            public Optional<EquipableDP> RightHeld;
            public bool RightSelected;
            public Optional<EquipableDP> Worn;
            public bool WornSelected;

            public bool CanUnequip;
            public List<EquipableDP> EquipableOptions;
        }

        public Image LeftHighlightImage;
        public UIButton LeftHeldBtn;
        public Image RightHighlightImage;
        public UIButton RightHeldBtn;
        public Image WornHighlightImage;
        public UIButton WornBtn;
        public UIButton UnEquipBtn;
        public UIList EquipableOptionsBtns;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = dataProvider as DataProvider;

            { // Left
                bool isSelectable = true;
                string text = "Empty";
                if (dp.LeftHeld.HasValue)
                {
                    text = dp.LeftHeld.Value.Name;
                }
                LeftHeldBtn.Publish(new UIButton.DataProvider(text, isSelectable));

                LeftHighlightImage.gameObject.SetActive(dp.LeftSelected);
            }

            { // Right
                bool isSelectable = true;
                string text = "Empty";
                if (dp.RightHeld.HasValue)
                {
                    text = dp.RightHeld.Value.Name;
                }
                RightHeldBtn.Publish(new UIButton.DataProvider(text, isSelectable));

                RightHighlightImage.gameObject.SetActive(dp.RightSelected);
            }

            { // Worn
                bool isSelectable = true;
                string text = "Empty";
                if (dp.Worn.HasValue)
                {
                    text = dp.Worn.Value.Name;
                }
                WornBtn.Publish(new UIButton.DataProvider(text, isSelectable));

                WornHighlightImage.gameObject.SetActive(dp.WornSelected);
            }

            { // Unequip
                UnEquipBtn.Publish(new UIButton.DataProvider("Unequip", dp.CanUnequip));
            }

            { // Equipable Options
                List<IDataProvider> btnListDP = new List<IDataProvider>();
                foreach (EquipableDP equipableDP in dp.EquipableOptions)
                {
                    string text = string.Format("[{0}] x {1}", equipableDP.Name, equipableDP.Count);
                    btnListDP.Add(new UIButton.DataProvider(text, equipableDP.CanEquip));
                }
                EquipableOptionsBtns.Publish(new UIList.DataProvider(btnListDP));
            }
        }

        protected override void InitChildren()
        {
            LeftHeldBtn.Init((int)ComponentId.LeftHeldSlotBtn, this);
            RightHeldBtn.Init((int)ComponentId.RightHeldSlotBtn, this);
            WornBtn.Init((int)ComponentId.WornSlotBtn, this);
            UnEquipBtn.Init((int)ComponentId.UnEquipBtn, this);
            EquipableOptionsBtns.Init((int)ComponentId.EquipableSelectionBtns, this);
        }
    }


}
