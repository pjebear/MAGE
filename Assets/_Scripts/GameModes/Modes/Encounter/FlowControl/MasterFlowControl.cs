
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.World;
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
                        foreach (Character character in EncounterModule.Model.Characters.Values)
                        {
                            if (character.IsAlive)
                            {
                                character.IncrementClock();
                                if (character.IsClockGuageFull)
                                {
                                    EncounterModule.Model.PendingCharacterTurns.Add(character);
                                }
                            }
                        }

                        ProgressState();
                        break;

                    case (FlowState.TurnResolution):
                    {
                        // Progress turn queue if current turn completed
                        if (EncounterModule.Model.CurrrentTurnCharacter == null)
                        {
                            if (EncounterModule.Model.PendingCharacterTurns.Count > 0)
                            {
                                EncounterModule.Model.CurrrentTurnCharacter = EncounterModule.Model.PendingCharacterTurns[0];
                                EncounterModule.Model.PendingCharacterTurns.RemoveAt(0);
                                EncounterModule.Model.TurnCompleted = false;

                                EncounterModule.Model.CurrrentTurnCharacter.RolloverClock();
                            }
                        }

                        // If current turn not assigned, turn queue is complete. Progress flow
                        if (EncounterModule.Model.CurrrentTurnCharacter != null)
                        {
                            Character toProgress = EncounterModule.Model.CurrrentTurnCharacter;
                            if ((toProgress.HasActed && toProgress.HasMoved) 
                                || EncounterModule.Model.TurnCompleted)
                            {
                                toProgress.DecrementClock();
                                EncounterModule.Model.CurrrentTurnCharacter = null;
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

        private void GetInputForActor(Character character)
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.TurnBegun, character));
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
                            EncounterModule.Model.TurnCompleted = true;
                            ProgressFlow();
                        }
                        break;

                        case EncounterMessage.EventType.CharacterKO:
                        {
                            Character kodCharacter = message.Arg<Character>();

                            if (kodCharacter == EncounterModule.Model.CurrrentTurnCharacter)
                            {
                                kodCharacter.DecrementClock();
                                EncounterModule.Model.TurnCompleted = true;
                            }

                            EncounterModule.Model.PendingCharacterTurns.Remove(kodCharacter);
                        }
                        break;
                    }
                }
                break;
            }
        }
    }
}


