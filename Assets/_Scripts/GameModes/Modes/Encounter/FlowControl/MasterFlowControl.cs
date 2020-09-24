
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
            EncounterFlowControl.Model.Clock++;
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
                        EncounterFlowControl.CharacterDirector.IncrementStatusEffects();

                        ProgressState();
                        break;

                    case (FlowState.StatusCheck):
                        EncounterFlowControl.CharacterDirector.ApplyStatusEffects();
                        ProgressState();

                        break;

                    case (FlowState.ActionIncrement):
                        EncounterFlowControl.ActionDirector.IncrementDelayedActions();
                        ProgressState();
                        break;

                    case (FlowState.ActionResolution):
                    {
                        if (EncounterFlowControl.ActionDirector.HasReadyActions())
                        {
                            EncounterFlowControl.ActionDirector.ProgressDelayedActions();
                            mIsFlowPaused = true;
                        }
                        else
                        {
                            ProgressState();
                        }
                    }
                    break;

                    case (FlowState.TurnIncrement):
                        foreach (Character character in EncounterFlowControl.Model.Characters.Values)
                        {
                            if (character.IsAlive)
                            {
                                character.IncrementClock();
                                if (character.IsClockGuageFull)
                                {
                                    EncounterFlowControl.Model.PendingCharacterTurns.Add(character);
                                }
                            }
                        }

                        ProgressState();
                        break;

                    case (FlowState.TurnResolution):
                    {
                        // Progress turn queue if current turn completed
                        if (EncounterFlowControl.Model.CurrrentTurnCharacter == null)
                        {
                            if (EncounterFlowControl.Model.PendingCharacterTurns.Count > 0)
                            {
                                EncounterFlowControl.Model.CurrrentTurnCharacter = EncounterFlowControl.Model.PendingCharacterTurns[0];
                                EncounterFlowControl.Model.PendingCharacterTurns.RemoveAt(0);
                                EncounterFlowControl.Model.TurnCompleted = false;

                                EncounterFlowControl.Model.CurrrentTurnCharacter.RolloverClock();
                            }
                        }

                        // If current turn not assigned, turn queue is complete. Progress flow
                        if (EncounterFlowControl.Model.CurrrentTurnCharacter != null)
                        {
                            Character toProgress = EncounterFlowControl.Model.CurrrentTurnCharacter;
                            if ((toProgress.HasActed && toProgress.HasMoved) 
                                || EncounterFlowControl.Model.TurnCompleted)
                            {
                                toProgress.DecrementClock();
                                EncounterFlowControl.Model.CurrrentTurnCharacter = null;
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
                EncounterFlowControl.Model.EncounterState = EncounterState.Defeat;
            }
            else if (IsEncounterWon())
            {
                encounterOver = true;
                EncounterFlowControl.Model.EncounterState = EncounterState.Win;
            }

            return encounterOver;
        }

        private bool IsEncounterLost()
        {
            bool encounterLost = false;

            foreach (EncounterCondition loseCondition in EncounterFlowControl.Model.EncounterContext.LoseConditions)
            {
                bool isConditionMet = loseCondition.IsConditionMet(EncounterFlowControl.Model);

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

            foreach (EncounterCondition winCondition in EncounterFlowControl.Model.EncounterContext.WinConditions)
            {
                bool isConditionMet = winCondition.IsConditionMet(EncounterFlowControl.Model);

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
                                EncounterFlowControl.UnitPlacementViewControl.Start();
                            }
                            else
                            {
                                EncounterFlowControl.IntroViewControl.Show();
                            }
                            break;

                        case EncounterMessage.EventType.IntroComplete:
                            EncounterFlowControl.IntroViewControl.Hide();
                            ProgressState();
                            EncounterFlowControl.UnitPlacementViewControl.Start();
                            break;

                        case EncounterMessage.EventType.UnitPlacementComplete:

                            EncounterFlowControl.UnitPlacementViewControl.Cleanup();
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
                            EncounterFlowControl.Model.TurnCompleted = true;
                            ProgressFlow();
                        }
                        break;

                        case EncounterMessage.EventType.CharacterKO:
                        {
                            Character kodCharacter = message.Arg<Character>();

                            if (kodCharacter == EncounterFlowControl.Model.CurrrentTurnCharacter)
                            {
                                kodCharacter.DecrementClock();
                                EncounterFlowControl.Model.TurnCompleted = true;
                            }

                            EncounterFlowControl.Model.PendingCharacterTurns.Remove(kodCharacter);
                        }
                        break;
                    }
                }
                break;
            }
        }
    }
}


