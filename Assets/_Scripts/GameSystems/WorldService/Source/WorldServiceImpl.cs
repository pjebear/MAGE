using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.World.Internal
{
    class WorldServiceImpl : IWorldService
    {
        private LocationSystem mLocationSystem;
        private PartySystem mPartySystem;
        private EncounterSystem mEncounterSystem;

        public WorldServiceImpl()
        {
            mLocationSystem = new LocationSystem();
            mPartySystem = new PartySystem();
            mEncounterSystem = new EncounterSystem();
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
        public EncounterContext GetEncounterContext()
        {
            return mEncounterSystem.GetEncounterContext();
        }

        public void PrepareEncounter(EncounterCreateParams encounterParams)
        {
            mPartySystem.PrepareForEncounter(encounterParams);
            mEncounterSystem.PrepareEncounter(encounterParams);
        }

        public void UpdateOnEncounterEnd(EncounterResultInfo resultInfo)
        {
            mPartySystem.UpdateOnEncounterEnd(resultInfo);
            mEncounterSystem.CleanupEncounter();
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
        public LevelId GetCurrentLevel()
        {
            return mLocationSystem.PartyLocation;
        }
        // Location - End

        // Party
        public void AddCharacterToParty(int characterId)
        {
            mPartySystem.AddCharacterToParty(characterId);
        }

        public void AddToInventory(int itemId)
        {
            mPartySystem.AddToInventory(itemId);

            StoryService.Get().NotifyStoryEvent(new Story.StoryEventBase((ItemId)itemId));
        }

        public void AssignTalentPoint(int characterId, Character.TalentId talentId)
        {
            CharacterService.Get().AssignTalentPoint(characterId, talentId);
        }

        public void ChangeSpecialization(int characterId, Character.SpecializationType specializationType)
        {
            List<int> unequippedItems = CharacterService.Get().ChangeSpecialization(characterId, specializationType);

            mPartySystem.AddToInventory(unequippedItems);
        }

        public void EquipCharacter(int characterId, EquippableId equippableId, Character.Equipment.Slot inSlot)
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

        public void RemoveFromInventory(int itemId)
        {
            mPartySystem.RemoveFromInventory(itemId);
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

        public void UnEquipCharacter(int characterId, Character.Equipment.Slot inSlot)
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
