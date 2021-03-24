using MAGE.GameSystems.World;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.FlowControl
{
    class MapViewControl 
        : FlowControlBase
        , UIContainerControl
    {
        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Map;
        }

        protected override void Setup()
        {
            UIManager.Instance.PostContainer(UIContainerId.MapView, this);
        }

        protected override void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.MapView);
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch (containerId)
            {
                case (int)UIContainerId.MapView:
                {
                    if (interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        switch (interactionInfo.ComponentId)
                        {
                            case (int)MapView.ComponentId.ForestLevelBtn:
                            {
                                MAGE.GameSystems.WorldService.Get().UpdatePartyLocation(new PartyLocation() { Level = GameSystems.LevelId.Forest, SpawnPoint = 0 });
                                SendFlowMessage("back");
                            }
                            break;

                            case (int)MapView.ComponentId.DemoLevelBtn:
                            {
                                MAGE.GameSystems.WorldService.Get().UpdatePartyLocation(new PartyLocation() { Level = GameSystems.LevelId.Demo, SpawnPoint = 0 });
                                SendFlowMessage("back");
                            }
                            break;
                        }
                    }
                }
                break;
            }
        }

        public string Name()
        {
            return "MapViewControl";
        }

        public IDataProvider Publish(int containerId)
        {
            MapView.DataProvider dataProvider = new MapView.DataProvider();

            PartyLocation partyLocation = MAGE.GameSystems.WorldService.Get().GetPartyLocation();
            dataProvider.IsForestCurrentLocation = partyLocation.Level == GameSystems.LevelId.Forest;

            return dataProvider;
        }
    }
}


