using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.FlowControl
{
    class MainFlow : FlowGraph
    {
        public MainFlow()
            : base("MainFlow")
        {
            OnEnterActions = new List<FlowActionBase>()
            {
                new LoadFlowControl(FlowControlId.Debug)
            };

             OnExitActions = new List<FlowActionBase>()
            {
                new UnLoadFlowControl(FlowControlId.Debug)
            };
            
            States = new List<FlowNode>()
            {
                new FlowNode("AutoStartGame")
                {
                    OnEnterActions = new List<FlowActionBase>()
                    {
                        new Condition("skipMainMenu")
                    }
                    , Transitions = new Dictionary<string, string>()
                    {
                        { "true", "DebugInit" }
                        , { "false", "MainMenu" }
                    }
                }
                , new FlowNode("DebugInit")
                {
                    OnEnterActions = new List<FlowActionBase>()
                    {
                        new Notify("debugInit")
                        , new Invoke("advance")
                    }
                    , Transitions = new Dictionary<string, string>()
                    {
                        { "advance", "LevelFlow" }
                    }
                }
                , new FlowNode("MainMenu")
                {
                     OnEnterActions = new List<FlowActionBase>()
                     {
                         new LoadFlowControl(FlowControlId.MainMenu)
                     }
                     , OnExitActions = new List<FlowActionBase>()
                     {
                         new UnLoadFlowControl(FlowControlId.MainMenu)
                     }
                     , Transitions = new Dictionary<string, string>()
                     {
                         { "advance", "LevelFlow" }
                     }
                }
                , new FlowNode("LevelFlow")
                {
                    OnEnterActions = new List<FlowActionBase>()
                    {
                        new LoadFlowControl(FlowControlId.Level)
                    }
                    , OnExitActions = new List<FlowActionBase>()
                    {
                        new Notify("unloadLevel")
                        , new Notify("fadeOut")
                        , new UnLoadFlowControl(FlowControlId.Level)
                    }
                    , Transitions = new Dictionary<string, string>()
                    {
                        { "encounter", "EncounterFlow" }
                        ,{ "explore", "ExplorationFlow" }
                        ,{ "cinematic", "CinematicFlow" }
                    }
                    , States = new List<FlowNode>()
                    {
                        new FlowNode("LoadLevel")
                        {
                            OnEnterActions = new List<FlowActionBase>()
                            {
                                new Notify("loadLevel")
                            }
                            , Transitions = new Dictionary<string, string>()
                            {
                                { "levelLoaded", "QueryFlowType" }
                            }
                        }
                        , new FlowNode("QueryFlowType")
                        {
                            OnEnterActions = new List<FlowActionBase>()
                            {
                                new Query("levelFlowType")
                            }
                        }
                        , new ExplorationFlow()
                        {
                            ExternalTransitions = new Dictionary<string, string>()
                            {
                                { OutAdvance, "QueryFlowType" }
                            }
                        }
                        , new FlowNode("CinematicFlow")
                        {
                            States = new List<FlowNode>()
                            {
                                new FlowNode("PlayCinematic")
                                {
                                    OnEnterActions = new List<FlowActionBase>()
                                    {
                                        new Notify("fadeIn")
                                        , new LoadFlowControl(FlowControlId.Cinematic)
                                    }
                                    , OnExitActions = new List<FlowActionBase>()
                                    {
                                        new UnLoadFlowControl(FlowControlId.Cinematic)
                                    }
                                    , Transitions = new Dictionary<string, string>()
                                    {
                                        { "advance", "FadeOut" }
                                    }
                                }
                                , new WaitState("FadeOut", "fadeOut", "fadeComplete")
                                {
                                    ExternalTransitions = new Dictionary<string, string>()
                                    {
                                        {FlowGraph.OutAdvance, "QueryFlowType" }
                                    }
                                }
                            }
                        }
                        , new EncounterFlow()
                        {
                            ExternalTransitions = new Dictionary<string, string>()
                            {
                                { OutAdvance, "QueryFlowType" }
                            }
                        }
                        //, new FlowNode("Encounter")
                        //{
                        //    OnEnterActions = new List<FlowActionBase>()
                        //    {
                        //        new LoadFlowControl(FlowControlId.Exploration)
                        //    }
                        //    , OnExitActions = new List<FlowActionBase>()
                        //    {
                        //        new UnLoadFlowControl(FlowControlId.Exploration)
                        //    }
                        //} 
                        //, new FlowNode("Explore")
                        //{
                        //    OnEnterActions = new List<FlowActionBase>()
                        //    {
                        //        new LoadFlowControl(FlowControlId.Exploration)
                        //    }
                        //    , OnExitActions = new List<FlowActionBase>()
                        //    {
                        //        new UnLoadFlowControl(FlowControlId.Exploration)
                        //    }
                        //}
                    }
                }
            };
        }
    }
}
