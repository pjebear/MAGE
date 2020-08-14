
using MAGE.GameServices.Character;
using MAGE.GameServices.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class MasterFlowControl
        : MonoBehaviour
        , Messaging.IMessageHandler
    {
        private string TAG = "MasterFlowControl";
        // StatusEffect state
        private Dictionary<EncounterCharacter, List<StatusEffect>> StaleStatusEffects = new Dictionary<EncounterCharacter, List<StatusEffect>>();

        enum FlowState
        {
            Intro,
            UnitPlacement,
            //Init,

            CoreLoopBegin,
            StatusIncrement = CoreLoopBegin,
            StatusCheck,
            ActionIncrement,
            ActionResolution,
            TurnIncrement,
            TurnResolution,
            CoreLoopEnd,

            Outro,

            NUM
        }

        private bool mIsFlowPaused = true;
        private FlowState mState = FlowState.Intro;

        private void IncrementClock()
        {
            EncounterModule.Model.Clock++;
            Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.ClockProgressed));
        }

        public void Init()
        {
            Messaging.MessageRouter.Instance.RegisterHandler(this);
            mState = FlowState.Intro;

            mIsFlowPaused = true;
        }

        private void Start()
        {

        }

        private void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        private void Update()
        {
            if (!mIsFlowPaused)
            {
                RunFlowTick();
            }
        }

        private void RunFlowTick()
        {
            //if (Model.ChoreoEvents.Count > 0)
            //{
            //    pauseFlow = ProgressChoreo();
            //}
            if (CheckIsEncounterOver())
            {
                mIsFlowPaused = true;
                Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.EncounterOver));
            }
            else
            {
                switch (mState)
                {
                    case (FlowState.StatusIncrement):
                        EncounterModule.CharacterDirector.IncrementStatusEffects();

                        ProgressState();
                        break;

                    case (FlowState.StatusCheck):
                        EncounterModule.CharacterDirector.ApplyStatusEffects();
                        ProgressState();

                        break;

                    case (FlowState.ActionIncrement):
                        EncounterModule.ActionDirector.IncrementDelayedActions();
                        ProgressState();
                        break;

                    case (FlowState.ActionResolution):
                    {
                        if (EncounterModule.ActionDirector.HasReadyActions())
                        {
                            EncounterModule.ActionDirector.ProgressDelayedActions();
                            mIsFlowPaused = true;
                        }
                        else
                        {
                            ProgressState();
                        }
                    }
                    break;

                    case (FlowState.TurnIncrement):
                        foreach (EncounterCharacter actor in EncounterModule.Model.Characters.Values)
                        {
                            if (actor.IsAlive)
                            {
                                actor.DEBUG_ClockGuage += (int)actor.Attributes[new AttributeIndex(TertiaryStat.Speed)];
                                if (actor.DEBUG_ClockGuage >= 100)
                                {
                                    actor.DEBUG_HasActed = false;
                                    actor.DEBUG_HasMoved = false;
                                    actor.DEBUG_IsTurnComplete = false;
                                    EncounterModule.Model.TurnOrder.Add(actor);
                                }
                            }
                        }

                        ProgressState();
                        break;

                    case (FlowState.TurnResolution):

                        if (EncounterModule.Model.TurnOrder.Count > 0)
                        {
                            EncounterCharacter toProgress = EncounterModule.Model.TurnOrder[0];
                            if ((toProgress.DEBUG_HasActed && toProgress.DEBUG_HasMoved) || toProgress.DEBUG_IsTurnComplete)
                            {
                                EncounterCharacter turnComplete = EncounterModule.Model.TurnOrder[0];
                                int clockDecrement = 40;
                                if (toProgress.DEBUG_HasActed && toProgress.DEBUG_HasMoved)
                                {
                                    clockDecrement = 80;
                                }
                                else if (toProgress.DEBUG_HasActed || toProgress.DEBUG_HasMoved)
                                {
                                    clockDecrement = 60;
                                }
                                turnComplete.DEBUG_ClockGuage -= clockDecrement;

                                EncounterModule.Model.TurnOrder.RemoveAt(0);
                            }
                            else
                            {
                                mIsFlowPaused = true;
                                GetInputForActor(toProgress);
                            }
                        }
                        else
                        {
                            ProgressState();
                        }
                        break;

                    default:
                        mIsFlowPaused = true;
                        break;
                }
            }
        }

        private void ProgressFlow()
        {
            mIsFlowPaused = false;
        }

        private bool ProgressChoreo()
        {
            //// TODO
            //if (Model.ChoreoEvents.Count > 0)
            //{
            //    // TODO: Display Something
            //    Debug.Log(string.Format("Choreo Event: {0} ", Model.ChoreoEvents[0]));
            //    Model.ChoreoEvents.RemoveAt(0);
            //}
            // Don't pause flow;
            return false;
        }

        private bool CheckIsEncounterOver()
        {
            bool encounterOver = false;
            if (IsEncounterLost())
            {
                encounterOver = true;
                EncounterModule.Model.EncounterState = EncounterState.Defeat;
            }
            else if (IsEncounterWon())
            {
                encounterOver = true;
                EncounterModule.Model.EncounterState = EncounterState.Win;
            }

            return encounterOver;
        }

        private bool IsEncounterLost()
        {
            bool encounterLost = false;

            foreach (EncounterCondition loseCondition in EncounterModule.Model.EncounterContext.LoseConditions)
            {
                bool isConditionMet = loseCondition.IsConditionMet(EncounterModule.Model);

                if (isConditionMet)
                {
                    Logger.Log(LogTag.GameModes, TAG, string.Format("Loss condition met: {0} ", loseCondition.ToString()));
                }

                encounterLost |= isConditionMet;
            }

            return encounterLost;
        }

        private bool IsEncounterWon()
        {
            bool encounterWon = false;

            foreach (EncounterCondition winCondition in EncounterModule.Model.EncounterContext.WinConditions)
            {
                bool isConditionMet = winCondition.IsConditionMet(EncounterModule.Model);

                if (isConditionMet)
                {
                    Logger.Log(LogTag.GameModes, TAG, string.Format("Loss condition met: {0} ", winCondition.ToString()));
                }

                encounterWon |= isConditionMet;
            }

            return encounterWon;
        }

        private void ProgressState()
        {
            mState++;

            if (mState == FlowState.CoreLoopBegin)
            {
                ProgressFlow();
            }
            else if (mState == FlowState.CoreLoopEnd)
            {
                mState = FlowState.CoreLoopBegin;
                IncrementClock();
            }
        }

        private void GetInputForActor(EncounterCharacter actor)
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.TurnBegun, actor));
        }

        // EventHandler
        public void HandleMessage(Messaging.MessageInfoBase messageInfoBase)
        {
            switch (messageInfoBase.MessageId)
            {
                case EncounterMessage.Id:
                {
                    EncounterMessage message = messageInfoBase as EncounterMessage;

                    Logger.Log(LogTag.GameModes, TAG, "HandleMessage() - " + message.Type.ToString());

                    switch (message.Type)
                    {
                        case EncounterMessage.EventType.EncounterBegun:
                            if (ModulesContainer.Container.DebugEncounter)
                            {
                                ProgressState();
                                EncounterModule.UnitPlacementViewControl.Start();
                            }
                            else
                            {
                                EncounterModule.IntroViewControl.Show();
                            }
                            break;

                        case EncounterMessage.EventType.IntroComplete:
                            EncounterModule.IntroViewControl.Hide();
                            ProgressState();
                            EncounterModule.UnitPlacementViewControl.Start();
                            break;

                        case EncounterMessage.EventType.UnitPlacementComplete:

                            EncounterModule.UnitPlacementViewControl.Cleanup();
                            ProgressState();

                            break;
                        case EncounterMessage.EventType.ActionResolved:
                            ProgressFlow();
                            break;

                        case EncounterMessage.EventType.MoveResolved:
                            ProgressFlow();
                            break;

                        case EncounterMessage.EventType.TurnFinished:
                        {
                            ProgressFlow();
                        }
                        break;

                        case EncounterMessage.EventType.CharacterKO:
                        {
                            EncounterCharacter kodActor = message.Arg<EncounterCharacter>();

                            EncounterModule.Model.TurnOrder.Remove(kodActor);
                        }
                        break;

                    }
                }
                break;
            }
        }
    }
}


