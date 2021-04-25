using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Loot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.World.Internal
{
    class WorldServiceImpl : IWorldService
    {
        private LocationSystem mLocationSystem;
        private PartySystem mPartySystem;
        private LootTable mLootTable;

        public WorldServiceImpl()
        {
            mLocationSystem = new LocationSystem();
            mPartySystem = new PartySystem();
            mLootTable = new LootTable();
        }

        // IService
        public void Init()
        {
            // empty
        }

        public void Takedown()
        {
            // empty
        }

        // Conversation
        public Conversation GetConversation(ConversationId conversationId)
        {
            DB.DBConversation dbConversation = DBService.Get().LoadConversation((int)conversationId);

            return ConversationUtil.FromDB(dbConversation);
        }

        public void NotifyConversationComplete(ConversationId conversationId)
        {
            StoryService.Get().NotifyStoryEvent(new Story.StoryEventBase(conversationId));
        }
        // Conversation - End

        // Encounter
        public EncounterEndInfo UpdateOnEncounterEnd(EncounterEndParams resultParams)
        {
            EncounterEndInfo encounterEndInfo = new EncounterEndInfo();
            encounterEndInfo.Won = resultParams.DidUserWin;

            encounterEndInfo.CharacterGrowth = new Dictionary<int, CharacterGrowthInfo>();
            foreach (int characterId in resultParams.PlayersInEncounter)
            {
                encounterEndInfo.CharacterGrowth.Add(characterId, CharacterService.Get().AssignExperience(characterId, CharacterConstants.LEVEL_UP_THRESHOLD));
            }

            encounterEndInfo.Rewards = mLootTable.CheckoutLoot(resultParams.LootParams);

            StoryService.Get().NotifyStoryEvent(new Story.StoryEventBase(resultParams.EncounterScenarioId));

            return encounterEndInfo;
        }
        // Encounter - End

        // Flow
        public void PrepareNewGame()
        {
            mPartySystem.CreateDefaultParty();

            StoryService.Get().NotifyStoryEvent(new Story.StoryEventBase(Story.StoryEventType.NewGame));
        }
        // Flow - End

        // Location
        public PartyLocation GetPartyLocation()
        {
            return mLocationSystem.PartyLocation;
        }

        public void UpdatePartyLocation(World.PartyLocation updatedLocation)
        {
            mLocationSystem.PartyLocation = updatedLocation;
        }
        // Location - End

        // Loot 
        public Loot.LootTable DEBUG_GetLootTable()
        {
            return mLootTable;
        }
        // Loot - End

        // Party
        public void AddCharacterToParty(int characterId)
        {
            mPartySystem.AddCharacterToParty(characterId);
        }

        public void AddToInventory(int itemId, int num = 1)
        {
            mPartySystem.AddToInventory(itemId, num);

            StoryService.Get().NotifyStoryEvent(new Story.StoryEventBase((ItemId)itemId, 1));
        }

        public void AssignTalentPoint(int characterId, Characters.TalentId talentId)
        {
            CharacterService.Get().AssignTalentPoint(characterId, talentId);
        }

        public void ChangeSpecialization(int characterId, Characters.SpecializationType specializationType)
        {
            List<int> unequippedItems = CharacterService.Get().ChangeSpecialization(characterId, specializationType);

            mPartySystem.AddToInventory(unequippedItems);
        }

        public void EquipCharacter(int characterId, EquippableId equippableId, Items.Equipment.Slot inSlot)
        {
            mPartySystem.EquipCharacter(characterId, equippableId, inSlot);
        }

        public List<int> GetCharactersInParty()
        {
            return mPartySystem.GetCharactersInParty();
        }

        public int GetCurrency()
        {
            return mPartySystem.GetCurrency();
        }

        public Inventory GetInventory()
        {
            return mPartySystem.GetInventory();
        }

        public int GetPartyAvatarId()
        {
            return mPartySystem.GetPartyAvatarId();
        }

        public void PurchaseItem(int cost, int itemId)
        {
            mPartySystem.RemoveCurrency(cost);
            AddToInventory(itemId);
        }

        public void RemoveCharacterFromParty(int characterId)
        {
            mPartySystem.RemoveCharacterFromParty(characterId);
        }

        public void RemoveFromInventory(int itemId, int num)
        {
            mPartySystem.RemoveFromInventory(itemId, num);
        }

        public void ResetTalentPoints(int characterId)
        {
            List<int> unequippedItems = CharacterService.Get().ResetTalentPoints(characterId);

            mPartySystem.AddToInventory(unequippedItems);
        }

        public void SellItem(int cost, int itemId)
        {
            mPartySystem.AddCurrency(cost);
            mPartySystem.RemoveFromInventory(itemId);
        }

        public void UnEquipCharacter(int characterId, Items.Equipment.Slot inSlot)
        {
            mPartySystem.UnEquipCharacter(characterId, inSlot);
        }

        // Party - End

        // SaveLoad
        public List<string> GetSaveFiles()
        {
            return SaveLoadUtil.GetSaveFiles();
        }

        public void Save()
        {
            mPartySystem.Save();
        }

        public void Load(string saveFileName)
        {
            mPartySystem.Load(saveFileName);
        }
        // SaveLoad - End
    }
}
