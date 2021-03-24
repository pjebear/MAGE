using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    enum ActionResponseType
    {
        StateChange,
        ActionProposal,
        ActionProposalDeprecated,
    }

    abstract class ActionResponseBase
    {
        public ActionResponseType ResponseType;
        protected ActionResponseBase(ActionResponseType actionResponseType)
        {
            ResponseType = actionResponseType;
        }
    }

    class StateChangeResponse : ActionResponseBase
    {
        public StateChange Response = StateChange.Empty;
        public StateChangeResponse(StateChange response)
            : base(ActionResponseType.StateChange)
        {
            Response = response;
        }
    }

    class ActionProposalResponse : ActionResponseBase
    {
        public ActionProposal Response = null;
        public ActionProposalResponse(ActionProposal response)
            : base(ActionResponseType.ActionProposal)
        {
            Response = response;
        }
    }

    class ActionProposalResponse_Deprecated : ActionResponseBase
    {
        public ActionProposal_Deprecated Response = null;
        public ActionProposalResponse_Deprecated(ActionProposal_Deprecated response)
            : base(ActionResponseType.ActionProposalDeprecated)
        {
            Response = response;
        }
    }

}
