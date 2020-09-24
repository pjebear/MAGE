using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAGE.Messaging;

namespace MAGE.GameModes.FlowControl
{
    class WaitState : FlowGraph
    {
        public WaitState(string name, string notifyParam, string waitForParam)
            : base (name)
        {
            OnEnterActions = new List<FlowActionBase>()
            {
                new Notify(notifyParam)
            };

            Transitions = new Dictionary<string, string>()
            {
                { waitForParam, OutAdvance }
            };
        }
    }

    //class FadeFlow 
    //    : FlowControlBase
    //{
    //    public enum FadeState
    //    {
    //        In,
    //        Out,

    //        NUM
    //    }

    //    private FadeState mGoalState = FadeState.NUM;

    //    public FadeFlow(FadeState fadeState)
    //    {
    //        mGoalState = fadeState;
    //    }

    //    protected override void Setup()
    //    {
    //        // Empty
    //    }

    //    public override void Begin()
    //    {
    //        GameModeMessage.EventType message = mGoalState == FadeState.In ? GameModeMessage.EventType.FadeIn : GameModeMessage.EventType.FadeOut;
    //        Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(message));
    //    }

    //    public override void End()
    //    {
    //        // empty
    //    }

    //    protected override void Cleanup()
    //    {
    //        // empty
    //    }

    //    public override void HandleMessage(MessageInfoBase eventInfoBase)
    //    {
    //        base.HandleMessage(eventInfoBase);

    //        switch (eventInfoBase.MessageId)
    //        {
    //            case GameModeMessage.Id:
    //            {
    //                GameModeMessage gameModeMessage = eventInfoBase as GameModeMessage;
    //                if (gameModeMessage.Type == GameModeMessage.EventType.FadeComplete)
    //                {
    //                    MessageRouter.Instance.NotifyMessage(new FlowMessage(FlowMessage.EventType.Advance));
    //                }
    //            }
    //            break;
    //        }
    //    }
    //}
}
