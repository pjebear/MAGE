﻿using System;
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
        World.EncounterContext GetEncounterContext();
        EncounterCreateParams GetEncounterParams();
        void PrepareEncounter(EncounterCreateParams encounterParams);
        void UpdateOnEncounterEnd(EncounterResultInfo resultInfo);
        // Encounter - End

        // Flow
        void PrepareNewGame();
        // Flow - End

        // Location
        World.PartyLocation GetPartyLocation();
        void UpdatePartyLocation(World.PartyLocation updatedLocation);
        // Location - End

        // Party
        void AddCharacterToParty(int characterId);
        void AddToInventory(int itemId);
        void AssignTalentPoint(int characterId, Characters.TalentId talentId);
        void ChangeSpecialization(int characterId, Characters.SpecializationType specializationType);
        void EquipCharacter(int characterId, EquippableId equippableId, Characters.Equipment.Slot inSlot);
        List<int> GetCharactersInParty(); 
        int GetCurrency(); 
        Inventory GetInventory();
        int GetPartyAvatarId();
        void PurchaseItem(int cost, int itemId);
        void RemoveCharacterFromParty(int characterId);
        void RemoveFromInventory(int itemId);
        void ResetTalentPoints(int characterId);
        void SellItem(int cost, int itemId);
        void UnEquipCharacter(int characterId, Characters.Equipment.Slot inSlot);
        // Party - End

        // Save Load
        List<string> GetSaveFiles();
        void Load(string saveFileName);
        void Save();
        // SaveLoad End
    }

    abstract class WorldService : Services.ServiceBase<IWorldService> { };
}