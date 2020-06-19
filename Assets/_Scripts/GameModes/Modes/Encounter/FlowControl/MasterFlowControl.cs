using System;
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
        EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.ClockProgressed));
    }

    public void Init()
    {
        EncounterEventRouter.Instance.RegisterHandler(this);
        mState = FlowState.Intro;

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
        EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.TurnBegun, actor));
    }

    // EventHandler
    public void HandleEvent(EncounterEvent eventInfo)
    {
        Logger.Log(LogTag.GameModes, TAG, "HandleEvent() - " + eventInfo.Type.ToString());

        switch (eventInfo.Type)
        {
            case EncounterEvent.EventType.EncounterBegun:
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

            case EncounterEvent.EventType.IntroComplete:
                EncounterModule.IntroViewControl.Hide();
                ProgressState();
                EncounterModule.UnitPlacementViewControl.Start();
                break;

            case EncounterEvent.EventType.UnitPlacementComplete:

                EncounterModule.UnitPlacementViewControl.Cleanup();
                ProgressState();

                break;
            case EncounterEvent.EventType.ActionResolved:
                ProgressFlow();
                break;

            case EncounterEvent.EventType.MoveResolved:
                ProgressFlow();
                break;

            case EncounterEvent.EventType.TurnFinished:
            {
                ProgressFlow();
            }    
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

