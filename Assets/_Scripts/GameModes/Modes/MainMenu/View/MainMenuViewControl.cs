using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.FlowControl
{
    class MainMenuViewControl : UIContainerControl
    {
        private List<string> SaveFiles = new List<string>();

        public void Start()
        {
            SaveFiles = MAGE.GameServices.WorldService.Get().GetSaveFiles();

            UIManager.Instance.PostContainer(UIContainerId.MainMenuView, this);
        }

        public void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.MainMenuView);
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch (containerId)
            {
                case (int)UIContainerId.MainMenuView:
                {
                    if (interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        switch (interactionInfo.ComponentId)
                        {
                            case (int)MainMenuView.ComponentId.NewGameBtn:
                            {
                                MAGE.GameServices.WorldService.Get().PrepareNewGame();
                                GameModesModule.Instance.Explore();
                            }
                            break;

                            case (int)MainMenuView.ComponentId.SaveFileBtns:
                            {
                                ListInteractionInfo buttonListInteractionInfo = interactionInfo as ListInteractionInfo;
                                string saveFileName = SaveFiles[buttonListInteractionInfo.ListIdx];

                                MAGE.GameServices.WorldService.Get().Load(saveFileName);
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
            return "MainMenuControl";
        }

        public IDataProvider Publish(int containerId)
        {
            MainMenuView.DataProvider dataProvider = new MainMenuView.DataProvider();

            dataProvider.SaveFiles = SaveFiles;

            return dataProvider;
        }
    }
}


