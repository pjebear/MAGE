
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
        //, Messaging.IMessageHandler
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

        //private void IncrementClock()
        //{
        //    GameModel.Encounter.Clock++;
        //    Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.ClockProgressed));
        //}

        //public void Init()
        //{
        //    Messaging.MessageRouter.Instance.RegisterHandler(this);
        //    mState = FlowState.Intro;

        //    mIsFlowPaused = true;
        //}

        //private void Start()
        //{

        //}

        //private void OnDestroy()
        //{
        //    Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        //}

        //private void Update()
        //{
        //    if (!mIsFlowPaused)
        //    {
        //        RunFlowTick();
        //    }
        //}

        //private void RunFlowTick()
        //{
        //    //if (Model.ChoreoEvents.Count > 0)
        //    //{
        //    //    pauseFlow = ProgressChoreo();
        //    //}
        //    if (CheckIsEncounterOver())
        //    {
        //        mIsFlowPaused = true;
        //        Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.EncounterOver));
        //    }
        //    else
        //    {
        //        switch (mState)
        //        {
        //            case (FlowState.StatusIncrement):
        //                EncounterFlowControl_Deprecated.CharacterDirector.IncrementStatusEffects();

        //                ProgressState();
        //                break;

        //            case (FlowState.StatusCheck):
        //                EncounterFlowControl_Deprecated.CharacterDirector.ApplyStatusEffects();
        //                ProgressState();

        //                break;

        //            case (FlowState.ActionIncrement):
        //                EncounterFlowControl_Deprecated.ActionDirector.IncrementDelayedActions();
        //                ProgressState();
        //                break;

        //            case (FlowState.ActionResolution):
        //            {
        //                if (EncounterFlowControl_Deprecated.ActionDirector.HasReadyActions())
        //                {
        //                    EncounterFlowControl_Deprecated.ActionDirector.ProgressDelayedActions();
        //                    mIsFlowPaused = true;
        //                }
        //                else
        //                {
        //                    ProgressState();
        //                }
        //            }
        //            break;

        //            case (FlowState.TurnIncrement):
        //                foreach (Character character in GameModel.Encounter.Characters.Values)
        //                {
        //                    if (character.IsAlive)
        //                    {
        //                        character.IncrementClock();
        //                        if (character.IsClockGuageFull)
        //                        {
        //                            GameModel.Encounter.PendingCharacterTurns.Add(character);
        //                        }
        //                    }
        //                }

        //                ProgressState();
        //                break;

        //            case (FlowState.TurnResolution):
        //            {
        //                // Progress turn queue if current turn completed
        //                if (GameModel.Encounter.CurrrentTurnCharacter == null)
        //                {
        //                    if (GameModel.Encounter.PendingCharacterTurns.Count > 0)
        //                    {
        //                        GameModel.Encounter.CurrrentTurnCharacter = GameModel.Encounter.PendingCharacterTurns[0];
        //                        GameModel.Encounter.PendingCharacterTurns.RemoveAt(0);
        //                        GameModel.Encounter.TurnCompleted = false;

        //                        GameModel.Encounter.CurrrentTurnCharacter.RolloverClock();
        //                    }
        //                }

        //                // If current turn not assigned, turn queue is complete. Progress flow
        //                if (GameModel.Encounter.CurrrentTurnCharacter != null)
        //                {
        //                    Character toProgress = GameModel.Encounter.CurrrentTurnCharacter;
        //                    if ((toProgress.HasActed && toProgress.HasMoved) 
        //                        || GameModel.Encounter.TurnCompleted)
        //                    {
        //                        toProgress.DecrementClock();
        //                        GameModel.Encounter.CurrrentTurnCharacter = null;
        //                    }
        //                    else
        //                    {
        //                        mIsFlowPaused = true;
        //                        GetInputForActor(toProgress);
        //                    }
        //                }
        //                else
        //                {
        //                    ProgressState();
        //                }
        //            }
        //            break;

        //            default:
        //                mIsFlowPaused = true;
        //                break;
        //        }
        //    }
        //}

        //private void ProgressFlow()
        //{
        //    mIsFlowPaused = false;
        //}

        //private bool ProgressChoreo()
        //{
        //    //// TODO
        //    //if (Model.ChoreoEvents.Count > 0)
        //    //{
        //    //    // TODO: Display Something
        //    //    Debug.Log(string.Format("Choreo Event: {0} ", Model.ChoreoEvents[0]));
        //    //    Model.ChoreoEvents.RemoveAt(0);
        //    //}
        //    // Don't pause flow;
        //    return false;
        //}

        //private bool CheckIsEncounterOver()
        //{
        //    bool encounterOver = false;
        //    if (IsEncounterLost())
        //    {
        //        encounterOver = true;
        //        GameModel.Encounter.EncounterState = EncounterState.Defeat;
        //    }
        //    else if (IsEncounterWon())
        //    {
        //        encounterOver = true;
        //        GameModel.Encounter.EncounterState = EncounterState.Win;
        //    }

        //    return encounterOver;
        //}

        //private bool IsEncounterLost()
        //{
        //    bool encounterLost = false;

        //    foreach (EncounterCondition loseCondition in GameModel.Encounter.EncounterContext.LoseConditions)
        //    {
        //        bool isConditionMet = loseCondition.IsConditionMet(GameModel.Encounter);

        //        if (isConditionMet)
        //        {
        //            Logger.Log(LogTag.GameModes, TAG, string.Format("Loss condition met: {0} ", loseCondition.ToString()));
        //        }

        //        encounterLost |= isConditionMet;
        //    }

        //    return encounterLost;
        //}

        //private bool IsEncounterWon()
        //{
        //    bool encounterWon = false;

        //    foreach (EncounterCondition winCondition in GameModel.Encounter.EncounterContext.WinConditions)
        //    {
        //        bool isConditionMet = winCondition.IsConditionMet(GameModel.Encounter);

        //        if (isConditionMet)
        //        {
        //            Logger.Log(LogTag.GameModes, TAG, string.Format("Loss condition met: {0} ", winCondition.ToString()));
        //        }

        //        encounterWon |= isConditionMet;
        //    }

        //    return encounterWon;
        //}

        //private void ProgressState()
        //{
        //    mState++;

        //    if (mState == FlowState.CoreLoopBegin)
        //    {
        //        ProgressFlow();
        //    }
        //    else if (mState == FlowState.CoreLoopEnd)
        //    {
        //        mState = FlowState.CoreLoopBegin;
        //        IncrementClock();
        //    }
        //}

        //private void GetInputForActor(Character character)
        //{
        //    Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.TurnBegun, character));
        //}

        //// EventHandler
        //public void HandleMessage(Messaging.MessageInfoBase messageInfoBase)
        //{
        //    switch (messageInfoBase.MessageId)
        //    {
        //        case EncounterMessage.Id:
        //        {
        //            EncounterMessage message = messageInfoBase as EncounterMessage;

        //            Logger.Log(LogTag.GameModes, TAG, "HandleMessage() - " + message.Type.ToString());

        //            switch (message.Type)
        //            {
        //                case EncounterMessage.EventType.EncounterBegun:
        //                    EncounterFlowControl_Deprecated.IntroViewControl.Show();
        //                    break;

        //                case EncounterMessage.EventType.IntroComplete:
        //                    EncounterFlowControl_Deprecated.IntroViewControl.Hide();
        //                    ProgressState();
        //                    EncounterFlowControl_Deprecated.UnitPlacementViewControl.Start();
        //                    break;

        //                case EncounterMessage.EventType.UnitPlacementComplete:

        //                    EncounterFlowControl_Deprecated.UnitPlacementViewControl.Cleanup();
        //                    ProgressState();

        //                    break;
        //                case EncounterMessage.EventType.ActionResolved:
        //                    ProgressFlow();
        //                    break;

        //                case EncounterMessage.EventType.MoveResolved:
        //                    ProgressFlow();
        //                    break;

        //                case EncounterMessage.EventType.TurnFinished:
        //                {
        //                    GameModel.Encounter.TurnCompleted = true;
        //                    ProgressFlow();
        //                }
        //                break;

        //                case EncounterMessage.EventType.CharacterKO:
        //                {
        //                    Character kodCharacter = message.Arg<Character>();

        //                    if (kodCharacter == GameModel.Encounter.CurrrentTurnCharacter)
        //                    {
        //                        kodCharacter.DecrementClock();
        //                        GameModel.Encounter.TurnCompleted = true;
        //                    }

        //                    GameModel.Encounter.PendingCharacterTurns.Remove(kodCharacter);
        //                }
        //                break;
        //            }
        //        }
        //        break;
        //    }
        //}
    }
}


