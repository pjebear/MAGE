
using MAGE.GameServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class ActionDirector : MonoBehaviour
    {
        private Timeline<ActionEvent> mActionTimeline;
        private ActionResult mActionResult;

        private Queue<ActionProposal> mActionQueue;
        private Dictionary<ActionProposal, int> mDelayedActions;

        public void Init()
        {
            mActionQueue = new Queue<ActionProposal>();
            mDelayedActions = new Dictionary<ActionProposal, int>();
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

        public void DirectAction(ActionProposal proposal)
        {
            int castSpeed = ActionUtil.GetTurnCountForCastSpeed(proposal.Owner.GetActionInfo(proposal.Action).CastSpeed);

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
            List<ActionProposal> readyActions = GetReadyActions();
            if (readyActions.Count > 0)
            {
                mDelayedActions.Remove(readyActions[0]);
                BeginNextAction(readyActions[0]);
            }
        }

        public List<ActionProposal> GetReadyActions()
        {
            return mDelayedActions.Where(x => x.Value <= 0).Select(x => x.Key).ToList();
        }

        void BeginNextAction(ActionProposal nextAction)
        {
            ActionComposer.ComposeAction(nextAction, out mActionResult, out mActionTimeline);

            ProgressTimeline(0);
        }

        void ProgressTimeline(float dt)
        {
            bool isTimelineFinished = mActionTimeline.ProgressTimeline(dt);
            if (isTimelineFinished)
            {
                ApplyActionResults();

                foreach (EncounterCharacter actor in EncounterModule.Model.Characters.Values)
                {
                    actor.NotifyActionResults(mActionResult);
                }

                mActionTimeline = null;
                mActionResult = null;

                if (mActionQueue.Count > 0)
                {
                    BeginNextAction(mActionQueue.Dequeue());
                }
                else
                {
                    Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.ActionResolved));
                }
            }
        }

        void ApplyActionResults()
        {
            mActionResult.Initiator.ApplyStateChange(mActionResult.InitiatorResult.StateChange);

            foreach (var actorResultPair in mActionResult.TargetResults)
            {
                EncounterModule.CharacterDirector.ApplyStateChange(actorResultPair.Key, actorResultPair.Value.StateChange);
            }
        }

        public void IncrementDelayedActions()
        {
            foreach (ActionProposal proposal in mDelayedActions.Keys.ToList())
            {
                mDelayedActions[proposal]--;
            }
        }
    }
}

