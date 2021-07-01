
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
    class ActionResolutionFlowControl : FlowControlBase
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
            nextAction.Proposer.GetComponent<ActionsControl>().OnActionPerformed(nextAction.Action.ActionInfo.ActionCost);
            mComposition = nextAction.Action.Compose(nextAction.Target);
            
            mActionTimeline = new Timeline<TimelineElement>(mComposition.ActionTimeline);

            ProgressTimeline(0);
        }

        void ProgressTimeline(float dt)
        {
            bool isTimelineFinished = mActionTimeline.ProgressTimeline(dt);
            if (isTimelineFinished)
            {
                mActionTimeline = null;

                foreach (CombatEntity character in GameModel.Encounter.Players.Values.Where(x => x.GetComponent<ResourcesControl>().IsAlive()))
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
}

