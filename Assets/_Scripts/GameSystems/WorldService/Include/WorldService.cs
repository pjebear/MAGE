using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    interface IWorldService : Services.IService
    {
        // Conversation
        Conversation GetConversation(ConversationId conversationId);
        void NotifyConversationComplete(ConversationId conversationId);
        // Conversation - End

        // Encounter
        EncounterEndInfo UpdateOnEncounterEnd(EncounterEndParams resultInfo);
        // Encounter - End

        // Flow
        void PrepareNewGame();
        // Flow - End

        // Location
        World.PartyLocation GetPartyLocation();
        void UpdatePartyLocation(World.PartyLocation updatedLocation);
        // Location - End

        // Loot
        Loot.LootTable DEBUG_GetLootTable();
        // Loot - End

        // Party
        void AddCharacterToParty(int characterId);
        void AddToInventory(int itemId, int num = 1);
        void AssignTalentPoint(int characterId, Characters.TalentId talentId);
        void ChangeSpecialization(int characterId, Characters.SpecializationType specializationType);
        void EquipCharacter(int characterId, EquippableId equippableId, Items.Equipment.Slot inSlot);
        List<int> GetCharactersInParty(); 
        int GetCurrency(); 
        Inventory GetInventory();
        int GetPartyAvatarId();
        void PurchaseItem(int cost, int itemId);
        void RemoveCharacterFromParty(int characterId);
        void RemoveFromInventory(int itemId, int num = 1);
        void ResetTalentPoints(int characterId);
        void SellItem(int cost, int itemId);
        void UnEquipCharacter(int characterId, Items.Equipment.Slot inSlot);
        // Party - End

        // Save Load
        List<string> GetSaveFiles();
        void Load(string saveFileName);
        void Save();
        // SaveLoad End
    }

    abstract class WorldService : Services.ServiceBase<IWorldService> { };
}
