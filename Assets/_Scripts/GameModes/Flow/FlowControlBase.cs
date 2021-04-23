using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAGE.Input;
using MAGE.Messaging;
using UnityEngine;

namespace MAGE.GameModes.FlowControl
{
    enum FlowControlId
    {
        Debug,

        Cinematic,
        Encounter,
        EncounterActionDirector,
        EncounterIntroFlowControl,
        EncounterUnitPlacementFlowControl,
        EncounterPlayerTurnFlowControl,
        EncounterAITurnFlowControl,
        Exploration,
        ExplorationInteractionFlowControl,
        ExplorationMenuFlowControl,
        ExplorationRoamFlowControl,
        ExplorationOutfiterFlowControl,
        Level,
        MainMenu,
        Map,

        NUM
    }

    abstract class FlowControlBase
        : MonoBehaviour
        , IInputHandler
        , IMessageHandler
    {
        public abstract FlowControlId GetFlowControlId();

        public void Init()
        {
            Setup();
            Input.InputManager.Instance.RegisterHandler(this, false);
            MessageRouter.Instance.RegisterHandler(this);
        }
        protected abstract void Setup();

        public void Takedown()
        {
            Input.InputManager.Instance.ReleaseHandler(this);
            MessageRouter.Instance.UnRegisterHandler(this);
            Cleanup();    
        }
        protected abstract void Cleanup();

        // Input Handling
        public virtual void OnMouseHoverChange(GameObject mouseHover)
        {
            // empty
        }

        public virtual void OnKeyPressed(InputSource source, int key, InputState state)
        {
            // empty
        }

        public virtual void OnMouseScrolled(float scrollDelta)
        {
            // empty
        }

        public virtual void HandleMessage(MessageInfoBase eventInfoBase)
        {
            // empty
        }

        public virtual bool Notify(string notifyEvent)
        {
            return false;
        }

        public virtual string Query(string queryEvent)
        {
            return "";
        }

        public virtual string Condition(string queryEvent)
        {
            return "";
        }

        protected void SendFlowMessage(string action)
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new FlowMessage(FlowMessage.EventType.FlowEvent, action));
        }
    }
}
