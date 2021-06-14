﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.FlowControl
{
    class EncounterFlow : FlowGraph
    {
        public EncounterFlow()
            : base("EncounterFlow")
        {
            Transitions = new Dictionary<string, string>()
            {
                { "forceWin", "ResultsFlow" }
                ,{ "forceLoss", "ResultsFlow" }
            };

            OnEnterActions = new List<FlowActionBase>()
            {
                new LoadFlowControl(FlowControlId.Encounter)
            };

            OnExitActions = new List<FlowActionBase>()
            {
                new UnLoadFlowControl(FlowControlId.Encounter)
            };

            States = new List<FlowNode>()
            {
                new FlowNode("LoadFlow")
                {
                    OnEnterActions = new List<FlowActionBase>()
                    {
                        new LoadFlowControl(FlowControlId.EncounterLoadFlowControl)
                    }
                    ,OnExitActions = new List<FlowActionBase>()
                    {
                        new UnLoadFlowControl(FlowControlId.EncounterLoadFlowControl)
                    }
                    , Transitions = new Dictionary<string, string>()
                    {
                        { "encounterLoaded", "FadeIn" }
                    }
                }
                ,new WaitState("FadeIn", "fadeIn", "fadeComplete")
                {
                    ExternalTransitions = new Dictionary<string, string>()
                    {
                        { OutAdvance, "Combat" }
                    }
                }
                //, new FlowNode("IntroView")
                //{
                //    OnEnterActions = new List<FlowActionBase>()
                //    {
                //        new LoadFlowControl(FlowControlId.EncounterIntroFlowControl)
                //    }
                //    , OnExitActions= new List<FlowActionBase>()
                //    {
                //        new UnLoadFlowControl(FlowControlId.EncounterIntroFlowControl)
                //    }
                //    ,Transitions = new Dictionary<string, string>()
                //    {
                //        { "advance", "TurnResolution" }
                //    }
                //}
                , new FlowNode("Combat")
                {
                    OnEnterActions = new List<FlowActionBase>()
                    {
                        new LoadFlowControl(FlowControlId.EncounterActionDirector)
                        , new Notify("encounterStarted")
                    }
                    , OnExitActions = new List<FlowActionBase>()
                    {
                        new UnLoadFlowControl(FlowControlId.EncounterActionDirector)
                    }
                    , States = new List<FlowNode>()
                    {
                        new FlowNode("QueryFlowState")
                        {
                            OnEnterActions = new List<FlowActionBase>()
                            {
                                new Query("encounterFlowState")
                            }
                            , Transitions = new Dictionary<string, string>()
                            {
                                { "playerTurnFlow", "TurnFlow" }
                                ,{ "progressClock", "ProgressClock" }
                                ,{ "actionResolution", "ActionResolution" }
                                ,{ "encounterComplete", "ResultsFlow" }
                            }
                        }
                        , new FlowNode("ProgressClock")
                        {
                            OnEnterActions = new List<FlowActionBase>()
                            {
                                new Notify("progressClock")
                                , new Invoke("advance")
                            }
                            ,Transitions = new Dictionary<string, string>()
                            {
                                { "advance", "QueryFlowState" }
                            }
                        }
                        , new FlowNode("TurnFlow")
                        {
                            Transitions = new Dictionary<string, string>()
                            {
                                { "actionChosen", "QueryFlowState" }
                            }
                            , States = new List<FlowNode>()
                            {
                                new FlowNode("BranchOnControlledBy")
                                {
                                    OnEnterActions = new List<FlowActionBase>()
                                    {
                                        new Condition("isCurrentTurnPlayControlled")
                                    }
                                    , Transitions = new Dictionary<string, string>()
                                    {
                                        { "true", "PlayerTurnFlow" }
                                        ,{ "false", "AITurnFlow" }
                                    }
                                }
                                , new FlowNode("PlayerTurnFlow")
                                {
                                    OnEnterActions = new List<FlowActionBase>()
                                    {
                                        new LoadFlowControl(FlowControlId.EncounterPlayerTurnFlowControl)
                                    }
                                    , OnExitActions = new List<FlowActionBase>()
                                    {
                                        new UnLoadFlowControl(FlowControlId.EncounterPlayerTurnFlowControl)
                                    }
                                }
                                , new FlowNode("AITurnFlow")
                                {
                                    OnEnterActions = new List<FlowActionBase>()
                                    {
                                        new LoadFlowControl(FlowControlId.EncounterAITurnFlowControl)
                                    }
                                    , OnExitActions = new List<FlowActionBase>()
                                    {
                                        new UnLoadFlowControl(FlowControlId.EncounterAITurnFlowControl)
                                    }
                                    
                                }
                            }
                        }
                        , new FlowNode("ActionResolution")
                        {
                            OnEnterActions = new List<FlowActionBase>()
                            {
                                new Notify("resolveAction")
                            }
                            ,Transitions = new Dictionary<string, string>()
                            {
                                { "actionResolved", "QueryFlowState" }
                            }
                        }
                    }
                }
                , new FlowNode("ResultsFlow")
                {
                    OnEnterActions = new List<FlowActionBase>()
                    {
                        new LoadFlowControl(FlowControlId.EncounterResultsFlowControl)
                        , new Notify("encounterComplete")
                    }
                    , OnExitActions = new List<FlowActionBase>()
                    {
                        new UnLoadFlowControl(FlowControlId.EncounterResultsFlowControl)
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
                        { OutAdvance, OutAdvance }
                    }
                }
            };
        }
    }    
}
