using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PartySystem
{
    private string TAG = "PartySystem";

    private string SaveFileName = "";

    private Party Party = new Party();

    public void CreateDefaultParty()
    {
        DB.DBCharacter Rheinhardt = CharacterUtil.CreateBaseCharacter( (int)StoryCharacterId.Rheinhardt, StoryCharacterId.Rheinhardt.ToString(), SpecializationType.Footman,
                new List<int>() { (int)EquippableId.ChainArmor_0, (int)EquippableId.Shield_0, (int)EquippableId.Sword_0, (int)EquippableId.INVALID });

        DB.DBHelper.WriteCharacter(Rheinhardt);

        Party.CharacterIds.Add(Rheinhardt.Id);
        foreach (int equippedItem in Rheinhardt.Equipment.EquipmentIds)
        {
            if (equippedItem != (int)EquippableId.INVALID)
            {
                Party.Inventory.Add(equippedItem);
            }
        }

        DB.DBCharacter Asmund = CharacterUtil.CreateBaseCharacter( (int)StoryCharacterId.Asmund, StoryCharacterId.Asmund.ToString(), SpecializationType.Monk,
                new List<int>() { (int)EquippableId.ClothArmor_0, (int)EquippableId.Staff_0, (int)EquippableId.INVALID, (int)EquippableId.INVALID });

        DB.DBHelper.WriteCharacter(Asmund);

        Party.CharacterIds.Add(Asmund.Id);
        foreach (int equippedItem in Asmund.Equipment.EquipmentIds)
        {
            if (equippedItem != (int)EquippableId.INVALID)
            {
                Party.Inventory.Add(equippedItem);
            }
        }
    }

    public List<int> GetCharactersInParty()
    {
        return new List<int>(Party.CharacterIds);
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
        DB.DBHelper.ClearTeam(TeamSide.AllyHuman, false);

        foreach (int characterId in resultInfo.PlayersInEncounter[TeamSide.AllyHuman])
        {
            DB.DBCharacter character = DB.DBHelper.LoadCharacter(characterId);

            // Add Experience
            DB.DBCharacterInfo characterInfo = character.CharacterInfo;
            characterInfo.Experience += CharacterConstants.LEVEL_UP_THRESHOLD;
            if (characterInfo.Experience >= CharacterConstants.LEVEL_UP_THRESHOLD)
            {
                characterInfo.Experience -= CharacterConstants.LEVEL_UP_THRESHOLD;
                characterInfo.Level++;

                foreach (AttributeModifier modifier in SpecializationFactory.CheckoutSpecializationInfo(characterInfo.CurrentSpecialization).LevelUpModifiers)
                {
                    Logger.Assert(modifier.ModifierType == ModifierType.Increment, LogTag.GameSystems, TAG,
                        string.Format("Invalid Levelup modifier for Specialization [{0}] - {1}", characterInfo.CurrentSpecialization.ToString(), modifier.ToString()), LogLevel.Warning);

                    characterInfo.Attributes[(int)modifier.AttributeIndex.Type].Attributes[modifier.AttributeIndex.Index] += modifier.Delta;
                }
            }

            // Update specialization
            DB.SpecializationInfo specializationInfo = character.Specializations.Specializations[(int)characterInfo.CurrentSpecialization];
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

        Save();
    }

    public void Save()
    {
        string saveFileName = SaveFileName != "" ? SaveFileName : SaveLoadUtil.GetNextAvailableSaveFileName();

        SaveLoad.SaveFile saveFile = new SaveLoad.SaveFile();
        saveFile.Name = SaveFileName != "" ? SaveFileName : SaveLoadUtil.GetNextAvailableSaveFileName();

        SaveLoadUtil.AddParty(saveFile, Party);

        SaveLoadUtil.Save(saveFile);
    }

    public void Load(string saveFileName)
    {
        SaveLoad.SaveFile saveFile = SaveLoadUtil.Load(saveFileName);

        SaveFileName = saveFile.Name;
        Party = SaveLoadUtil.ExtractParty(saveFile);
    }
}

