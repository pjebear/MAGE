using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.FlowControl
{
    abstract class FlowActionBase
    {
        protected FlowMessage.EventType mAction;
        protected object mParam;

        protected FlowActionBase(FlowMessage.EventType actionType, object param)
        {
            mAction = actionType;
            mParam = param;
        }

        public void Trigger()
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new FlowMessage(mAction, mParam));
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", mAction.ToString(), mParam);
        }
    }

    class Notify : FlowActionBase
    {
        public Notify(string arg)
            : base(FlowMessage.EventType.Notify, arg)
        {
            // empty
        }
    }

    class Query : FlowActionBase
    {
        public Query(string arg)
            : base(FlowMessage.EventType.Query, arg)
        {
            // empty
        }
    }

    class LoadFlowControl : FlowActionBase
    {
        public LoadFlowControl(FlowControlId flowControlId)
            : base( FlowMessage.EventType.LoadFlowControl, flowControlId)
        {
            // empty
        }
    }

    class UnLoadFlowControl : FlowActionBase
    {
        public UnLoadFlowControl(FlowControlId flowControlId)
            : base(FlowMessage.EventType.UnLoadFlowControl, flowControlId)
        {
            // empty
        }
    }
}
