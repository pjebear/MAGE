using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class EncounterStatusControl 
    : MonoBehaviour, 
    UIContainerControl, 
    IEventHandler<EncounterEvent>
{
    private string Tag = "EncounterStatusControl";

    public EncounterCharacter mCurrentTurn = null;

    void Awake()
    {
        EncounterEventRouter.Instance.RegisterHandler(this);
    }

    public void HandleComponentInteraction(int containerId, IUIInteractionInfo interactionInfo)
    {
       switch (containerId)
        {
            case (int)UIContainerId.EncounterStatusOverlay:
                {
                    if (interactionInfo.ComponentId == (int)EncounterStatus.ComponentId.ContinueBtn
                        && interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        GameModesModule.Instance.Explore();
                    }
                }
                break;
        }
    }

    public void HandleEvent(EncounterEvent eventInfo)
    {
        switch (eventInfo.Type)
        {
            case (EncounterEvent.EventType.EncounterBegun):
                UIManager.Instance.PostContainer(UIContainerId.EncounterStatusOverlay, this);
                break;

            case (EncounterEvent.EventType.TurnBegun):
                mCurrentTurn = eventInfo.Arg<EncounterCharacter>();
                UIManager.Instance.Publish(UIContainerId.EncounterStatusOverlay);
                break;

            case (EncounterEvent.EventType.TurnFinished):
                UIManager.Instance.Publish(UIContainerId.EncounterStatusOverlay);
                break;

            case (EncounterEvent.EventType.ClockProgressed):
                UIManager.Instance.Publish(UIContainerId.EncounterStatusOverlay);
                break;
            case (EncounterEvent.EventType.EncounterOver):
                UIManager.Instance.Publish(UIContainerId.EncounterStatusOverlay);
                break;
        }
    }

    public string Name()
    {
        return Tag;
    }

    public IDataProvider Publish()
    {
        EncounterStatus.DataProvider dp = new EncounterStatus.DataProvider();

        List<string> turnOrder = new List<string>();
        foreach (EncounterCharacter actor in EncounterModule.Model.TurnOrder)
        {
            turnOrder.Add(actor.Name);
        }
        dp.TurnOrder = turnOrder;

        dp.CurrentTurn = EncounterModule.Model.Clock;

        if (mCurrentTurn != null)
        {
            dp.ActorName = mCurrentTurn.Name;
            dp.ActorHealth = mCurrentTurn.Resources[ResourceType.Health].Current.ToString() + "/" + mCurrentTurn.Resources[ResourceType.Health].Max.ToString();
            dp.ActorOwner = mCurrentTurn.Team.ToString();
            dp.ActorStatuses = new List<string>();
            foreach (StatusEffect effect in mCurrentTurn.StatusEffects)
            {
                dp.ActorStatuses.Value.Add(effect.ToString() + " x " + effect.StackCount);
            }
        }

        dp.CurrentState = EncounterModule.Model.EncounterState;

        return dp;
    }
}

