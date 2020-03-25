using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class MainMenuViewControl : UIContainerControl
{
    private List<string> SaveFiles = new List<string>();

    public void Start()
    {
        SaveFiles = SaveLoadUtil.GetSaveFiles();

        UIManager.Instance.PostContainer(UIContainerId.MainMenuView, this);
    }

    public void Cleanup()
    {
        UIManager.Instance.RemoveOverlay(UIContainerId.MainMenuView);
    }

    public void HandleComponentInteraction(int containerId, IUIInteractionInfo interactionInfo)
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
                                    GameSystemModule.Instance.PrepareNewGame();
                                    GameModesModule.Instance.Explore();
                                }
                                break;

                            case (int)MainMenuView.ComponentId.SaveFileBtns:
                                {
                                    UIButtonList.ButtonListInteractionInfo buttonListInteractionInfo = interactionInfo as UIButtonList.ButtonListInteractionInfo;
                                    string saveFileName = SaveFiles[buttonListInteractionInfo.ButtonIdx];

                                    GameSystemModule.Instance.Load(saveFileName);
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

    public IDataProvider Publish()
    {
        MainMenuView.DataProvider dataProvider = new MainMenuView.DataProvider();

        dataProvider.SaveFiles = SaveFiles;

        return dataProvider;
    }
}

