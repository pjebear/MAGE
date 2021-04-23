using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAGE.Messaging;
using UnityEngine;

namespace MAGE.GameModes.FlowControl
{
    class FlowManager
        : MonoBehaviour
        , Messaging.IMessageHandler
        
    {
        private static string TAG = "FlowManager";

        private List<FlowNode> mFlowStack = new List<FlowNode>();
        private Dictionary<FlowControlId, FlowControlBase> mFlowControls = new Dictionary<FlowControlId, FlowControlBase>();
        private List<FlowControlBase> mFlowControlStack = new List<FlowControlBase>();

        public void Init()
        {
            Messaging.MessageRouter.Instance.RegisterHandler(this);
        }

        public void Cleanup()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        public void BeginFlow(FlowNode flowNode)
        {
            PushFlowNode(flowNode);
        }

        public void HandleMessage(MessageInfoBase eventInfoBase)
        {
            if (eventInfoBase.MessageId == FlowMessage.Id)
            {
                FlowMessage message = eventInfoBase as FlowMessage;
                switch (message.Type)
                {
                    case FlowMessage.EventType.FlowEvent:
                    {
                        HandleEvent(message.Arg<string>());
                    }
                    break;
                    case FlowMessage.EventType.Notify:
                    {
                        Notify(message.Arg<string>());
                    }
                    break;
                    case FlowMessage.EventType.Query:
                    {
                        Query(message.Arg<string>());
                    }
                    break;
                    case FlowMessage.EventType.Condition:
                    {
                        Condition(message.Arg<string>());
                    }
                    break;
                    case FlowMessage.EventType.Invoke:
                    {
                        HandleEvent(message.Arg<string>());
                    }
                    break;
                    case FlowMessage.EventType.LoadFlowControl:
                    {
                        LoadFlowControl(message.Arg<FlowControlId>());
                    }
                    break;
                    case FlowMessage.EventType.UnLoadFlowControl:
                    {
                        UnLoadFlowControl(message.Arg<FlowControlId>());
                    }
                    break;
                }
            }
        }

        protected void HandleEvent(string arg)
        {
            Logger.Log(LogTag.FlowControl, TAG, string.Format("HandleEvent() - {0}", arg));

            string destination = "";

            int eventHandledAtNodeIdx = FindNodeIdxWithTransition(arg);
            Logger.Assert(eventHandledAtNodeIdx != -1, LogTag.FlowControl, TAG, string.Format("HandleEvent() - Failed to find flow that handles event [{0}]", arg));
            if (eventHandledAtNodeIdx != -1)
            {
                while (mFlowStack.Count > eventHandledAtNodeIdx + 1)
                {
                    PopFlowNode();
                }

                FlowNode currentNode = mFlowStack[mFlowStack.Count - 1];
                destination = currentNode.Transitions[arg];

                while (true)
                {
                    FlowGraph parent = GetParentGraph();
                    if (parent != null && parent.ExternalTransitions.ContainsKey(destination))
                    {
                        destination = parent.ExternalTransitions[destination];
                        Logger.Assert(destination != "", LogTag.FlowControl, TAG,
                            string.Format("HandleEvent() - External transition [{0}] not connected for FlowGraph [{1}]", destination, parent.Name), LogLevel.Error);

                        PopToNode(parent);
                        PopFlowNode();
                    }
                    else
                    {
                        break;
                    }
                }
                
                Logger.Assert(destination != "", LogTag.FlowControl, TAG, string.Format("HandleEvent() - Failed to find destination node [{0}] from transition [{1}] in invalid state", arg, destination), LogLevel.Error);
                if (destination != "")
                {
                    int parentGraphIndex = GetIndexOfParentGraph();
                    List<FlowNode> newNodeStack = mFlowStack.GetRange(0, parentGraphIndex);
                    newNodeStack.AddRange(GetNodeStackEndingInState(destination, mFlowStack[parentGraphIndex]));

                    int stackDivergenceIdx = -1;
                    for (int i = parentGraphIndex; i < mFlowStack.Count; ++i)
                    {
                        if (i >= newNodeStack.Count)
                        {
                            stackDivergenceIdx = i;
                            break;
                        }
                        else if (newNodeStack[i] != mFlowStack[i])
                        {
                            stackDivergenceIdx = i;
                            break;
                        }
                    }

                    if (stackDivergenceIdx != -1)
                    {
                        while (mFlowStack.Count > stackDivergenceIdx)
                        {
                            PopFlowNode();
                        }
                    }

                    while (mFlowStack.Count < newNodeStack.Count)
                    {
                        PushFlowNode(newNodeStack[mFlowStack.Count]);
                    }
                }
            }
                
            
            
        }

        private void PopFlowNode()
        {
            Logger.Assert(mFlowStack.Count > 0, LogTag.FlowControl, TAG, "PopFlowNode() - Stack is empty");
            if (mFlowStack.Count > 0)
            {
                FlowNode node = mFlowStack[mFlowStack.Count - 1];
                mFlowStack.RemoveAt(mFlowStack.Count - 1);
                foreach (FlowActionBase exitAction in node.OnExitActions)
                {
                    TriggerAction(exitAction);
                }
            }
        }

        private void PushFlowNode(FlowNode node, string startingStateName = "")
        {
            Logger.Log(LogTag.FlowControl, TAG, string.Format("PushFlowNode() - {0}", node.Name));

            mFlowStack.Add(node);
            foreach (FlowActionBase enterAction in node.OnEnterActions)
            {
                TriggerAction(enterAction);
            }

            FlowNode childState = null;
            if (startingStateName != "")
            {
                childState = node.States.Find(x => x.Name == startingStateName);
                Logger.Assert(childState != null, LogTag.FlowControl, TAG, string.Format("PushFlowNode() - Failed to find child node {0}", startingStateName));
            }
            else if (node.InitialState != "")
            {
                childState = node.States.Find(x => x.Name == node.InitialState);
                Logger.Assert(childState != null, LogTag.FlowControl, TAG, string.Format("PushFlowNode() - Failed to find child node {0}", node.InitialState));
            }
            else if (node.States.Count > 0)
            {
                childState = node.States[0];
            }

            if (childState != null)
            {
                PushFlowNode(childState);
            }
        }

        protected void LoadFlowControl(FlowControlId flowControlId)
        {
            Logger.Log(LogTag.FlowControl, TAG, string.Format("LoadFlowControl() - {0}", flowControlId.ToString()));

            Logger.Assert(!mFlowControls.ContainsKey(flowControlId), LogTag.FlowControl, TAG, string.Format("LoadFlowControl() - {0} Control already loaded!", flowControlId.ToString()));
            if (!mFlowControls.ContainsKey(flowControlId))
            {
                FlowControlBase flowControlBase = null;
                GameObject gameObject = new GameObject(flowControlId.ToString());
                gameObject.transform.SetParent(transform);

                switch (flowControlId)
                {
                    case FlowControlId.Debug: flowControlBase = gameObject.AddComponent<DebugFlow.DebugFlowControl>(); break;
                    case FlowControlId.Cinematic: flowControlBase = gameObject.AddComponent<CinematicFlowControl>(); break;
                    case FlowControlId.Encounter: flowControlBase = gameObject.AddComponent<Encounter.EncounterFlowControl>(); break;
                    case FlowControlId.EncounterActionDirector: flowControlBase = gameObject.AddComponent<Encounter.ActionResolutionFlowControl>(); break;
                    case FlowControlId.EncounterAITurnFlowControl: flowControlBase = gameObject.AddComponent<Encounter.AITurnFlowControl>(); break;
                    case FlowControlId.EncounterPlayerTurnFlowControl: flowControlBase = gameObject.AddComponent<Encounter.PlayerTurnFlowControl>(); break;
                    case FlowControlId.Exploration: flowControlBase = gameObject.AddComponent<ExplorationFlowControl>(); break;
                    case FlowControlId.ExplorationOutfiterFlowControl: flowControlBase = gameObject.AddComponent<PartyOutfiterViewControl>(); break;
                    case FlowControlId.ExplorationRoamFlowControl: flowControlBase = gameObject.AddComponent<Exploration.RoamFlowControl>(); break;
                    case FlowControlId.ExplorationInteractionFlowControl: flowControlBase = gameObject.AddComponent<InteractionFlowControl>(); break;
                    case FlowControlId.ExplorationMenuFlowControl: flowControlBase = gameObject.AddComponent<ExplorationMenuViewControl>(); break;
                    case FlowControlId.Level: flowControlBase = gameObject.AddComponent<LevelManagement.Internal.LevelManagerServiceImpl>(); break;
                    case FlowControlId.MainMenu: flowControlBase = gameObject.AddComponent<MainMenuViewControl>(); break;
                    case FlowControlId.Map: flowControlBase = gameObject.AddComponent<MapViewControl>(); break;
                    default:
                        Debug.Assert(false);
                        break;
                }

                mFlowControls.Add(flowControlId, flowControlBase);
                mFlowControlStack.Add(flowControlBase);
                flowControlBase.Init();
            }
        }

        protected void UnLoadFlowControl(FlowControlId flowControlId)
        {
            Logger.Log(LogTag.FlowControl, TAG, string.Format("UnLoadFlowControl() - {0}", flowControlId.ToString()));

            Logger.Assert(mFlowControls.ContainsKey(flowControlId), LogTag.FlowControl, TAG, string.Format("UnLoadFlowControl() - {0} Control wasn't loaded!", flowControlId.ToString()));
            if (mFlowControls.ContainsKey(flowControlId))
            {
                FlowControlBase flowControlBase = mFlowControls[flowControlId];
                mFlowControls.Remove(flowControlId);
                mFlowControlStack.Remove(flowControlBase);
                flowControlBase.Takedown();
                Destroy(flowControlBase.gameObject);
            }
        }

        protected void Notify(string arg)
        {
            Logger.Log(LogTag.FlowControl, TAG, string.Format("Notify() - {0}", arg));
            bool handled = false;

            for (int i = mFlowControls.Count - 1; i >= 0; --i)
            {
                handled = mFlowControlStack[i].Notify(arg);
                if (handled) break;
            }

            Logger.Assert(handled, LogTag.FlowControl, TAG, string.Format("Notify() {0} - Was not handled by any flow controls", arg));
        }

        protected void Query(string arg)
        {
            Logger.Log(LogTag.FlowControl, TAG, string.Format("Query() - {0}", arg));
            string result = "";

            for (int i = mFlowControls.Count - 1; i >= 0; --i)
            {
                result = mFlowControlStack[i].Query(arg);
                if (result != "") break;
            }

            Logger.Assert(result != "", LogTag.FlowControl, TAG, string.Format("Query() {0} - Was not handled by any flow controls", arg));
            if (result != "")
            {
                HandleEvent(result);
            }
        }

        protected void Condition(string arg)
        {
            Logger.Log(LogTag.FlowControl, TAG, string.Format("Condition() - {0}", arg));
            string result = "";

            for (int i = mFlowControls.Count - 1; i >= 0; --i)
            {
                result = mFlowControlStack[i].Condition(arg);
                if (result != "") break;
            }

            Logger.Assert(result != "", LogTag.FlowControl, TAG, string.Format("Condition() {0} - Was not handled by any flow controls", arg));
            if (result != "")
            {
                HandleEvent(result);
            }
        }

        protected void TriggerAction(FlowActionBase flowActionBase)
        {
            Logger.Log(LogTag.FlowControl, TAG, string.Format("TriggerAction() - {0}", flowActionBase.ToString()));

            flowActionBase.Trigger();
        }

        protected int FindNodeIdxWithTransition(string transition)
        {
            int nodeIdx = -1;
            for (int i = mFlowStack.Count - 1; i >= 0; --i)
            {
                FlowNode atIdx = mFlowStack[i];
                if (atIdx.Transitions.ContainsKey(transition))
                {
                    nodeIdx = i;
                    break;
                }
            }
            return nodeIdx;
        }

        protected void PopToNode(FlowNode toNode)
        {
            while (CurrentNode != toNode)
            {
                PopFlowNode();
            }
        }

        protected List<FlowNode> GetNodeStackEndingInState(string stateName, FlowNode rootNode)
        {
            List<FlowNode> nodeStack = new List<FlowNode>();

            if (rootNode.Name == stateName)
            {
                nodeStack.Add(rootNode);
            }
            else
            {
                foreach (FlowNode child in rootNode.States)
                {
                    if (child.IsParentNode)
                    {
                        if (child.Name == stateName)
                        {
                            nodeStack.Add(rootNode);
                            nodeStack.Add(child);
                            break;
                        }
                    }
                    else
                    {
                        List<FlowNode> recursiveStack = GetNodeStackEndingInState(stateName, child);
                        if (recursiveStack.Count > 0)
                        {
                            nodeStack.Add(rootNode);
                            nodeStack.AddRange(recursiveStack);
                            break;
                        }
                    }
                }
            }

            return nodeStack;
        }

        protected FlowGraph GetParentGraph()
        {
            return mFlowStack[GetIndexOfParentGraph()] as FlowGraph;
        }

        protected int GetIndexOfParentGraph()
        {
            int parentIndex = 0;

            for (int i = mFlowStack.Count - 1; i >= 0; --i)
            {
                if (mFlowStack[i].IsParentNode)
                {
                    parentIndex = i;
                    break;
                }
            }

            return parentIndex;
        }

        protected FlowNode CurrentNode
        {
            get
            {
                return mFlowStack.Last();
            }
        }
    }
}
