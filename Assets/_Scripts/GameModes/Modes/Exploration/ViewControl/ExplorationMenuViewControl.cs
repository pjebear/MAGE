using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ExplorationMenuViewControl 
    : UIContainerControl
{
    public string Name()
    {
        return "ExplorationMenuViewControl";
    }

    public void Show()
    {
        UIManager.Instance.PostContainer(UIContainerId.ExplorationMenuView, this);
    }

    public void Hide()
    {
        UIManager.Instance.RemoveOverlay(UIContainerId.ExplorationMenuView);
    }

    public void HandleComponentInteraction(int containerId, IUIInteractionInfo interactionInfo)
    {
        switch (containerId)
        {
            case (int)UIContainerId.ExplorationMenuView:
                {
                    if (interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        switch (interactionInfo.ComponentId)
                        {
                            case (int)ExplorationMenuView.ComponentId.SaveBtn:
                                {
                                    GameSystemModule.Instance.Save();
                                }
                                break;

                            case (int)ExplorationMenuView.ComponentId.ExitBtn:
                                {
                                    GameModesModule.Instance.Quit();
                                }
                                break;
                            case (int)ExplorationMenuView.ComponentId.OutfiterBtn:
                                {
                                    GameModesModule.Instance.Outfit();
                                }
                                break;
                        }
                    }
                }
                break;
        }
    }

    public IDataProvider Publish()
    {
        return IDataProvider.Empty;
    }
}

