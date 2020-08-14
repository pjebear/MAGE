﻿using MAGE.GameServices;
using MAGE.GameServices.Character;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MAGE.GameModes.FlowControl
{
    class EquipmentOutfiterViewControl
    : UIContainerControl
    , ICharacterOutfiter
    {
        private MAGE.GameServices.Character.CharacterInfo mOutfitingCharacter = null;
        private UnityAction mOnUpdatedCB;

        private Equipment.Slot mSelectedSlot = Equipment.Slot.INVALID;
        private List<int> mEquipmentSelections = new List<int>();

        public string Name()
        {
            return "EquipmentOutfiterViewControl";
        }

        //! ICharacterOutfiter
        public void BeginOutfitting(MAGE.GameServices.Character.CharacterInfo character, UnityAction characterUpdated)
        {
            mOutfitingCharacter = character;
            mOnUpdatedCB = characterUpdated;

            UIManager.Instance.PostContainer(UIContainerId.EquipmentOutfiterView, this);
        }

        public void Cleanup()
        {
            mOutfitingCharacter = null;
            mOnUpdatedCB = null;

            UIManager.Instance.RemoveOverlay(UIContainerId.EquipmentOutfiterView);
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch (containerId)
            {
                case (int)UIContainerId.EquipmentOutfiterView:
                {
                    if (interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        switch (interactionInfo.ComponentId)
                        {
                            case (int)EquipmentOutfiterView.ComponentId.LeftHeldSlotBtn:
                            {
                                mSelectedSlot = Equipment.Slot.LeftHand;
                                UIManager.Instance.Publish(UIContainerId.EquipmentOutfiterView);
                            }
                            break;

                            case (int)EquipmentOutfiterView.ComponentId.RightHeldSlotBtn:
                            {
                                mSelectedSlot = Equipment.Slot.RightHand;
                                UIManager.Instance.Publish(UIContainerId.EquipmentOutfiterView);
                            }
                            break;

                            case (int)EquipmentOutfiterView.ComponentId.WornSlotBtn:
                            {
                                mSelectedSlot = Equipment.Slot.Armor;
                                UIManager.Instance.Publish(UIContainerId.EquipmentOutfiterView);
                            }
                            break;

                            case (int)EquipmentOutfiterView.ComponentId.UnEquipBtn:
                            {
                                UnEquip();
                                mSelectedSlot = Equipment.Slot.INVALID;
                                UIManager.Instance.Publish(UIContainerId.EquipmentOutfiterView);
                            }
                            break;

                            case (int)EquipmentOutfiterView.ComponentId.EquipableSelectionBtns:
                            {
                                ListInteractionInfo buttonListInteractionInfo = interactionInfo as ListInteractionInfo;
                                Equip(mEquipmentSelections[buttonListInteractionInfo.ListIdx]);
                                mSelectedSlot = Equipment.Slot.INVALID;
                                UIManager.Instance.Publish(UIContainerId.EquipmentOutfiterView);
                            }
                            break;
                        }
                    }
                }
                break;
            }
        }

        private void Equip(int equipmentId)
        {
            MAGE.GameServices.WorldService.Get().EquipCharacter(mOutfitingCharacter.Id, (EquippableId)equipmentId, mSelectedSlot);

            mOnUpdatedCB();
        }

        private void UnEquip()
        {
            MAGE.GameServices.WorldService.Get().UnEquipCharacter(mOutfitingCharacter.Id, mSelectedSlot);

            mOnUpdatedCB();
        }

        public IDataProvider Publish(int containerId)
        {
            EquipmentOutfiterView.DataProvider dataProvider = new EquipmentOutfiterView.DataProvider();

            Inventory inventory = MAGE.GameServices.WorldService.Get().GetInventory();

            MAGE.GameServices.Character.Specialization specialization = mOutfitingCharacter.CurrentSpecialization;

            { // Left Held
                Equippable equippable = mOutfitingCharacter.Equipment[Equipment.Slot.LeftHand];
                if (equippable != Equipment.NO_EQUIPMENT)
                {
                    dataProvider.LeftHeld = new EquipmentOutfiterView.EquipableDP() { Name = equippable.EquipmentId.ToString() };
                }
                dataProvider.LeftSelected = mSelectedSlot == Equipment.Slot.LeftHand;
            }

            { // Right Held
                Equippable equippable = mOutfitingCharacter.Equipment[Equipment.Slot.RightHand];
                if (equippable != Equipment.NO_EQUIPMENT)
                {
                    dataProvider.RightHeld = new EquipmentOutfiterView.EquipableDP() { Name = equippable.EquipmentId.ToString() };
                }
                dataProvider.RightSelected = mSelectedSlot == Equipment.Slot.RightHand;
            }

            { // Worn
                Equippable equippable = mOutfitingCharacter.Equipment[Equipment.Slot.Armor];
                if (equippable != Equipment.NO_EQUIPMENT)
                {
                    dataProvider.Worn = new EquipmentOutfiterView.EquipableDP() { Name = equippable.EquipmentId.ToString() };
                }
                dataProvider.WornSelected = mSelectedSlot == Equipment.Slot.Armor;
            }

            {// Unequip 
                dataProvider.CanUnequip = mSelectedSlot != Equipment.Slot.INVALID &&
                    mOutfitingCharacter.Equipment[mSelectedSlot] != Equipment.NO_EQUIPMENT;
            }

            { // Equip Options
                mEquipmentSelections.Clear();
                if (mSelectedSlot != Equipment.Slot.INVALID)
                {
                    List<EquipmentOutfiterView.EquipableDP> areEquipable = new List<EquipmentOutfiterView.EquipableDP>();
                    List<EquipmentOutfiterView.EquipableDP> notEquipable = new List<EquipmentOutfiterView.EquipableDP>();

                    foreach (var keyValuePair in inventory.Items
                        .Where(x => ItemUtil.TypeFromId(x.Key) == ItemType.Equippable))
                    {
                        Equippable equippable = ItemFactory.LoadEquipable((EquippableId)keyValuePair.Key);
                        if (EquipmentUtil.FitsInSlot(equippable.EquipmentTag.Category, mSelectedSlot))
                        {
                            EquipmentOutfiterView.EquipableDP equipableDP = new EquipmentOutfiterView.EquipableDP() { Name = equippable.EquipmentId.ToString(), Count = keyValuePair.Value };
                            if (EquipmentUtil.HasProficiencyFor(specialization, equippable))
                            {
                                equipableDP.CanEquip = true;
                                areEquipable.Add(equipableDP);
                                mEquipmentSelections.Add(keyValuePair.Key);
                            }
                            else
                            {
                                equipableDP.CanEquip = false;
                                notEquipable.Add(equipableDP);
                            }
                        }
                    }

                    dataProvider.EquipableOptions.AddRange(areEquipable);
                    dataProvider.EquipableOptions.AddRange(notEquipable);

                }
            }

            return dataProvider;
        }
    }
}



