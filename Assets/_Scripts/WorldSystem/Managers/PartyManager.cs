using Common.UnitTypes;
using Screens.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Character;
using WorldSystem.Items;
using WorldSystem.InventorySystem;
using Common.EquipmentEnums;
using Common.EquipmentTypes;

namespace WorldSystem.Managers
{
    class PartyManager
    {
        private UnitRoster mUnitRoster;
        private Inventory mInventory;
        private int mWealth;

        public PartyManager()
        {
            mUnitRoster = new UnitRoster();
            mInventory = new Inventory();
            mWealth = 0;
        }

        #region _Roster_
        public UnitRoster GetRoster()
        {
            return mUnitRoster;
        }

        public void ResetPostEncounter()
        {
            foreach (CharacterBase character in mUnitRoster.Roster.Values)
            {
                character.PostEncounterReset();
            }
        }

        public void AddCharacter(CharacterBase toAdd)
        {
            mUnitRoster.AddCharacter(toAdd);
        }

        public void RemoveCharacter(CharacterBase toRemove)
        {
            mUnitRoster.RemoveCharacter(toRemove.CharacterID);
        }

        public void EquipCharacterInSlot(int characterId, KeyValuePair<int, int> equipmentIndex, WornEquipmentSlot slot)
        {
            CharacterBase equipping = mUnitRoster.Roster[characterId];
            EquipmentBase equipment = mInventory.CheckoutEquipment(new KeyValuePair<EquipmentCategory, int>((EquipmentCategory)equipmentIndex.Key, equipmentIndex.Value));
            List<EquipmentBase> unequippedItems = equipping.EquipInSlot(equipment, slot);
            foreach(EquipmentBase item in unequippedItems)
            {
                mInventory.AddItem(item);
            }
        }

        public void UnequipCharacterSlot(int characterId, WornEquipmentSlot slot)
        {
            CharacterBase equipping = mUnitRoster.Roster[characterId];
            EquipmentBase unequippedItem = equipping.UnequipSlot(slot);
            if (unequippedItem != null)
            {
                mInventory.AddItem(unequippedItem);
            }
        }

        public List<InventoryEquipmentPayload> GetEquipmentInventoryPayloads(int characterId)
        {
            return mInventory.GetEquipmentPayloads();
        }

        public List<InventoryEquipmentPayload> GetValidEquipmentForSlot(CharacterBase forCharacter, WornEquipmentSlot equipmentSlot)
        {
            return mInventory.GetEquipmentPayloadsForSlot(forCharacter.Proficiencies, equipmentSlot);
        }

        public void AddEquipmentToInventory(EquipmentBase equipment)
        {
            mInventory.AddItem(equipment);
        }
        #endregion
    }
}
