using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.FlowControl
{
    class MainMenuViewControl 
        : FlowControlBase
        , UIContainerControl
    {
        private List<string> SaveFiles = new List<string>();

        // FlowControl
        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.MainMenu;
        }

        protected override void Setup()
        {
            SaveFiles = MAGE.GameSystems.WorldService.Get().GetSaveFiles();

            UIManager.Instance.PostContainer(UIContainerId.MainMenuView, this);
        }

        protected override void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.MainMenuView);
        }

        // UIContainerControl
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
                                MAGE.GameSystems.WorldService.Get().PrepareNewGame();
                                SendFlowMessage(FlowAction.advance.ToString());
                            }
                            break;

                            case (int)MainMenuView.ComponentId.SaveFileBtns:
                            {
                                ListInteractionInfo buttonListInteractionInfo = interactionInfo as ListInteractionInfo;
                                string saveFileName = SaveFiles[buttonListInteractionInfo.ListIdx];

                                MAGE.GameSystems.WorldService.Get().Load(saveFileName);

                                SendFlowMessage(FlowAction.advance.ToString());
                            }
                            break;
                        }
                    }
                }
                break;
            }
        }

        public IDataProvider Publish(int containerId)
        {
            MainMenuView.DataProvider dataProvider = new MainMenuView.DataProvider();

            dataProvider.SaveFiles = SaveFiles;

            return dataProvider;
        }

        public string Name()
        {
            return "MainMenuViewControl";
        }
    }
}


