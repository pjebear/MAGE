using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.Messaging;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.FlowControl
{
    class PartyOutfiterViewControl : FlowControlBase, UIContainerControl
    {
        enum OutfitState
        {
            INVALID,

            Equip,
            Specialize,

            NUM
        }
        private OutfitState mState = OutfitState.INVALID;

        private ICharacterOutfiter mOutfiter;

        private List<int> mCharacterIds = new List<int>();
        private int mCharacterIdx = 0;

        private ActorSpawner mSpawner = null;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.ExplorationOutfiterFlowControl;
        }

        protected override void Setup()
        {
            mSpawner = LevelManagementService.Get().GetLoadedLevel().Player.GetComponentInChildren<ActorSpawner>();
            mSpawner.GetComponentInChildren<ActorOutfitter>().UpdateHeldApparelState(HumanoidActorConstants.HeldApparelState.Held);

            Camera.main.GetComponent<Cameras.CameraController>().SetTarget(mSpawner.transform, Cameras.CameraType.Outfitter);

            mCharacterIds = MAGE.GameSystems.WorldService.Get().GetCharactersInParty();

            mCharacterIdx = mCharacterIds.IndexOf(GameSystems.WorldService.Get().GetPartyAvatarId());

            RefreshOutfittedCharacter();

            SetOutfiter(OutfitState.Equip);

            UIManager.Instance.PostContainer(UIContainerId.OutfiterSelectView, this);
        }

        protected override void Cleanup()
        {
            if (mOutfiter != null)
            {
                mOutfiter.Cleanup();
            }

            Camera.main.GetComponent<Cameras.CameraController>().SetTarget(mSpawner.transform, Cameras.CameraType.ThirdPerson);

            mSpawner.GetComponentInChildren<ActorOutfitter>().UpdateHeldApparelState(HumanoidActorConstants.HeldApparelState.Holstered);
            mSpawner.GetComponent<CharacterPickerControl>().CharacterId = WorldService.Get().GetPartyAvatarId();
            mSpawner.Refresh();

            UIManager.Instance.RemoveOverlay(UIContainerId.OutfiterSelectView);
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch (containerId)
            {
                case (int)UIContainerId.OutfiterSelectView:
                {
                    if (interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        switch (interactionInfo.ComponentId)
                        {
                            case (int)OutfiterSelectView.ComponentId.CharacterSelectLeftBtn:
                            {
                                CycleCharacter(-1);
                            }
                            break;

                            case (int)OutfiterSelectView.ComponentId.CharacterSelectRightBtn:
                            {
                                CycleCharacter(1);
                            }
                            break;
                            case (int)OutfiterSelectView.ComponentId.ExitBtn:
                            {
                                SendFlowMessage("advance");
                            }
                            break;
                            case (int)OutfiterSelectView.ComponentId.EquipBtn:
                            {
                                SetOutfiter(OutfitState.Equip);    
                            }
                            break;

                            case (int)OutfiterSelectView.ComponentId.SpecBtn:
                            {
                                SetOutfiter(OutfitState.Specialize);
                            }
                            break;
                        }
                    }
                }
                break;
            }
        }

        public string Name()
        {
            return "PartyOutfiterViewControl";
        }

        public IDataProvider Publish(int containerId)
        {
            OutfiterSelectView.DataProvider dataProvider = new OutfiterSelectView.DataProvider();

            Character character = CharacterService.Get().GetCharacter(mCharacterIds[mCharacterIdx]);

            dataProvider.character = character.Name;

            return dataProvider;
        }

        private void SetOutfiter(OutfitState state)
        {
            if (mState != state)
            {
                mState = state;

                if (mOutfiter != null)
                {
                    mOutfiter.Cleanup();
                }

                switch (mState)
                {
                    case OutfitState.Equip: mOutfiter = new EquipmentOutfiterViewControl(); break;
                    case OutfitState.Specialize: mOutfiter = new SpecializationOutfiterViewControl(); break;
                }

                mOutfiter.SetCharacter(GetCurrentCharacterId());
                mOutfiter.BeginOutfitting();
            }
        }

        private void CycleCharacter(int direction)
        {
            int newIdx = mCharacterIdx + direction;
            if (newIdx < 0) newIdx = mCharacterIds.Count - 1;
            if (newIdx == mCharacterIds.Count) newIdx = 0;

            if (newIdx != mCharacterIdx)
            {
                mCharacterIdx = newIdx;

                RefreshOutfittedCharacter();

                mOutfiter.SetCharacter(GetCurrentCharacterId());
            }

            UIManager.Instance.Publish(UIContainerId.OutfiterSelectView);
        }

        private int GetCurrentCharacterId()
        {
            return mCharacterIds[mCharacterIdx];
        }

        private void RefreshOutfittedCharacter()
        {
            mSpawner.GetComponent<CharacterPickerControl>().CharacterId = GetCurrentCharacterId();
            mSpawner.Refresh();
        }

        public override void HandleMessage(MessageInfoBase eventInfoBase)
        {
            switch (eventInfoBase.MessageId)
            {
                case CharacterMessage.Id:
                {
                    CharacterMessage message = eventInfoBase as CharacterMessage;
                    if (message.Type == CharacterMessage.MessageType.CharacterUpdated && message.Arg<int>() == GetCurrentCharacterId())
                    {
                        mOutfiter.Refresh();
                    }
                }
                break;
            }
        }
    }
}
