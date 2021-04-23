using MAGE.GameModes.Combat;
using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounters;
using MAGE.GameModes.SceneElements.Navigation;
using MAGE.GameSystems;
using MAGE.GameSystems.Loot;
using MAGE.GameSystems.Mobs;
using MAGE.Input;
using MAGE.Messaging;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Exploration
{
    class RoamFlowControl
        : FlowControlBase
    {
        private string TAG = "RoamFlowControl";

        private Actor mPlayer = null;
        private Interactable mHoveredInteractable = null;
        private Optional<Vector3> mWorldHoverPosition;

        private InteractionFlowControl mInteractionFlowControl;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.ExplorationRoamFlowControl;
        }

        protected override void Setup()
        {
            mPlayer = GameModel.Exploration.PartyAvatar;
            mPlayer.GetComponent<ThirdPersonActorController>().enabled = true;
            UIManager.Instance.SetCursor(CursorControl.CursorType.Default);
        }

        protected override void Cleanup()
        {
            mPlayer.GetComponent<ThirdPersonActorController>().enabled = false;
            UIManager.Instance.SetCursor(CursorControl.CursorType.Default);
        }

        public override void HandleMessage(MessageInfoBase eventInfoBase)
        {
            switch (eventInfoBase.MessageId)
            {
                case (int)ExplorationMessage.Id:
                {
                    ExplorationMessage explorationMessage = eventInfoBase as ExplorationMessage;
                    switch (explorationMessage.Type)
                    {
                        case ExplorationMessage.EventType.EncounterTriggered:
                        {
                            TriggerEncounter(explorationMessage.Arg<GameObject>());
                        }
                        break;
                    }
                }
                break;
            }
        }

        //private void Update()
        //{
        //    // Update Cursor
        //    UI.Views.CursorControl.CursorType playerCursorType = UI.Views.CursorControl.CursorType.Default;

        //    Interactable hoveredInteractable = null;
        //    List<RaycastHit> hits = RaycastUtil.GetObjectsHitByCursor();

        //    foreach (RaycastHit raycastHit in hits)
        //    {
        //        hoveredInteractable = raycastHit.collider.gameObject.GetComponent<Interactable>();
        //        if (hoveredInteractable != null)
        //        {
        //            float distanceToInteractible = Vector3.Distance(mPlayer.transform.position, hoveredInteractable.transform.position);
        //            bool inRange = hoveredInteractable.GetInteractionRange() >= distanceToInteractible;

        //            switch (hoveredInteractable.InteractionType)
        //            {
        //                case InteractionType.Combat: playerCursorType = inRange ? UI.Views.CursorControl.CursorType.Combat_Near : UI.Views.CursorControl.CursorType.Combat_Far; break;
        //            }

        //            break;
        //        }
        //    }

        //    mHoveredInteractable = hoveredInteractable;

        //    UI.UIManager.Instance.SetCursor(playerCursorType);
        //}

        // IInputHandler
        public override void OnKeyPressed(InputSource source, int key, InputState state)
        {
            switch (source)
            {
                case InputSource.Mouse:
                {
                    if (key == (int)MouseKey.Right && state == InputState.Down)
                    {
                        HandleMouseRightClick();
                    }
                }
                break;
            }
        }

        private void HandleMouseRightClick()
        {
            if (mHoveredInteractable != null)
            {
                if (IsInteractableInRange())
                {
                    switch (mHoveredInteractable.InteractionType)
                    {
                        case InteractionType.Combat:
                        {
                            TriggerEncounter(mHoveredInteractable.gameObject);
                        }
                        break;
                        case InteractionType.NPC:
                        {
                            GameModel.Exploration.InteractionTarget = mHoveredInteractable;
                            SendFlowMessage("interact");
                        }
                        break;
                    }
                }
            }
        }

        public override void OnMouseHoverChange(GameObject mouseHover)
        {
            // handled in update
        }
        
        public override void OnMouseScrolled(float scrollDelta)
        {
            // empty
        }

        void Update()
        {
            mHoveredInteractable = null;

            MouseInfo mouseInfo = InputManager.Instance.GetMouseInfo();
            if (mouseInfo.IsOverWindow)
            {
                List<RaycastHit> hits = RaycastUtil.GetObjectsHitByCursor();
                foreach (RaycastHit raycastHit in hits)
                {
                    mHoveredInteractable = raycastHit.collider.gameObject.GetComponent<Interactable>();
                    if (mHoveredInteractable != null)
                    {
                        break;
                    }
                }

                if (mHoveredInteractable == null 
                    && hits.Count > 0
                    && hits[0].collider.gameObject.layer == (int)Layer.Terrain)
                {
                    mWorldHoverPosition = hits[0].point;
                }
                else
                {
                    mWorldHoverPosition = Optional<Vector3>.Empty;
                }
            }
            
            UpdateHoverTargetCursor();
        }

        private void UpdateHoverTargetCursor()
        {
            UI.Views.CursorControl.CursorType playerCursorType = UI.Views.CursorControl.CursorType.Default;

            if (mHoveredInteractable != null)
            {
                bool inRange = IsInteractableInRange();

                switch (mHoveredInteractable.InteractionType)
                {
                    case InteractionType.Combat:    playerCursorType = inRange ? UI.Views.CursorControl.CursorType.Combat_Near : UI.Views.CursorControl.CursorType.Combat_Far; break;
                    case InteractionType.NPC:       playerCursorType = inRange ? UI.Views.CursorControl.CursorType.Interact_Near : UI.Views.CursorControl.CursorType.Interact_Far; break;
                }
            }

            UIManager.Instance.SetCursor(playerCursorType);
        }

        private bool IsInteractableInRange()
        {
            bool inRange = false;
            if (mHoveredInteractable != null)
            {
                float distanceToInteractible = Vector3.Distance(mPlayer.transform.position, mHoveredInteractable.transform.position);
                inRange = mHoveredInteractable.GetInteractionRange() >= distanceToInteractible;
            }
            return inRange;
        }

        private void TriggerEncounter(GameObject enemy)
        {
            mPlayer.GetComponent<ThirdPersonActorController>().enabled = false;

            PrepareEncounter(enemy);
            SendFlowMessage("encounter");
        }

        private void PrepareEncounter(GameObject enemy)
        {
            GameSystems.World.PartyLocation partyLocation = GameSystems.WorldService.Get().GetPartyLocation();
            partyLocation.SetPosition(GameModel.Exploration.PartyAvatar.transform.position);
            GameSystems.WorldService.Get().UpdatePartyLocation(partyLocation);

            Level level = LevelManagementService.Get().GetLoadedLevel();
            EncounterContainer randomEncounter = level.CreateEncounter();
            randomEncounter.EncounterScenarioId = EncounterScenarioId.Random;

            Collider[] overlapped = Physics.OverlapSphere(enemy.transform.position, 15f);
            List<MobControl> mobsInRange = overlapped.Select(x => x.GetComponent<MobControl>()).Where(x => x != null && x.gameObject.activeSelf).ToList();

            ClaimLootParams claimLootParams = new ClaimLootParams();
            foreach (MobControl mob in mobsInRange)
            {
                mob.gameObject.SetActive(false);
                claimLootParams.Mobs.Add(mob.GetComponent<MobCharacterControl>().MobId);

                CombatCharacter player = level.CreateCombatCharacter(mob.transform.position, mob.transform.rotation, randomEncounter.Enemies);
                player.GetComponent<CharacterPickerControl>().CharacterPicker.SetRootCharacterId(mob.GetComponent<CharacterPickerControl>().CharacterPicker.GetCharacterId());
            }

            ClaimLootInfo lootInfo = WorldService.Get().GetLoot(claimLootParams);
            WorldService.Get().ClaimLoot(lootInfo);

            Vector3 partyToEnemy = enemy.transform.position - mPlayer.transform.position;
            partyToEnemy.y = 0;
            Vector3 right = Vector3.Cross(partyToEnemy, Vector3.up);

            int characterCount = 0;
            foreach (int partyCharacterId in WorldService.Get().GetCharactersInParty())
            //int partyCharacterId = WorldService.Get().GetPartyAvatarId();
            {
                CombatCharacter player = level.CreateCombatCharacter(mPlayer.transform.position + characterCount++ * right, mPlayer.transform.rotation, randomEncounter.Allys);
                player.GetComponent<CharacterPickerControl>().CharacterPicker.SetRootCharacterId(partyCharacterId);
            }
        }
    }
}


