using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.FlowControl
{
    class FlowNode
    {
        public string Name = "";
        public string InitialState = "";
        public List<FlowActionBase> OnEnterActions = new List<FlowActionBase>();
        public List<FlowActionBase> OnExitActions = new List<FlowActionBase>();

        public Dictionary<string, string> Transitions = new Dictionary<string, string>();
        public bool IsParentNode = false;

        public List<FlowNode> States = new List<FlowNode>();
        public int CurrentState = -1;

        public FlowNode(string name)
        {
            Name = name;
        }
    }

    abstract class FlowGraph : FlowNode
    {
        public static readonly string OutBack = "out_back";
        public static readonly string OutAdvance = "out_advance";

        public Dictionary<string, string> ExternalTransitions = new Dictionary<string, string>();

        protected FlowGraph(string name)
            : base(name)
        {
            IsParentNode = true;
        }
    }
}
