using MAGE.GameModes.Encounter;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
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

        private ActionProposal mCurrentAction = null;

        public void Init()
        {
            Messaging.MessageRouter.Instance.RegisterHandler(this);
        }

        private void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterStatusView);
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
                        Messaging.MessageRouter.Instance.NotifyMessage(new FlowMessage(FlowMessage.EventType.FlowEvent, FlowAction.advance.ToString()));
                    }
                    else if (interactionInfo.ComponentId == (int)EncounterStatus.ComponentId.WinBtn
                        && interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        EncounterFlowControl.Model.EncounterState = EncounterState.Win;
                        Messaging.MessageRouter.Instance.NotifyMessage(new FlowMessage(FlowMessage.EventType.FlowEvent, FlowAction.advance.ToString()));
                    }
                    if (interactionInfo.ComponentId == (int)EncounterStatus.ComponentId.LoseBtn
                        && interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        EncounterFlowControl.Model.EncounterState = EncounterState.Defeat;
                        Messaging.MessageRouter.Instance.NotifyMessage(new FlowMessage(FlowMessage.EventType.FlowEvent, FlowAction.advance.ToString()));
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
                        case (EncounterMessage.EventType.ActionResolutionBegin):
                            mCurrentAction = message.Arg<ActionProposal>();
                            UIManager.Instance.Publish(UIContainerId.EncounterStatusView);
                            break;
                        case (EncounterMessage.EventType.ActionResolved):
                            mCurrentAction = null;
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
            foreach (Character character in EncounterFlowControl.Model.PendingCharacterTurns)
            {
                turnOrder.Add(character.Name);
            }
            dp.TurnOrder = turnOrder;

            dp.CurrentTurn = EncounterFlowControl.Model.Clock;

            if (EncounterFlowControl.Model.CurrrentTurnCharacter != null)
            {
                Character currentTurn = EncounterFlowControl.Model.CurrrentTurnCharacter;
                dp.ActorName = currentTurn.Name;
                dp.ActorHealth = currentTurn.CurrentResources[ResourceType.Health].Current.ToString() + "/" + currentTurn.CurrentResources[ResourceType.Health].Max.ToString();
                dp.ActorOwner = currentTurn.TeamSide.ToString();
                dp.ActorStatuses = new List<string>();
                foreach (StatusEffect effect in currentTurn.StatusEffects)
                {
                    dp.ActorStatuses.Value.Add(effect.ToString() + " x " + effect.StackCount);
                }
            }

            dp.CurrentState = EncounterFlowControl.Model.EncounterState;

            if (mCurrentAction != null)
            {
                dp.CurrentAction = mCurrentAction.Action.ToString();
            }

            return dp;
        }
    }


}

