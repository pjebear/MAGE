using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EncounterSystem
{
    using Action;
    using MapTypes;
    using Character;
    using EventSystem;
    using Common.ActionTypes;
    using MapEnums;
    using Map;
    using Screen;

    namespace EncounterFlow
    {
        namespace Mediator
        {
            class ActionFlowMediator : MediatorBase
            {
                public void MediateAction(QueuedActionPayload actionPayload)
                {
                    bool canPerformAction = actionPayload.Actor.CanPerformAction(actionPayload.Action);
                    if (canPerformAction)
                    {
                        rViewController.gameObject.SetActive(true);
                        // Calculate the action target tiles 
                        rMapManager.CalculateActionAOETiles(actionPayload.TargetSelection.GetActionOrigin(), actionPayload.Actor.GetCurrentTile(), actionPayload.Action.MapInteractionInfo);

                        //pan camera to target

                        rViewController.DisplayActionText(actionPayload.Action.ActionInfo.Name);
                        actionPayload.Actor.HasActed = true;
                        actionPayload.Actor.IsChargingAction = false;
                        StartCoroutine(_Act(actionPayload.Actor, actionPayload.Action, rMapManager.GetActionAOETiles(), rMapManager.GetAOEOrigin()));
                    } 
                    else
                    {
                        rFlowManager.ReturnControlToFlow();
                    }
                }

                IEnumerator _Act(CharacterManager toAct, ActionBase action, List<MapTile> targets, MapTile lookAt)
                {
                    // TODO: pan camera to casting unit
                    //unit performs cast animation

                    toAct.BroadcastMessage(EventType.OnAct.ToString(), new OnActPayload(action), SendMessageOptions.DontRequireReceiver);
                    if (!action.AnimationInfo.PreActionAnimationId.Equals(""))
                    {
                        toAct.GetComponent<Animator>().SetTrigger(action.AnimationInfo.PreActionAnimationId);
                        
                    }
                    yield return new WaitForSeconds(1f); // wait for cast animation

                    // no action has interupted action
                    if (toAct.CanPerformAction(action)) 
                    {
                        toAct.RemoveActionResourceCost(action);
                        //do what the action wants
                        foreach (MapTile tile in targets)
                        {
                            if (tile.GetCharacterOnTile() != null)
                            {
                                tile.GetCharacterOnTile().BroadcastMessage("OnTargeted", new OnTargetedPayload(toAct, action), SendMessageOptions.DontRequireReceiver);
                            }
                        }
                        List<ActionInteractionResult> actionResults = new List<ActionInteractionResult>();

                        Vector3 currentForward = transform.forward;
                        if (action.MapInteractionInfo.TargetSelectionType == TargetSelectionType.Targeted
                            && targets.Count > 0)
                        {
                            Vector3 lookAtPosition = lookAt.transform.GetChild(0).position;
                            lookAtPosition.y = toAct.transform.position.y;
                            toAct.gameObject.transform.LookAt(lookAtPosition);
                        }
                        

                        StartCoroutine(action.ExececuteAction(toAct, // caster
                            targets, // action targets
                            (x) => actionResults.Add(x))); // _recordActionResults
                        yield return new WaitUntil(delegate () { return action.FinishedExecution; });

                        bool successfulAction = false;
                        foreach (ActionInteractionResult actionResult in actionResults)
                        {
                            if (actionResult.WasSuccessful)
                            {
                                successfulAction = true;
                                break;
                            }
                        }

                        // give unit experience if they were successful
                        if (toAct.IsAlive && successfulAction)
                        {
                            toAct.AddExperience();
                        }
                        

                        bool didNegativeResourceChange = false;
                        bool didPositiveResourceChange = false;

                        foreach (var actionResult in actionResults)
                        {
                            if (actionResult.ChangedResource.Value < 0)
                            {
                                toAct.BroadcastMessage(EventType.OnDamageDealt.ToString(), new OnDamagedPayload(toAct, actionResult.Target), SendMessageOptions.DontRequireReceiver);
                                actionResult.Target.BroadcastMessage(EventType.OnDamageTaken.ToString(), new OnDamagedPayload(toAct, actionResult.Target), SendMessageOptions.DontRequireReceiver);
                            }
                        }

                        //pan back to casting unit
                        yield return new WaitForSeconds(2f);
                        
                        Debug.Log("Returning from action");
                        
                    }
                    toAct.BroadcastMessage(EventType.OnActComplete.ToString(), new OnActPayload(action), SendMessageOptions.DontRequireReceiver);
                    //do what the action wants
                    foreach (MapTile tile in targets)
                    {
                        if (tile.GetCharacterOnTile() != null)
                        {
                            tile.GetCharacterOnTile().BroadcastMessage("OnTargetedComplete", new OnTargetedPayload(toAct, action), SendMessageOptions.DontRequireReceiver);
                        }
                    }
                    // clean up
                    CleanUp();
                    rFlowManager.ReturnControlToFlow();
                }

                void CleanUp()
                {
                    rViewController.gameObject.SetActive(false);
                }
            }
        }
    }
    
}

