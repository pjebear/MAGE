﻿using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.World.Internal
{
    class PartySystem
    {
        private string TAG = "PartySystem";

        private string mSaveFileName = "";

        private PartyInfo mPartyInfo = new PartyInfo();
        private PartyLocation mPartyLocation;

        public void AddCharacterToParty(int characterId)
        {
            Logger.Assert(!mPartyInfo.CharacterIds.Contains(characterId), LogTag.Party, TAG, string.Format("::AddCharacterToParty() Character [{0}] is already in party!", characterId));
            if (!mPartyInfo.CharacterIds.Contains(characterId))
            {
                mPartyInfo.CharacterIds.Add(characterId);
            }
        }

        public void RemoveCharacterFromParty(int characterId)
        {
            Logger.Assert(mPartyInfo.CharacterIds.Contains(characterId), LogTag.Party, TAG, string.Format("::RemoveCharacterFromParty() Character [{0}] isn't in party!", characterId));
            mPartyInfo.CharacterIds.Remove(characterId);
        }

        public int GetPartyAvatarId()
        {
            return mPartyInfo.AvatarId;
        }

        public List<int> GetCharactersInParty()
        {
            return new List<int>(mPartyInfo.CharacterIds);
        }

        public void AddCurrency(int amount)
        {
            mPartyInfo.Currency += amount;
        }

        public int GetCurrency()
        {
            return mPartyInfo.Currency;
        }

        public void RemoveCurrency(int amount)
        {
            Logger.Assert(mPartyInfo.Currency >= amount, LogTag.Party, TAG, string.Format("::RemoveCurrency() Insufficient funds. Cost {0} Available {1}", amount, mPartyInfo.Currency));
            mPartyInfo.Currency -= amount;
        }

        public Inventory GetInventory()
        {
            return mPartyInfo.Inventory;
        }

        public void AddToInventory(List<int> itemIds)
        {
            foreach (int itemId in itemIds)
            {
                mPartyInfo.Inventory.Add(itemId);
            }
        }

        public void AddToInventory(int itemId)
        {
            mPartyInfo.Inventory.Add(itemId);
        }

        public void RemoveFromInventory(int itemId)
        {
            mPartyInfo.Inventory.Remove(itemId);
        }

        public void UnEquipCharacter(int characterId, Equipment.Slot inSlot)
        {
            List<int> unequippedItems = CharacterService.Get().UnEquipCharacter(characterId, inSlot);

            foreach (int itemId in unequippedItems)
            {
                mPartyInfo.Inventory.Add(itemId);
            }
        }

        public void EquipCharacter(int characterId, EquippableId equippableId, Equipment.Slot inSlot, bool pullFromInventory = true)
        {
            if (pullFromInventory)
            {
                Logger.Assert(mPartyInfo.Inventory.Contains((int)equippableId), LogTag.GameSystems, TAG, string.Format("EquipCharacter() - Failed to find item in inventory [{0}].", equippableId.ToString()));
                if (mPartyInfo.Inventory.Contains((int)equippableId))
                {
                    mPartyInfo.Inventory.Remove((int)equippableId);
                }
            }

            List<int> unequippedItems = CharacterService.Get().EquipCharacter(characterId, equippableId, inSlot);

            foreach (int itemId in unequippedItems)
            {
                mPartyInfo.Inventory.Add(itemId);
            }
        }

        public void PrepareForEncounter(EncounterCreateParams createParams)
        {
            foreach (int characterId in mPartyInfo.CharacterIds)
            {
                DBService.Get().AddToTeam(characterId, TeamSide.AllyHuman);
            }
        }

        public void UpdateOnEncounterEnd(EncounterResultInfo resultInfo)
        {
            DBService.Get().ClearTeam(TeamSide.AllyHuman);

            foreach (int characterId in resultInfo.PlayersInEncounter[TeamSide.AllyHuman])
            {
                CharacterService.Get().AssignExperience(characterId, CharacterConstants.LEVEL_UP_THRESHOLD);
            }

            // Add new Items
            foreach (int itemReward in resultInfo.ItemRewards)
            {
                mPartyInfo.Inventory.Add(itemReward);
            }

            // Currency Reward
            mPartyInfo.Currency += resultInfo.CurrencyReward;
        }

        //! Characters

        // Character End

        public void Save()
        {
            if (mSaveFileName == "")
            {
                mSaveFileName = SaveLoadUtil.GetNextAvailableSaveFileName();
            }

            SaveLoad.SaveFile saveFile = new SaveLoad.SaveFile();
            saveFile.Name = mSaveFileName;

            SaveLoadUtil.AddParty(saveFile, mPartyInfo);

            SaveLoadUtil.Save(saveFile);
        }

        public void Load(string saveFileName)
        {
            SaveLoad.SaveFile saveFile = SaveLoadUtil.Load(saveFileName);

            mSaveFileName = saveFile.Name;
            mPartyInfo = SaveLoadUtil.ExtractParty(saveFile);
        }

        #region DEBUG
        public void CreateDefaultParty()
        {   
            mPartyInfo.CharacterIds.AddRange(CharacterService.Get().GetCharactersOfType(CharacterType.Create)); 
            mPartyInfo.CharacterIds.Add((int)StoryCharacterId.Rheinhardt);
            mPartyInfo.CharacterIds.Add((int)StoryCharacterId.Asmund);
            mPartyInfo.CharacterIds.Add((int)StoryCharacterId.Balgrid);

            List<int> defaultInventory = new List<int>()
            {
                 (int)EquippableId.Axe_0
                 , (int)EquippableId.Mace_0
                 , (int)EquippableId.Sword_0
                 , (int)EquippableId.LeatherArmor_0
            };

            foreach (int itemId in defaultInventory)
            {
                mPartyInfo.Inventory.Add(itemId);
            }

            mPartyInfo.AvatarId = (int)StoryCharacterId.Rheinhardt;
            mPartyInfo.Currency = 1000;
        }
        #endregion
    }
}

