using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PartySystem
{
    private string TAG = "PartySystem";

    private string mSaveFileName = "";

    private Party Party = new Party();

    public int GetPartyAvatarId()
    {
        return Party.AvatarId;
    }

    public List<int> GetCharactersInParty()
    {
        return new List<int>(Party.CharacterIds);
    }

    public Inventory GetInventory()
    {
        return Party.Inventory;
    }

    public void AddToInventory(int itemId)
    {
        Party.Inventory.Add(itemId);
    }

    public void UnEquipCharacter(int characterId, Equipment.Slot inSlot, bool returnToInventory = true)
    {
        DB.DBCharacter dbCharacter = DB.DBHelper.LoadCharacter(characterId);
        Logger.Assert(dbCharacter.Id != -1, LogTag.GameSystems, TAG, string.Format("UnEquipCharacter() - Failed to find character [{0}] in db.", characterId));
        if (dbCharacter.Id != -1)
        {
            CharacterInfo toUnEquip = DB.CharacterHelper.FromDB(dbCharacter);
            Optional<int> unequipped = CharacterUtil.UnEquipCharacter(toUnEquip, inSlot);
            if (unequipped.HasValue)
            {
                Party.Inventory.Add(unequipped.Value);
            }
        }
    }

    public void EquipCharacter(int characterId, EquippableId equippableId, Equipment.Slot inSlot, bool pullFromInventory = true)
    {
        DB.DBCharacter dbCharacter = DB.DBHelper.LoadCharacter(characterId);
        Logger.Assert(dbCharacter.Id != -1, LogTag.GameSystems, TAG, string.Format("EquipCharacter() - Failed to find character [{0}] in db.", characterId));
        if (dbCharacter.Id != -1)
        {
            CharacterInfo toEquip = DB.CharacterHelper.FromDB(dbCharacter);

            if (pullFromInventory)
            {
                Logger.Assert(Party.Inventory.Contains((int)equippableId), LogTag.GameSystems, TAG, string.Format("EquipCharacter() - Failed to find item in inventory [{0}].", equippableId.ToString()));
                if (Party.Inventory.Contains((int)equippableId))
                {
                    Party.Inventory.Remove((int)equippableId);
                }
            }

            Equippable equipable = ItemFactory.LoadEquipable(equippableId);

            List<int> unequippedItems = CharacterUtil.EquipCharacter(toEquip, equipable, inSlot);
            foreach (int itemId in unequippedItems)
            {
                Party.Inventory.Add(itemId);
            }
        }
    }

    public void PrepareForEncounter(EncounterCreateParams createParams)
    {
        foreach (int characterId in Party.CharacterIds)
        {
            DB.DBHelper.AddToTeam(characterId, TeamSide.AllyHuman);
        }
    }

    public void UpdateOnEncounterEnd(EncounterResultInfo resultInfo)
    {
        DB.DBHelper.ClearTeam(TeamSide.AllyHuman);

        foreach (int characterId in resultInfo.PlayersInEncounter[TeamSide.AllyHuman])
        {
            DB.DBCharacter character = DB.DBHelper.LoadCharacter(characterId);

            // Add Experience
            DB.Character.DBCharacterInfo characterInfo = character.CharacterInfo;
            characterInfo.Experience += CharacterConstants.LEVEL_UP_THRESHOLD;
            if (characterInfo.Experience >= CharacterConstants.LEVEL_UP_THRESHOLD)
            {
                characterInfo.Experience -= CharacterConstants.LEVEL_UP_THRESHOLD;
                characterInfo.Level++;

                foreach (AttributeModifier modifier in SpecializationFactory.CheckoutSpecialization((SpecializationType)characterInfo.CurrentSpecialization).LevelUpModifiers)
                {
                    Logger.Assert(modifier.ModifierType == ModifierType.Increment, LogTag.GameSystems, TAG,
                        string.Format("Invalid Levelup modifier for Specialization [{0}] - {1}", characterInfo.CurrentSpecialization.ToString(), modifier.ToString()), LogLevel.Warning);

                    characterInfo.Attributes[(int)modifier.AttributeIndex.Type].Attributes[modifier.AttributeIndex.Index] += modifier.Delta;
                }
            }

            // Update specialization
            DB.Character.DBSpecializationInfo specializationInfo = character.Specializations[characterInfo.CurrentSpecialization];
            specializationInfo.Experience += SpecializationConstants.LEVEL_UP_THRESHOLD;
            if (specializationInfo.Experience >= SpecializationConstants.LEVEL_UP_THRESHOLD)
            {
                specializationInfo.Level++;
            }

            DB.DBHelper.WriteCharacter(character);
        }

        // Add new Items
        foreach (int itemReward in resultInfo.ItemRewards)
        {
            Party.Inventory.Add(itemReward);
        }

        // Currency Reward
        Party.Currency += resultInfo.CurrencyReward;
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

        SaveLoadUtil.AddParty(saveFile, Party);

        SaveLoadUtil.Save(saveFile);
    }

    public void Load(string saveFileName)
    {
        SaveLoad.SaveFile saveFile = SaveLoadUtil.Load(saveFileName);

        mSaveFileName = saveFile.Name;
        Party = SaveLoadUtil.ExtractParty(saveFile);
    }

    #region DEBUG
    public void CreateDefaultParty()
    {
        Party.CharacterIds.Add((int)StoryCharacterId.Rheinhardt);
        Party.CharacterIds.Add((int)StoryCharacterId.Asmund);

        List<int> defaultInventory = new List<int>()
        {
             (int)EquippableId.Axe_0
             , (int)EquippableId.Mace_0
             , (int)EquippableId.Sword_0
             , (int)EquippableId.LeatherArmor_0
        };

        foreach (int itemId in defaultInventory)
        {
            Party.Inventory.Add(itemId);
        }

        Party.AvatarId = (int)StoryCharacterId.Rheinhardt;
    }
    #endregion
}

