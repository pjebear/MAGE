using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class EncounterIntroViewControl 
    : MonoBehaviour, 
    UIContainerControl
{
    private string Tag = "EncounterIntroViewControl";

    public void Init()
    {
        
    }

    public void Show()
    {
        UIManager.Instance.PostContainer(UIContainerId.EncounterIntroView, this);
    }

    public void Hide()
    {
        UIManager.Instance.RemoveOverlay(UIContainerId.EncounterIntroView);
    }

    public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
    {
       switch (containerId)
        {
            case (int)UIContainerId.EncounterIntroView:
            {
                if (interactionInfo.ComponentId == (int)EncounterIntroView.ComponentId.ContinueBtn
                    && interactionInfo.InteractionType == UIInteractionType.Click)
                {
                    EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.IntroComplete));
                }
            }
            break;
        }
    }

    public string Name()
    {
        return Tag;
    }

    public IDataProvider Publish(int containerId)
    {
        EncounterIntroView.DataProvider dp = new EncounterIntroView.DataProvider();

        EncounterContext context = EncounterModule.Model.EncounterContext;

        dp.EncounterType = context.EncounterType.ToString();
        dp.EncounterInfo = "";

        foreach (EncounterCondition encounterCondition in context.WinConditions)
        {
            dp.WinConditions.Add(encounterCondition.ToString());
        }

        foreach (EncounterCondition encounterCondition in context.LoseConditions)
        {
            dp.LoseConditions.Add(encounterCondition.ToString());
        }

        dp.Rewards.Add(context.CurrencyReward.ToString() + " gil");
        foreach (ItemId itemReward in context.ItemRewards)
        {
            dp.Rewards.Add(itemReward.ToString());
        }

        return dp;
    }
}

