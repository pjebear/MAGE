using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.FlowControl
{
   

    enum FlowState
    {
        INVALID = -1,

        Inactive,
        Setup,
        Running,
        Complete,
        Takedown,

        NUM
    }

    enum FlowSetup
    {
        SetupComplete,
        TakeDownComplete,

        NUM
    }

    enum FlowAction
    {
        advance,
        back,

        NUM
    }

    static class FlowConstants
    {
        public static readonly string CONDITION_TRUE = "true";
        public static readonly string CONDITION_FALSE = "false";
        public static readonly string CONDITION_EMPTY = "";
    }

}
