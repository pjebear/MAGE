using MAGE.GameModes.LevelManagement;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems;
using MAGE.GameSystems.World;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CharacterInfo = MAGE.GameSystems.Characters.CharacterInfo;
using MAGE.GameModes.Exploration;
using MAGE.GameSystems.Appearances;

namespace MAGE.GameModes.FlowControl
{
    class InteractionFlowControl
        : FlowControlBase 
        , UIContainerControl
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


        private string TAG = "InteractionViewControl";
        

        private float mDistanceToHovered = 0;
        private Actor mExplorationActor = null;
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

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.ExplorationInteractionFlowControl;
        }

        protected override void Setup()
        {
            mExplorationActor = GameModel.Exploration.PartyAvatar;
            BeginInteraction(GameModel.Exploration.InteractionTarget.GetComponent<PropBase>());
        }

        protected override void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.NPCActionSelectView);
            UIManager.Instance.RemoveOverlay(UIContainerId.ContainerInspectView);
            UIManager.Instance.RemoveOverlay(UIContainerId.ConversationView);
            UIManager.Instance.RemoveOverlay(UIContainerId.VendorView);

            mInteractingWith.OnInteractionEnd();
            mInteractingWith = null;
            //mExplorationActor.SetControllerState(ActorController.ControllerState.ThirdPerson);
            //Camera.main.gameObject.GetComponent<ThirdPersonCamera>().Follow(mExplorationActor.transform);
        }

        public void BeginInteraction(PropBase propBase)
        {
            mInteractingWith = propBase;
            mInteractingWith.OnInteractionStart();

            switch (propBase.GetPropType())
            {
                case PropType.NPC:
                {
                    mInteractionState = InteractionState.NPCActionSelect;

                    GameModel.Exploration.MovementDirector.RotateActor(mExplorationActor.transform, mInteractingWith.transform, null);
                    UIManager.Instance.PostContainer(UIContainerId.NPCActionSelectView, this);
                }
                break;

                case PropType.Container:
                {
                    mInteractionState = InteractionState.Inspect;

                    InteractWithContainer(propBase);
                }
                break;

                case PropType.Door:
                {
                    mInteractionState = InteractionState.INVALID;

                    InteractionComplete();
                }
                break;
            }
        }

        private void InteractionComplete()
        {
            SendFlowMessage("back");
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
                        EndConversation();
                    }
                }
            }
        }

        private void EndConversation()
        {
            mInteractionState = InteractionState.NPCActionSelect;

            WorldService.Get().NotifyConversationComplete(mConversation.ConversationId);
            UIManager.Instance.RemoveOverlay(UIContainerId.ConversationView);
            UIManager.Instance.PostContainer(UIContainerId.NPCActionSelectView, this);
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
                Character character = CharacterService.Get().GetCharacter(partyAvatarId);
                Appearance appearance = character.Appearance;
                speakerName = character.Name;
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

            int partyFunds = WorldService.Get().GetCurrency();
            dataProvider.Currency = partyFunds;

            dataProvider.ItemNames = new List<string>();
            dataProvider.ItemIconAssetNames = new List<string>();

            PropInfo propInfo = LevelManagementService.Get().GetPropInfo(mInteractingWith.GetPropId());
            if (mVendorState == VendorState.Buy)
            {
                foreach (Item item in propInfo.Inventory)
                {
                    dataProvider.ItemNames.Add(item.Name);
                    dataProvider.ItemIconAssetNames.Add(item.SpriteId.ToString());
                    dataProvider.ItemValues.Add(item.Value);
                    dataProvider.ItemSelectability.Add(item.Value < partyFunds);
                }
            }
            else
            {
                foreach (var itemCountPair in WorldService.Get().GetInventory().Items)
                {
                    Item item = ItemFactory.LoadItem(itemCountPair.Key);
                    dataProvider.ItemNames.Add(string.Format("{0} x{1}", item.Name, itemCountPair.Value));
                    dataProvider.ItemIconAssetNames.Add(item.SpriteId.ToString());
                    dataProvider.ItemValues.Add(item.Value);
                    dataProvider.ItemSelectability.Add(item.Value != -1 && item.ItemTag.ItemType != ItemType.Story);
                }
            }

            return dataProvider;
        }

        #endregion // UIContainerControl

        public override void OnKeyPressed(InputSource source, int key, InputState state)
        {
            if (source == InputSource.Keyboard
                && key == (int)KeyCode.Escape
                && state == InputState.Down)
            {
                if (mInteractionState == InteractionState.Conversation)
                {
                    EndConversation();
                }
            }
        }

        private void InteractWithContainer(PropBase propBase)
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new ExplorationMessage(ExplorationMessage.EventType.InteractionStart, mInteractingWith));
            GameModel.Exploration.MovementDirector.RotateActor(mExplorationActor.transform, mInteractingWith.transform, null);
            //mExplorationActor.SetControllerState(ActorController.ControllerState.None);
            //Camera.main.gameObject.GetComponent<ThirdPersonCamera>().Interact(mExplorationActor.transform, mInteractingWith.transform);
            UIManager.Instance.PostContainer(UIContainerId.ContainerInspectView, this);
        }
    }
}


