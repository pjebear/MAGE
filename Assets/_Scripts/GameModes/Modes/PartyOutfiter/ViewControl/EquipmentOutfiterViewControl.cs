using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using UnityEngine;
using UnityEngine.Events;

class EquipmentOutfiterViewControl 
    : UIContainerControl
    , ICharacterOutfiter
{
    private DB.DBCharacter mOutfitingCharacter = null;
    private UnityAction mOnUpdatedCB;

    private Equipment.Slot mSelectedSlot = Equipment.Slot.INVALID;
    private List<int> mEquipmentSelections = new List<int>();

    public string Name()
    {
        return "EquipmentOutfiterViewControl";
    }

    //! ICharacterOutfiter
    public void BeginOutfitting(DBCharacter character, UnityAction characterUpdated)
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
        if (mOutfitingCharacter.Equipment[(int)mSelectedSlot] != (int)EquippableId.INVALID)
        {
            GameSystemModule.Instance.AddToInventory(mOutfitingCharacter.Equipment[(int)mSelectedSlot]);
        }

        mOutfitingCharacter.Equipment[(int)mSelectedSlot] = equipmentId;

        DBHelper.WriteCharacter(mOutfitingCharacter);

        mOnUpdatedCB();
    }

    private void UnEquip()
    {
        if (mOutfitingCharacter.Equipment[(int)mSelectedSlot] != (int)EquippableId.INVALID)
        {
            GameSystemModule.Instance.AddToInventory(mOutfitingCharacter.Equipment[(int)mSelectedSlot]);
            mOutfitingCharacter.Equipment[(int)mSelectedSlot] = (int)EquippableId.INVALID;
        }

        DBHelper.WriteCharacter(mOutfitingCharacter);

        mOnUpdatedCB();
    }

    public IDataProvider Publish(int containerId)
    {
        EquipmentOutfiterView.DataProvider dataProvider = new EquipmentOutfiterView.DataProvider();

        Inventory inventory = GameSystemModule.Instance.GetInventory();
        Specialization specialization = SpecializationFactory.CheckoutSpecialization(
            (SpecializationType)mOutfitingCharacter.CharacterInfo.CurrentSpecialization,
            mOutfitingCharacter.Specializations[mOutfitingCharacter.CharacterInfo.CurrentSpecialization]);
        { // Left Held
            if (mOutfitingCharacter.Equipment[(int)Equipment.Slot.LeftHand] != (int)EquippableId.INVALID)
            {
                Equippable equippable = ItemFactory.LoadEquipable((EquippableId)mOutfitingCharacter.Equipment[(int)Equipment.Slot.LeftHand]);
                dataProvider.LeftHeld = new EquipmentOutfiterView.EquipableDP() { Name = equippable.EquipmentId.ToString() };
            }
            dataProvider.LeftSelected = mSelectedSlot == Equipment.Slot.LeftHand;
        }

        { // Right Held
            if (mOutfitingCharacter.Equipment[(int)Equipment.Slot.RightHand] != (int)EquippableId.INVALID)
            {
                Equippable equippable = ItemFactory.LoadEquipable((EquippableId)mOutfitingCharacter.Equipment[(int)Equipment.Slot.RightHand]);
                dataProvider.RightHeld = new EquipmentOutfiterView.EquipableDP() { Name = equippable.EquipmentId.ToString() };
            }
            dataProvider.RightSelected = mSelectedSlot == Equipment.Slot.RightHand;
        }

        { // Worn
            if (mOutfitingCharacter.Equipment[(int)Equipment.Slot.Armor] != (int)EquippableId.INVALID)
            {
                Equippable equippable = ItemFactory.LoadEquipable((EquippableId)mOutfitingCharacter.Equipment[(int)Equipment.Slot.Armor]);
                dataProvider.Worn = new EquipmentOutfiterView.EquipableDP() { Name = equippable.EquipmentId.ToString() };
            }
            dataProvider.WornSelected = mSelectedSlot == Equipment.Slot.Armor;
        }

        {// Unequip 

            dataProvider.CanUnequip = mSelectedSlot != Equipment.Slot.INVALID &&
                mOutfitingCharacter.Equipment[(int)mSelectedSlot] != (int)EquippableId.INVALID;
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

