using MAGE.GameModes.Encounter;
using MAGE.GameServices.Character;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.FlowControl
{
    class EncounterStatusControl
        : MonoBehaviour,
        UIContainerControl,
        Messaging.IMessageHandler
    {
        private string Tag = "EncounterStatusControl";

        public EncounterCharacter mCurrentTurn = null;

        public void Init()
        {
            Messaging.MessageRouter.Instance.RegisterHandler(this);
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch (containerId)
            {
                case (int)UIContainerId.EncounterStatusView:
                {
                    if (interactionInfo.ComponentId == (int)EncounterStatus.ComponentId.ContinueBtn
                        && interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        GameModesModule.Instance.Explore();
                    }
                    else if (interactionInfo.ComponentId == (int)EncounterStatus.ComponentId.WinBtn
                        && interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        EncounterModule.Model.EncounterState = EncounterState.Win;
                        GameModesModule.Instance.Explore();
                    }
                    if (interactionInfo.ComponentId == (int)EncounterStatus.ComponentId.LoseBtn
                        && interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        EncounterModule.Model.EncounterState = EncounterState.Defeat;
                        GameModesModule.Instance.Explore();
                    }
                }
                break;
            }
        }

        public void HandleMessage(Messaging.MessageInfoBase messageInfoBase)
        {
            switch (messageInfoBase.MessageId)
            {
                case EncounterMessage.Id:
                {
                     EncounterMessage message = messageInfoBase as EncounterMessage;

                    switch (message.Type)
                    {
                        case (EncounterMessage.EventType.UnitPlacementComplete):
                            UIManager.Instance.PostContainer(UIContainerId.EncounterStatusView, this);
                            break;

                        case (EncounterMessage.EventType.TurnBegun):
                            mCurrentTurn = message.Arg<EncounterCharacter>();
                            UIManager.Instance.Publish(UIContainerId.EncounterStatusView);
                            break;

                        case (EncounterMessage.EventType.TurnFinished):
                            UIManager.Instance.Publish(UIContainerId.EncounterStatusView);
                            break;

                        case (EncounterMessage.EventType.ClockProgressed):
                            UIManager.Instance.Publish(UIContainerId.EncounterStatusView);
                            break;
                        case (EncounterMessage.EventType.EncounterOver):
                            UIManager.Instance.Publish(UIContainerId.EncounterStatusView);
                            break;
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


}

