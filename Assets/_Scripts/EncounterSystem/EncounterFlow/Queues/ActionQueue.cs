using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EncounterSystem
{
    using ActionUtil;
    using Common.ActionTypes;
    using Common.AttributeEnums;

    namespace EncounterFlow
    {
        namespace Queue
        {
            class ActionQueue
            {
                Dictionary<QueuedActionPayload, float> mQueuedActionMap;
                Queue<QueuedActionPayload> mReadyActions;

                public ActionQueue()
                {
                    mQueuedActionMap = new Dictionary<QueuedActionPayload, float>();
                    mReadyActions = new Queue<QueuedActionPayload>();
                }

                public void QueueAction(QueuedActionPayload actionPayload)
                {
                    if (actionPayload.Action.ActionInfo.ChargeTime == ActionConstants.InstantChargeTime)
                    {
                        mReadyActions.Enqueue(actionPayload);
                    }
                    else
                    {
                        mQueuedActionMap.Add(actionPayload, 0);
                    }
                }

                public void IncrementChargeClocks()
                {
                    var queuedActions = mQueuedActionMap.ToList();
                    for (int i = 0; i < queuedActions.Count; ++i)
                    {
                        float castClock = queuedActions[i].Value;
                        castClock += queuedActions[i].Key.Actor.GetChargeSpeedForAction( queuedActions[i].Key.Action);
                        if (castClock >= 100)
                        {
                            mQueuedActionMap.Remove(queuedActions[i].Key);
                            mReadyActions.Enqueue(queuedActions[i].Key);
                        }
                        else
                        {
                            mQueuedActionMap[queuedActions[i].Key] = castClock;
                        }
                    }
                }

                public bool HasReadyActions { get { return mReadyActions.Count > 0; } }
                public QueuedActionPayload NextReadyAction { get { return mReadyActions.Dequeue(); } }
            }
        }
    }
    
}

