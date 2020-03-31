using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ActionDirector : MonoBehaviour
{
    private Timeline<ActionEvent> mActionTimeline;
    private ActionResult mActionResult;

    private Queue<ActionProposal> mActionQueue;
    private List<KeyValuePair<ActionProposal, int>> mDelayedActions;


    void Awake()
    {
        mActionQueue = new Queue<ActionProposal>();
        mDelayedActions = new List<KeyValuePair<ActionProposal, int>>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mActionTimeline != null)
        {
            ProgressTimeline(Time.deltaTime);
        }
    }

    public void DirectAction(ActionProposal proposal, int withDelay = 0)
    {
        if (withDelay > 0)
        {
            mDelayedActions.Add(new KeyValuePair<ActionProposal, int>(proposal, withDelay));
            EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.ActionResolved));
        }
        else if (withDelay == 0)
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
                EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.ActionResolved));
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
}
