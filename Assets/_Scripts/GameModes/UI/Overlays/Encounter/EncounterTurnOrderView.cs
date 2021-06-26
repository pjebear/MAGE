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
        private List<TurnOrderCharacterPanel> mTurnOrderPanels = new List<TurnOrderCharacterPanel>();
        public UIGrid UIGrid;

        public enum ComponentId
        {
            
        }

        public class DataProvider : IDataProvider
        {
            public List<TurnOrderCharacterPanel.DataProvider> TurnOrder = new List<TurnOrderCharacterPanel.DataProvider>();
        }

        public override void Init(int id, UIContainer container)
        {
            base.Init(id, container);

            UIGrid = GetComponentInChildren<UIGrid>();

            TurnOrderCharacterPanel turnOrderPanel = Resources.Load<TurnOrderCharacterPanel>("UI/Components/CustomComponents/TurnOrderCharacterPanel");
            for (int i = 0; i < 10; ++i)
            {
                TurnOrderCharacterPanel instantiated = Instantiate(turnOrderPanel, UIGrid.Grid.transform);
                instantiated.gameObject.SetActive(false);
                mTurnOrderPanels.Add(instantiated);
            }
        }

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            for (int i = 0; i < dp.TurnOrder.Count; ++i)
            {
                if (i >= mTurnOrderPanels.Count)
                {
                    mTurnOrderPanels.Add(Instantiate(Resources.Load<TurnOrderCharacterPanel>("UI/Components/CustomComponents/TurnOrderCharacterPanel"), UIGrid.transform));
                }

                mTurnOrderPanels[i].gameObject.SetActive(true);
                mTurnOrderPanels[i].Publish(dp.TurnOrder[i]);
            }

            for (int i = dp.TurnOrder.Count; i < mTurnOrderPanels.Count; ++i)
            {
                mTurnOrderPanels[i].gameObject.SetActive(false);
            }

        }

        protected override void InitChildren()
        {
            
        }
    }
}

