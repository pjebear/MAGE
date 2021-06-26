using MAGE.GameModes.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.UI.Views
{
    class EncounterTurnOrderView : UIContainer
    {
        public UIGrid UIGrid;

        public enum ComponentId
        {
            OrderList
        }

        public class DataProvider : IDataProvider
        {
            public List<TurnOrderCharacterPanel.DataProvider> TurnOrder = new List<TurnOrderCharacterPanel.DataProvider>();
        }

        public override void Init(int id, UIContainer container)
        {
            base.Init(id, container);

            UIGrid = GetComponentInChildren<UIGrid>();
        }

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            UIGrid.DataProvider gridDP = new UIGrid.DataProvider();
            for (int i =0; i < dp.TurnOrder.Count; ++i)
            {
                gridDP.Elements.Add(dp.TurnOrder[i]);
            }

            UIGrid.Publish(gridDP);
        }

        protected override void InitChildren()
        {
            UIGrid.Init((int)ComponentId.OrderList, this);
        }
    }
}

