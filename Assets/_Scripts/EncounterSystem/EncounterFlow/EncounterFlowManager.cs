using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EncounterSystem
{
    using Character;
    namespace EncounterFlow
    {
        using Common.ActionTypes;
        using Common.EncounterTypes;
        using Queue;
        using Mediator;
        using MapTypes;
        using Common.CharacterEnums;
        using Common.EncounterEnums;

        class EncounterFlowManager : MonoBehaviour
        {
            private EncounterManager rEncounterManager;
            private EncounterState rEncounterState;
            private EncounterBluePrint rEncounterBluePrint;

            private InputManager mInputManager;
            private ActionQueue mActionQueue;
            private ActionFlowMediator mActionMediator;
            private TurnQueue mTurnQueue;
            private MovementMediator mMovementMediator;
            
            private bool mFlowSuspended = false;
            private bool mFlowControlReturned = false;
            private bool mEncounterIsFinished = false;

            private List<EncounterSuccessEvent> mWinConditions;
            private List<EncounterCinematicEvent> mCinematicEvents;

            void Awake()
            {
                mActionQueue = new ActionQueue();
                mTurnQueue = new TurnQueue();
                mWinConditions = new List<EncounterSuccessEvent>();
                mCinematicEvents = new List<EncounterCinematicEvent>();

            }

            public void Initialize(EncounterManager manager, InputManager inputManager,
                EncounterState state, EncounterBluePrint bluePrint, 
                ActionFlowMediator actionFlowMediator, MovementMediator movementMediator,
                List<EncounterSuccessEvent> winConditions, List<EncounterCinematicEvent> dialogueEvents)
            {
                rEncounterManager = manager;
                mInputManager = inputManager;
                rEncounterState = state;
                rEncounterBluePrint = bluePrint;
                mTurnQueue.Initialize(rEncounterState);
                mWinConditions.AddRange(winConditions);
                mCinematicEvents.AddRange(dialogueEvents);
                mActionMediator = actionFlowMediator;
                mMovementMediator = movementMediator;
            }

            public void BeginEncounterFlow()
            {
                rEncounterState.FlowState = EncounterFlowState.UnitPlacement;
                mEncounterIsFinished = false;
                mFlowControlReturned = false;
                mFlowSuspended = false;
                StartCoroutine(_ProgressTurnFlow());
            }

            private IEnumerator _ProgressTurnFlow()
            {
                while (!mEncounterIsFinished)
                {
                    if (mFlowControlReturned)
                    {
                        mFlowControlReturned = mFlowSuspended = false;
                        if (rEncounterState.FlowState > EncounterFlowState.EncounterBegun)
                        {
                            QueryCinematicEvents();
                            QueryWinConditions();
                        }
                    }
                    else if (!mFlowSuspended)
                    {
                        // check for win conditions
                        switch (rEncounterState.FlowState)
                        {
                            case (EncounterFlowState.UnitPlacement):
                                List<CharacterManager> unitsToPlace = rEncounterState.GetUnitGroup(UnitGroup.Player);
                                if (unitsToPlace.Count > 0)
                                {
                                    mInputManager.GetInput(rEncounterState.GetUnitGroup(UnitGroup.Player), rEncounterBluePrint.NumUnitsToPlace, rEncounterBluePrint.UnitPlacementSide);
                                    mFlowSuspended = true;
                                }
                                
                                ++rEncounterState.FlowState;
                                break;

                            case (EncounterFlowState.OpeningCinematic):
                                if (!rEncounterBluePrint.OpeningCinematic.EmptyCinematic)
                                {
                                    foreach (CharacterManager character in rEncounterState.GetAllEncounterUnits())
                                    {
                                        character.gameObject.SetActive(false);
                                    }
                                    mInputManager.GetInput(rEncounterBluePrint.OpeningCinematic);
                                    mFlowSuspended = true;
                                }
                                else
                                {
                                    // inputManager.BeginEncounter
                                }
                                ++rEncounterState.FlowState;
                                break;

                            case (EncounterFlowState.Setup):
                                foreach (CharacterManager character in rEncounterState.GetAllEncounterUnits())
                                {
                                    character.gameObject.SetActive(true);
                                    character.BeginEncounter();
                                }
                                ProgressClockToFirstTurn();
                                rEncounterState.FlowState = EncounterFlowState.TurnResolution;
                                break;

                            case (EncounterFlowState.EncounterClock):
                                rEncounterState.IncrementTurnCount();
                                // Poll Event
                                ++rEncounterState.FlowState;
                                break;

                            case (EncounterFlowState.Status):
                                foreach (CharacterManager manager in rEncounterState.GetAllEncounterUnits())
                                {
                                    if (manager.ProgressStatusClock())
                                    {
                                        // Pan to character
                                        yield return new WaitForSeconds(0.5f); //TODO: Make this dependant on the number of status' expired
                                    }
                                }
                                ++rEncounterState.FlowState;
                                break;

                            case (EncounterFlowState.ActionCharge):
                                mActionQueue.IncrementChargeClocks();
                                ++rEncounterState.FlowState;
                                break;

                            case (EncounterFlowState.ActionResolution):
                                if (mActionQueue.HasReadyActions)
                                {
                                    QueuedActionPayload toMediate = mActionQueue.NextReadyAction;
                                    mFlowSuspended = true;
                                    mActionMediator.MediateAction(toMediate);
                                }
                                else
                                {
                                    mFlowSuspended = false;
                                    ++rEncounterState.FlowState;
                                }
                                break;

                            case (EncounterFlowState.TurnCharge):
                                mTurnQueue.IncrementClockCounts();
                                ++rEncounterState.FlowState;
                                break;

                            case (EncounterFlowState.TurnResolution):
                                CharacterManager nextUnit = ProgressTurnResolution(rEncounterState.CurrentUnit);

                                if (nextUnit != null)
                                {
                                    if (nextUnit != rEncounterState.CurrentUnit)
                                    {
                                        nextUnit.NewTurn();
                                        rEncounterState.SetCurrentUnit(nextUnit);
                                    }
                                    mInputManager.GetInput(rEncounterState.CurrentUnit);
                                    mFlowSuspended = true;
                                }
                                else // if current unit wasn't set, 
                                {
                                    rEncounterState.SetCurrentUnit(null);
                                    mFlowSuspended = false;
                                    rEncounterState.FlowState = EncounterFlowState.EncounterClock;
                                    rEncounterState.IncrementTurnCount();
                                }
                                break;

                            case (EncounterFlowState.EncounterFinished):
                                mEncounterIsFinished = true;
                                rEncounterManager.EncounterOver();
                                break;
                        }
                    }
                    
                    yield return new WaitForSeconds(0);
                }
                rEncounterManager.EncounterOver();
            }

            public void ReturnControlToFlow()
            {
                mFlowControlReturned = true;
            }

            public void InputChosen(QueuedActionPayload actionSelection)
            {
                mActionQueue.QueueAction(actionSelection); // queue immediately and let internals decide charge time
                if (mActionQueue.HasReadyActions)
                {
                    mActionMediator.MediateAction(mActionQueue.NextReadyAction);
                }
                else
                {
                    ReturnControlToFlow();
                }
            }

            public void InputChosen(MapTile movementSelection)
            {
                mMovementMediator.MediateMovement(rEncounterState.CurrentUnit, movementSelection, ReturnControlToFlow);
            }

            /// <summary>
            /// Attempts to get the next unit in the turn resolution queue
            /// </summary>
            private CharacterManager ProgressTurnResolution(CharacterManager currentUnit)
            {
                CharacterManager nextUnit = null;
                // Check if current unit still needs to finish turn
                if (currentUnit != null)
                {
                    if (currentUnit.FinishedTurn)
                    {
                        if (currentUnit.IsAlive)
                        {
                            currentUnit.CleanupAfterTurn();
                        }
                    }
                    else
                    {
                        nextUnit = currentUnit;
                    }
                }

                // If no current unit attempt to get a new one from the turn queue
                if (nextUnit == null)
                {
                    CharacterManager nextInQueue = null;
                    while (nextUnit == null && mTurnQueue.HasReadyUnits())
                    {
                        nextInQueue = mTurnQueue.NextReadyUnit();
                        if (nextInQueue.IsAlive)
                        {
                            nextUnit = nextInQueue;
                        }
                    }
                }
                return nextUnit;
            }

            // TODO: remove event if one of the characters is dead or missing
            private bool QueryCinematicEvents()
            {
                EncounterCinematicEvent triggeredCinematic = null;
                
                for (int i = 0; i < mCinematicEvents.Count; ++i)
                {
                    EncounterCinematicEvent cinematic = mCinematicEvents[i];
                    if (cinematic.IsEventTriggered(rEncounterState))
                    {
                        if (cinematic.IsEventValid(rEncounterState))
                        {
                            triggeredCinematic = cinematic;
                            break;
                        }
                        else
                        {
                            Debug.Log("Removing Cinematic event for being invalid");
                            mCinematicEvents.RemoveAt(i);
                            --i;
                        }
                    }
                }

                if (triggeredCinematic != null)
                {
                    mCinematicEvents.Remove(triggeredCinematic);
                    mFlowSuspended = true;
                    mInputManager.GetInput(triggeredCinematic.CinematicBlueprint);
                    return true;
                }
                return false;
            }

            private void QueryWinConditions()
            {
                foreach (EncounterSuccessEvent winCondition in mWinConditions)
                {
                    if (winCondition.IsEventTriggered(rEncounterState))
                    {
                        if (winCondition.EventConsequence != EncounterSuccessState.InProgess)
                        {
                            rEncounterState.SetCurrentSuccessState(winCondition.EventConsequence);
                            rEncounterState.FlowState = EncounterFlowState.EncounterFinished;
                            break;
                        }
                    }
                }
            }

            private void ProgressClockToFirstTurn()
            {
                while (!mTurnQueue.HasReadyUnits())
                    mTurnQueue.IncrementClockCounts();
            }

        }
    }
}
