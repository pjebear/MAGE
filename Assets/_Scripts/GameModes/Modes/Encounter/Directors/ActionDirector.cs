
using MAGE.GameModes.Combat;
using MAGE.GameModes.FlowControl;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class ActionDirector : FlowControlBase
    {
        private Timeline<TimelineElement> mActionTimeline;
        private ActionComposition mComposition;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.EncounterActionDirector;
        }

        protected override void Setup()
        {
            
        }

        protected override void Cleanup()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (mActionTimeline != null)
            {
                ProgressTimeline(Time.deltaTime);
            }
        }

        void BeginNextAction(ActionProposal nextAction)
        {
            ActionComposerBase actionComposer = ActionComposer.GetAction(nextAction.Proposer, nextAction.Action);
            mComposition = actionComposer.Compose(nextAction.Target);

            mActionTimeline = new Timeline<TimelineElement>(mComposition.ActionTimeline);

            ProgressTimeline(0);
        }

        void ProgressTimeline(float dt)
        {
            bool isTimelineFinished = mActionTimeline.ProgressTimeline(dt);
            if (isTimelineFinished)
            {
                mActionTimeline = null;

                foreach (CombatCharacter character in GameModel.Encounter.Players)
                {
                    if (character.GetComponent<ResourcesControl>().IsAlive())
                    {
                        foreach (ActionResponseBase actionResponseBase in character.GetComponent<ActionsControl>().RespondToAction(mComposition.ActionResults))
                        {
                            switch (actionResponseBase.ResponseType)
                            {
                                case ActionResponseType.ActionProposal:
                                {
                                    GameModel.Encounter.mActionQueue.Enqueue((actionResponseBase as ActionProposalResponse).Response);
                                }
                                break;
                                case ActionResponseType.StateChange:
                                {
                                    StateChangeResponse stateChangeResponse = actionResponseBase as StateChangeResponse;
                                    character.GetComponent<CombatTarget>().ApplyStateChange(stateChangeResponse.Response);
                                }
                                break;
                            }
                        }
                    }
                }

                SendFlowMessage("actionResolved");
            }
        }

        public override bool Notify(string notifyEvent)
        {
            bool handled = false;

            switch (notifyEvent)
            {
                case "resolveAction":
                {
                    ActionProposal proposal = GameModel.Encounter.mActionQueue.Dequeue();
                    BeginNextAction(proposal);
                    handled = true;
                }
                break;
            }

            return handled;
        }
    }

    class ActionDirector_Deprecated : MonoBehaviour
    {
        private Timeline<ActionEvent> mActionTimeline;
        private GameSystems.Actions.ActionResult mActionResult;

        private Queue<ActionProposal_Deprecated> mActionQueue;
        private Dictionary<ActionProposal_Deprecated, int> mDelayedActions;

        public void Init()
        {
            mActionQueue = new Queue<ActionProposal_Deprecated>();
            mDelayedActions = new Dictionary<ActionProposal_Deprecated, int>();
        }

        public void Cleanup()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (mActionTimeline != null)
            {
                ProgressTimeline(Time.deltaTime);
            }
        }

        public void DirectAction(ActionProposal_Deprecated proposal)
        {
            int castSpeed = ActionUtil.GetTurnCountForCastSpeed(proposal.Proposer.GetActionInfo(proposal.Action).CastSpeed);

            if (castSpeed != ActionConstants.INSTANT_CAST_SPEED)
            {
                mDelayedActions.Add(proposal, castSpeed);
                Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.ActionResolved));
            }
            else
            {
                if (mActionTimeline != null)
                {
                    mActionQueue.Enqueue(proposal);
                }
                else
                {
                    BeginNextAction(proposal);
                }
            }
        }

        public bool HasReadyActions()
        {
            return GetReadyActions().Count > 0;
        }

        public void ProgressDelayedActions()
        {
            List<ActionProposal_Deprecated> readyActions = GetReadyActions();
            if (readyActions.Count > 0)
            {
                mDelayedActions.Remove(readyActions[0]);
                BeginNextAction(readyActions[0]);
            }
        }

        public List<ActionProposal_Deprecated> GetReadyActions()
        {
            return mDelayedActions.Where(x => x.Value <= 0).Select(x => x.Key).ToList();
        }

        void BeginNextAction(ActionProposal_Deprecated nextAction)
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.ActionResolutionBegin, nextAction));
            ActionComposer.ComposeAction(nextAction, EncounterFlowControl_Deprecated.MapControl.Map, out mActionResult, out mActionTimeline);

            ProgressTimeline(0);
        }

        void ProgressTimeline(float dt)
        {
            //bool isTimelineFinished = mActionTimeline.ProgressTimeline(dt);
            //if (isTimelineFinished)
            //{
            //    ApplyActionResults();

            //    // Update Orientations
            //    UpdateOrientations();
                
            //    // Check for responses
            //    List<ActionResponseBase> responses = new List<ActionResponseBase>();
            //    foreach (Character character in GameModel.Encounter.Characters.Values)
            //    {
            //        if (character.IsAlive)
            //        {
            //            foreach (ActionResponseBase actionResponseBase in character.RespondToAction(mActionResult, EncounterFlowControl_Deprecated.MapControl.Map))
            //            {
            //                switch (actionResponseBase.ResponseType)
            //                {
            //                    case ActionResponseType.ActionProposal:
            //                    {
            //                        DirectAction((actionResponseBase as ActionProposalResponse).Response);
            //                    }
            //                    break;
            //                    case ActionResponseType.StateChange:
            //                    {
            //                        StateChangeResponse stateChangeResponse = actionResponseBase as StateChangeResponse;
            //                        EncounterFlowControl_Deprecated.CharacterDirector.ApplyStateChange(character, stateChangeResponse.Response);
            //                    }
            //                    break;
            //                }
            //            }
            //        }
            //    }

            //    mActionTimeline = null;
            //    mActionResult = null;

            //    if (mActionQueue.Count > 0)
            //    {
            //        BeginNextAction(mActionQueue.Dequeue());
            //    }
            //    else
            //    {
            //        Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.ActionResolved));
            //    }
            //}
        }

        void ApplyActionResults()
        {
            //mActionResult.Initiator.ApplyStateChange(mActionResult.InitiatorResult.StateChange);

            //foreach (var actorResultPair in mActionResult.TargetResults)
            //{
            //    EncounterFlowControl_Deprecated.CharacterDirector.ApplyStateChange(actorResultPair.Key, actorResultPair.Value.StateChange);
            //}
        }

        void UpdateOrientations()
        {
            //HashSet<Character> toUpdate = new HashSet<Character>();
            //toUpdate.Add(mActionResult.Initiator);
            //foreach (Character character in mActionResult.TargetResults.Keys)
            //{
            //    toUpdate.Add(character);
            //}

            //foreach (Character character in toUpdate)
            //{
            //    CharacterPosition characterPosition = EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(character);

            //    CharacterActorController controller = EncounterFlowControl_Deprecated.CharacterDirector.CharacterActorLookup[character];
            //    characterPosition.Orientation = OrientationUtil.FromVector(controller.transform.forward);
            //    EncounterFlowControl_Deprecated.MapControl.UpdateCharacterPosition(controller, characterPosition);
            //}            
        }

        public void IncrementDelayedActions()
        {
            foreach (ActionProposal_Deprecated proposal in mDelayedActions.Keys.ToList())
            {
                mDelayedActions[proposal]--;
            }
        }
    }
}

