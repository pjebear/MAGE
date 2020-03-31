﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class MasterFlowControl 
    : MonoBehaviour
    , IEventHandler<EncounterEvent>
{
    private string TAG = "MasterFlowControl";
    // StatusEffect state
    private Dictionary<EncounterCharacter, List<StatusEffect>> StaleStatusEffects = new Dictionary<EncounterCharacter, List<StatusEffect>>();

    enum FlowState
    {
        //Init,
        StatusIncrement,
        StatusCheck,
        //ActionIncrement,
        //ActionResolution,
        TurnIncrement,
        TurnResolution,

        NUM
    }

    private bool mIsFlowPaused;
    private FlowState mState;

    private void IncrementClock()
    {
        EncounterModule.Model.Clock++;
        EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.ClockProgressed));
    }

    public void Init()
    {
        EncounterEventRouter.Instance.RegisterHandler(this);
        mState = FlowState.TurnIncrement;

        mIsFlowPaused = true;
    }

    private void Start()
    {
        
    }

    private void OnDestroy()
    {
        EncounterEventRouter.Instance.UnRegisterListener(this);
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
            EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.EncounterOver));
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

                //case (FlowState.ActionIncrement):
                //    // TODO:
                //    ProgressState();
                //    break;

                //case (FlowState.ActionResolution):
                //    // TODO: 
                //    ProgressState();
                //    break;

                case (FlowState.TurnIncrement):
                    EncounterModule.Model.TurnOrder.Clear();

                    foreach (EncounterCharacter actor in EncounterModule.Model.Characters.Values)
                    {
                        if (actor.IsAlive)
                        {
                            actor.DEBUG_HasActed = false;
                            actor.DEBUG_HasMoved = false;
                            actor.DEBUG_IsTurnComplete = false;
                            EncounterModule.Model.TurnOrder.Add(actor);
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

        foreach (EncounterCondition loseCondition in EncounterModule.Model.LoseConditions)
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

        foreach (EncounterCondition winCondition in EncounterModule.Model.WinConditions)
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
        if (mState == FlowState.NUM)
        {
            mState = FlowState.StatusIncrement;
            IncrementClock();
        }
    }

    private void GetInputForActor(EncounterCharacter actor)
    {
        EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.TurnBegun, actor));
    }

    // EventHandler
    public void HandleEvent(EncounterEvent eventInfo)
    {
        Logger.Log(LogTag.GameModes, TAG, "HandleEvent() - " + eventInfo.Type.ToString());

        switch (eventInfo.Type)
        {
            case EncounterEvent.EventType.EncounterBegun:
                ProgressFlow();
                break;
            case EncounterEvent.EventType.ActionResolved:
                ProgressFlow();
                break;

            case EncounterEvent.EventType.MoveResolved:
                ProgressFlow();
                break;

            case EncounterEvent.EventType.TurnFinished:
                ProgressFlow();
                break;

            case EncounterEvent.EventType.CharacterKO:
                {
                    EncounterCharacter kodActor = eventInfo.Arg<EncounterCharacter>();

                    EncounterModule.Model.TurnOrder.Remove(kodActor);
                }
                break;

        }
    }
}
