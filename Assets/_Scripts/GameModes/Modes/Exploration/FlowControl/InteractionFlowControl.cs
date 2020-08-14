using MAGE.GameModes.LevelManagement;
using MAGE.GameModes.SceneElements;
using MAGE.GameServices.Character;
using MAGE.GameServices;
using MAGE.GameServices.World;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CharacterInfo = MAGE.GameServices.Character.CharacterInfo;
using MAGE.GameModes.Exploration;

namespace MAGE.GameModes.FlowControl
{
    class InteractionFlowControl
        : MonoBehaviour
        , IInputHandler
        , UIContainerControl
        , Messaging.IMessageHandler
    {
        enum InteractionState
        {
            INVALID = -1,

            NPCActionSelect,
            Vendor,
            Conversation,
            Inspect,

            NUM
        }


        enum NPCAction
        {
            Vendor,
            Conversation
        }

        enum VendorState
        {
            Buy,
            Sell
        }


        private string TAG = "InteractionFlowControl";
        private float mMinInteractionDistance = 2;

        private float mDistanceToHovered = 0;
        private ThirdPersonActorController mExplorationActor = null;
        private PropBase mHoveredInteractable = null;
        private PropBase mInteractingWith = null;
        private InteractionState mInteractionState = InteractionState.INVALID;

        // NPC Actions
        private List<KeyValuePair<NPCAction, int>> mNPCActions = new List<KeyValuePair<NPCAction, int>>();

        // Conversations
        private Conversation mConversation;
        private int mDialogueIdx = 0;

        // Inspect
        private List<Item> mInventory = new List<Item>();

        // Vendor
        private VendorState mVendorState = VendorState.Buy;

        public void Init(ThirdPersonActorController exploring)
        {
            mExplorationActor = exploring;
            Input.InputManager.Instance.RegisterHandler(this, false);
            Messaging.MessageRouter.Instance.RegisterHandler(this);
        }

        public void CleanUp()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
            Input.InputManager.Instance.ReleaseHandler(this);
            mHoveredInteractable = null;
        }

        void Update()
        {
            if (mHoveredInteractable != null)
            {
                bool wasPreviousInRange = IsInRange();
                mDistanceToHovered = DistanceToHoverTarget();
                bool isNowInRange = IsInRange();

                if (wasPreviousInRange != isNowInRange)
                {
                    UpdateHoverTargetCursor();
                }
            }
        }

