using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.FlowControl
{
    class ExplorationFlow : FlowGraph
    {
        public ExplorationFlow()
            : base("ExplorationFlow")
        {
            Transitions = new Dictionary<string, string>()
            {
                { "cinematicAvailable", "FadeOut" }
                ,{ "encounterAvailable", "FadeOut" }
                ,{ "travel", "FadeOut" }
            };

            OnEnterActions = new List<FlowActionBase>()
            {
                new LoadFlowControl(FlowControlId.Exploration)
            };

            OnExitActions = new List<FlowActionBase>()
            {
                new UnLoadFlowControl(FlowControlId.Exploration)
            };

            States = new List<FlowNode>()
            {
                new WaitState("FadeIn", "fadeIn", "fadeComplete")
                {
                    ExternalTransitions = new Dictionary<string, string>()
                    {
                        { OutAdvance, "RoamFlow" }
                    }
                }
                , new FlowNode("RoamFlow")
                {
                    OnEnterActions = new List<FlowActionBase>()
                    {
                        new LoadFlowControl(FlowControlId.ExplorationRoamFlowControl)
                        , new LoadFlowControl(FlowControlId.ExplorationMenuFlowControl)
                    }
                    , OnExitActions= new List<FlowActionBase>()
                    {
                        new UnLoadFlowControl(FlowControlId.ExplorationMenuFlowControl)
                        , new UnLoadFlowControl(FlowControlId.ExplorationRoamFlowControl)
                    }
                    ,Transitions = new Dictionary<string, string>()
                    {
                        { "interact", "InteractionFlow" }
                        ,{ "outfit", "OutfitFlow" }
                    }
                }
                , new FlowNode("InteractionFlow")
                {
                    OnEnterActions = new List<FlowActionBase>()
                    {
                        new LoadFlowControl(FlowControlId.ExplorationInteractionFlowControl)
                    }
                    , OnExitActions= new List<FlowActionBase>()
                    {
                         new UnLoadFlowControl(FlowControlId.ExplorationInteractionFlowControl)
                    }
                    ,Transitions = new Dictionary<string, string>()
                    {
                        { "back", "RoamFlow" }
                    }
                }
                , new FlowNode("OutfitFlow")
                {
                    OnEnterActions = new List<FlowActionBase>()
                    {
                        new LoadFlowControl(FlowControlId.ExplorationOutfiterFlowControl)
                    }
                    , OnExitActions= new List<FlowActionBase>()
                    {
                        new UnLoadFlowControl(FlowControlId.ExplorationOutfiterFlowControl)
                    }
                    ,Transitions = new Dictionary<string, string>()
                    {
                        { "advance", "RoamFlow" }
                    }
                }
                , new WaitState("FadeOut", "fadeOut", "fadeComplete")
                {
                    ExternalTransitions = new Dictionary<string, string>()
                    {
                        { OutAdvance, OutAdvance }
                    }
                }
            };
        }
    }    
}
