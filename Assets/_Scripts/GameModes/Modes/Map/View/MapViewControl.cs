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
    class MapViewControl : UIContainerControl
    {
        public void Start()
        {
            UIManager.Instance.PostContainer(UIContainerId.MapView, this);
        }

        public void Cleanup()
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
                                GameModesModule.Instance.Explore();
                            }
                            break;

                            case (int)MapView.ComponentId.DemoLevelBtn:
                            {
                                MAGE.GameSystems.WorldService.Get().UpdatePartyLocation(new PartyLocation() { Level = GameSystems.LevelId.DemoLevel, SpawnPoint = 0 });
                                GameModesModule.Instance.Explore();
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