        #region UIContainerControl
        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch ((UIContainerId)containerId)
            {
                case UIContainerId.NPCActionSelectView:
                {
                    HandleNPCActionsInteraction(interactionInfo);
                }
                break;
                case UIContainerId.ConversationView:
                {
                    HandleConversationInteraction(interactionInfo);
                }
                break;
                case UIContainerId.ContainerInspectView:
                {
                    HandleContainerInspectInteraction(interactionInfo);
                }
                break;
                case UIContainerId.VendorView:
                {
                    HandleVendorViewInteraction(interactionInfo);
                }
                break;
            }
        }

        private void HandleNPCActionsInteraction(UIInteractionInfo interactionInfo)
        {
            if (interactionInfo.InteractionType == UIInteractionType.Click)
            {
                if (interactionInfo.ComponentId == (int)NPCActionSelectView.ComponentId.Exit)
                {
                    UIManager.Instance.RemoveOverlay(UIContainerId.NPCActionSelectView);
                    InteractionComplete();
                }
                else if (interactionInfo.ComponentId == (int)NPCActionSelectView.ComponentId.ActionSelect)
                {
                    ListInteractionInfo listInfo = interactionInfo as ListInteractionInfo;

                    if (mNPCActions[listInfo.ListIdx].Key == NPCAction.Vendor)
                    {
                        mInteractionState = InteractionState.Vendor;

                        UIManager.Instance.RemoveOverlay(UIContainerId.NPCActionSelectView);
                        UIManager.Instance.PostContainer(UIContainerId.VendorView, this);
                    }
                    else if (mNPCActions[listInfo.ListIdx].Key == NPCAction.Conversation)
                    {
                        mInteractionState = InteractionState.Conversation;

                        UIManager.Instance.RemoveOverlay(UIContainerId.NPCActionSelectView);

                        PropInfo propInfo = LevelManagementService.Get().GetPropInfo(mInteractingWith.GetPropId());
                        mConversation = propInfo.Conversations[mNPCActions[listInfo.ListIdx].Value];
                        mDialogueIdx = 0;
                        UIManager.Instance.PostContainer(UIContainerId.ConversationView, this);
                    }
                }
            }
        }

        void HandleConversationInteraction(UIInteractionInfo interactionInfo)
        {
            if (interactionInfo.InteractionType == UIInteractionType.Click)
            {
                if (interactionInfo.ComponentId == (int)ConversationView.ComponentId.ContinueBtn)
                {
                    if (mDialogueIdx < mConversation.DialogueChain.Count - 1)
                    {
                        mDialogueIdx++;
                        UIManager.Instance.Publish(UIContainerId.ConversationView);
                    }
                    else
                    {
                        mInteractionState = InteractionState.NPCActionSelect;

                        WorldService.Get().NotifyConversationComplete(mConversation.ConversationId);
                        UIManager.Instance.RemoveOverlay(UIContainerId.ConversationView);
                        UIManager.Instance.PostContainer(UIContainerId.NPCActionSelectView, this);
                    }
                }
            }
        }

        private void HandleContainerInspectInteraction(UIInteractionInfo interactionInfo)
        {
            if (interactionInfo.InteractionType == UIInteractionType.Click)
            {
                if (interactionInfo.ComponentId == (int)ContainerInspectView.ComponentId.Exit)
                {
                    UIManager.Instance.RemoveOverlay(UIContainerId.ContainerInspectView);
                    InteractionComplete();
                }
                else if (interactionInfo.ComponentId == (int)ContainerInspectView.ComponentId.ItemBtns)
                {
                    ListInteractionInfo listInfo = interactionInfo as ListInteractionInfo;

                    PropInfo propInfo = LevelManagementService.Get().GetPropInfo(mInteractingWith.GetPropId());
                    Item item = propInfo.Inventory[listInfo.ListIdx];
                    propInfo.Inventory.RemoveAt(listInfo.ListIdx);
                    LevelManagementService.Get().UpdatePropInfo(propInfo);

                    WorldService.Get().AddToInventory(item.ItemTag.ItemId);

                    UIManager.Instance.Publish(UIContainerId.ContainerInspectView);
                }
            }
        }

        private void HandleVendorViewInteraction(UIInteractionInfo interactionInfo)
        {
            if (interactionInfo.InteractionType == UIInteractionType.Click)
            {
                if (interactionInfo.ComponentId == (int)VendorView.ComponentId.Exit)
                {
                    UIManager.Instance.RemoveOverlay(UIContainerId.VendorView);
                    UIManager.Instance.PostContainer(UIContainerId.NPCActionSelectView, this);
                }
                else if (interactionInfo.ComponentId == (int)VendorView.ComponentId.BuyBtn)
                {
                    if (mVendorState != VendorState.Buy)
                    {
                        mVendorState = VendorState.Buy;
                        UIManager.Instance.Publish(UIContainerId.VendorView);
                    }
                }
                else if (interactionInfo.ComponentId == (int)VendorView.ComponentId.SellBtn)
                {
                    if (mVendorState != VendorState.Sell)
                    {
                        mVendorState = VendorState.Sell;
                        UIManager.Instance.Publish(UIContainerId.VendorView);
                    }
                }
                else if (interactionInfo.ComponentId == (int)VendorView.ComponentId.ItemBtns)
                {
                    ListInteractionInfo listInfo = interactionInfo as ListInteractionInfo;
                    int itemIdx = listInfo.ListIdx;

                    // TODO: add how many of an item the vendor has
                    if (mVendorState == VendorState.Buy)
                    {
                        PropInfo propInfo = LevelManagementService.Get().GetPropInfo(mInteractingWith.GetPropId());
                        Item purchasedItem = propInfo.Inventory[itemIdx];
                        WorldService.Get().PurchaseItem(100, purchasedItem.ItemTag.ItemId);
                    }
                    else
                    {
                        int soldItem = WorldService.Get().GetInventory().Items.Keys.ToList()[itemIdx];
                        WorldService.Get().SellItem(100, soldItem);
                    }

                    UIManager.Instance.Publish(UIContainerId.VendorView);
                }
            }
        }

        public string Name()
        {
            return TAG;
        }

        public IDataProvider Publish(int containerId)
        {
            IDataProvider dataProvider = null;

            switch ((UIContainerId)containerId)
            {
                case UIContainerId.NPCActionSelectView:
                {
                    dataProvider = PublishNPCActions();
                }
                break;
                case UIContainerId.ConversationView:
                {
                    dataProvider = PublishConversation();
                }
                break;
                case UIContainerId.ContainerInspectView:
                {
                    dataProvider = PublishContainerInspect();
                }
                break;
                case UIContainerId.VendorView:
                {
                    dataProvider = PublishVendor();
                }
                break;
            }

            return dataProvider;
        }

        IDataProvider PublishConversation()
        {
            ConversationView.DataProvider dataProvider = new ConversationView.DataProvider();

            Dialogue currentDialogue = mConversation.DialogueChain[mDialogueIdx];

            string assetName = "";
            string speakerName = "";

            int speakerId = mConversation.SpeakerIds[currentDialogue.SpeakerIdx];
            if (speakerId == ConversationConstants.CONVERSATION_OWNER_ID)
            {
                PropInfo propInfo = LevelManagementService.Get().GetPropInfo(mInteractingWith.GetPropId());
                Appearance appearance = LevelManagementService.Get().GetAppearance(propInfo.AppearanceId);
                assetName = appearance.PortraitSpriteId.ToString();
                speakerName = propInfo.Name;
            }
            else if (speakerId == ConversationConstants.PARTY_AVATAR_ID)
            {
                int partyAvatarId = WorldService.Get().GetPartyAvatarId();
                CharacterInfo characterInfo = CharacterService.Get().GetCharacterInfo(partyAvatarId);
                Appearance appearance = LevelManagementService.Get().GetAppearance(characterInfo.AppearanceId);
                speakerName = characterInfo.Name;
                assetName = appearance.PortraitSpriteId.ToString();
            }

            dataProvider.Name = speakerName;
            dataProvider.PortraitAssetName = assetName;
            dataProvider.Content = currentDialogue.Content;

            return dataProvider;
        }

        IDataProvider PublishNPCActions()
        {
            NPCActionSelectView.DataProvider dataProvider = new NPCActionSelectView.DataProvider();

            PropInfo propInfo = LevelManagementService.Get().GetPropInfo(mInteractingWith.GetPropId());
            dataProvider.Name = propInfo.Name;
            Appearance appearance = LevelManagementService.Get().GetAppearance(propInfo.AppearanceId);
            dataProvider.PortraitAssetName = appearance.PortraitSpriteId.ToString();

            mNPCActions.Clear();
            dataProvider.NPCActions = new List<string>();
            if (propInfo.Inventory.Count > 0)
            {
                dataProvider.NPCActions.Add("Buy/Sell");
                mNPCActions.Add(new KeyValuePair<NPCAction, int>(NPCAction.Vendor, -1));
            }

            int conversationIdx = 0;
            foreach (Conversation conversation in propInfo.Conversations)
            {
                dataProvider.NPCActions.Add(conversation.Header);
                mNPCActions.Add(new KeyValuePair<NPCAction, int>(NPCAction.Conversation, conversationIdx++));
            }

            return dataProvider;
        }

        IDataProvider PublishContainerInspect()
        {
            ContainerInspectView.DataProvider dataProvider = new ContainerInspectView.DataProvider();

            PropInfo propInfo = LevelManagementService.Get().GetPropInfo(mInteractingWith.GetPropId());
            dataProvider.ItemNames = new List<string>();
            dataProvider.ItemIconAssetNames = new List<string>();
            foreach (Item item in propInfo.Inventory)
            {
                dataProvider.ItemNames.Add(item.Name);
                dataProvider.ItemIconAssetNames.Add(item.SpriteId.ToString());
            }

            return dataProvider;
        }

        IDataProvider PublishVendor()
        {
            VendorView.DataProvider dataProvider = new VendorView.DataProvider();


            dataProvider.Currency = WorldService.Get().GetCurrency();

            dataProvider.ItemNames = new List<string>();
            dataProvider.ItemIconAssetNames = new List<string>();

            PropInfo propInfo = LevelManagementService.Get().GetPropInfo(mInteractingWith.GetPropId());
            if (mVendorState == VendorState.Buy)
            {
                foreach (Item item in propInfo.Inventory)
                {
                    dataProvider.ItemNames.Add(item.Name);
                    dataProvider.ItemIconAssetNames.Add(item.SpriteId.ToString());
                }
            }
            else
            {
                foreach (var itemCountPair in WorldService.Get().GetInventory().Items)
                {
                    Item item = ItemFactory.LoadItem(itemCountPair.Key);
                    dataProvider.ItemNames.Add(string.Format("{0} x{1}", item.Name, itemCountPair.Value));
                    dataProvider.ItemIconAssetNames.Add(item.SpriteId.ToString());
                }
            }

            return dataProvider;
        }

        #endregion // UIContainerControl

        //! IInputHandler
        public void OnKeyPressed(InputSource source, int key, InputState state)
        {
            if (!Interacting())
            {
                switch (source)
                {
                    case InputSource.Mouse:
                    {
                        if (key == (int)MouseKey.Right
                            && state == InputState.Down
                            && mHoveredInteractable != null
                            && IsInRange()
                            && mHoveredInteractable.IsPropInteractable())
                        {
                            InteractionStart(mHoveredInteractable);
                        }
                    }
                    break;
                }
            }
        }

        public void OnMouseHoverChange(GameObject mouseHover)
        {
            if (!Interacting())
            {
                if (mouseHover != null)
                {
                    mHoveredInteractable = mouseHover.GetComponentInParent<PropBase>();
                    mDistanceToHovered = DistanceToHoverTarget();
                }
                else
                {
                    mHoveredInteractable = null;
                }

                UpdateHoverTargetCursor();
            }
        }

        public void OnMouseScrolled(float scrollDelta)
        {
            // empty
        }

        

        private void UpdateHoverTargetCursor()
        {
            if (mHoveredInteractable != null && mHoveredInteractable.IsPropInteractable())
            {
                float distanceTo = DistanceToHoverTarget();
                if (distanceTo < mMinInteractionDistance)
                {
                    UIManager.Instance.SetCursor(CursorControl.CursorType.Interact_Near);
                }
                else
                {
                    UIManager.Instance.SetCursor(CursorControl.CursorType.Interact_Far);
                }
            }
            else
            {
                UIManager.Instance.SetCursor(CursorControl.CursorType.Default);
            }

        }

        float DistanceToHoverTarget()
        {
            return (mExplorationActor.transform.position - mHoveredInteractable.gameObject.transform.position).magnitude;
        }

        bool IsInRange()
        {
            return mDistanceToHovered < mMinInteractionDistance;
        }
        bool Interacting()
        {
            return mInteractingWith != null;
        }

        private void InteractionStart(PropBase propBase)
        {
            mHoveredInteractable = null;

            switch (propBase.GetPropType())
            {
                case PropType.NPC:
                {
                    mInteractionState = InteractionState.NPCActionSelect;

                    mInteractingWith = propBase;
                    Messaging.MessageRouter.Instance.NotifyMessage(new ExplorationMessage(ExplorationMessage.EventType.InteractionStart, mInteractingWith));
                    ExplorationModule.Instance.MovementDirector.RotateActor(mExplorationActor.ActorController, mInteractingWith.transform, null);
                    mExplorationActor.Enable(false);
                    Camera.main.gameObject.GetComponent<ThirdPersonCamera>().Interact(mExplorationActor.transform, mInteractingWith.transform);
                    UIManager.Instance.SetCursor(CursorControl.CursorType.Default);
                    UIManager.Instance.PostContainer(UIContainerId.NPCActionSelectView, this);
                }
                break;

                case PropType.Container:
                {
                    mInteractionState = InteractionState.Inspect;

                    InteractWithContainer(propBase);
                }
                break;
            }
        }

        private void InteractionComplete()
        {
            mInteractionState = InteractionState.INVALID;

            mInteractingWith.OnInteractionEnd();
            mInteractingWith = null;
            mExplorationActor.Enable(true);
            Camera.main.gameObject.GetComponent<ThirdPersonCamera>().Follow(mExplorationActor.transform);
            Messaging.MessageRouter.Instance.NotifyMessage(new ExplorationMessage(ExplorationMessage.EventType.InteractionEnd));
        }

        private void InteractWithContainer(PropBase propBase)
        {
            mInteractingWith = propBase;
            Messaging.MessageRouter.Instance.NotifyMessage(new ExplorationMessage(ExplorationMessage.EventType.InteractionStart, mInteractingWith));
            ExplorationModule.Instance.MovementDirector.RotateActor(mExplorationActor.ActorController, mInteractingWith.transform, null);
            mExplorationActor.Enable(false);
            Camera.main.gameObject.GetComponent<ThirdPersonCamera>().Interact(mExplorationActor.transform, mInteractingWith.transform);
            UIManager.Instance.SetCursor(CursorControl.CursorType.Default);
            UIManager.Instance.PostContainer(UIContainerId.ContainerInspectView, this);
            propBase.OnInteractionStart();
        }

        // IMessageHandler
        public void HandleMessage(Messaging.MessageInfoBase messageInfoBase)
        {
            switch (messageInfoBase.MessageId)
            {
                case LevelManagement.LevelMessage.Id:
                {
                    //LevelMessage message = messageInfoBase as LevelMessage;
                    //switch (message.Type)
                    //{
                    //    case MessageType.PropUpdated:
                    //    {
                    //        if (mInteractingWith != null 
                    //            && mInteractingWith.GetPropId() == message.Arg<int>())
                    //        {
                    //            if (mInteractionState == InteractionState.Inspect)
                    //            {
                    //                UIManager.Instance.Publish(UIContainerId.ContainerInspectView);
                    //            }
                    //            else if (mInteractionState == InteractionState.NPCActionSelect)
                    //            {
                    //                UIManager.Instance.Publish(UIContainerId.NPCActionSelectView);
                    //            }
                    //        }
                    //    }
                    //    break;
                    //}

                }
                break;
            }
            
        }
    }
}


