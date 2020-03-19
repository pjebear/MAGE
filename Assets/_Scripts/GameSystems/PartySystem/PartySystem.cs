using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PartySystem
{
    private string TAG = "PartySystem";

    public void CreateDefaultParty()
    {
        DB.DBHelper.WriteNewCharacter(
            CharacterUtil.CreateBaseCharacter(
                "Rheinhardt",
                SpecializationType.Footman,
                new List<int>() { (int)EquippableId.ChainArmor_0, (int)EquippableId.Shield_0, (int)EquippableId.Sword_0, (int)EquippableId.INVALID }),
            TeamSide.AllyHuman);

        DB.DBHelper.WriteNewCharacter(
            CharacterUtil.CreateBaseCharacter(
                "Asmund",
                SpecializationType.Monk,
                new List<int>() { (int)EquippableId.ClothArmor_0, (int)EquippableId.Staff_0, (int)EquippableId.INVALID, (int)EquippableId.INVALID }),
            TeamSide.AllyHuman);
    }

    public void UpdatePartyOnEncounterEnd(EncounterResultInfo resultInfo)
    {
        foreach (int characterId in resultInfo.PlayersInEncounter[TeamSide.AllyHuman])
        {
            DB.DBCharacter character = DB.DBHelper.LoadCharacter(characterId);

            // Add Experience
            DB.CharacterInfo characterInfo = character.CharacterInfo;
            characterInfo.Experience += CharacterConstants.LEVEL_UP_THRESHOLD;
            if (characterInfo.Experience >= CharacterConstants.LEVEL_UP_THRESHOLD)
            {
                characterInfo.Experience -= CharacterConstants.LEVEL_UP_THRESHOLD;
                characterInfo.Level++;

                foreach (AttributeModifier modifier in SpecializationFactory.CheckoutSpecializationInfo(characterInfo.CurrentSpecialization).LevelUpModifiers)
                {
                    Logger.Assert(modifier.ModifierType == ModifierType.Increment, LogTag.GameSystems, TAG,
                        string.Format("Invalid Levelup modifier for Specialization [{0}] - {1}", characterInfo.CurrentSpecialization.ToString(), modifier.ToString()), LogLevel.Warning);

                    characterInfo.Attributes[(int)modifier.AttributeIndex.Type][modifier.AttributeIndex.Index] += modifier.Delta;
                }
            }

            // Update specialization
            DB.SpecializationInfo specializationInfo = character.SpecializationsInfo.Specializations[(int)characterInfo.CurrentSpecialization];
            specializationInfo.Experience += SpecializationConstants.LEVEL_UP_THRESHOLD;
            if (specializationInfo.Experience >= SpecializationConstants.LEVEL_UP_THRESHOLD)
            {
                specializationInfo.Level++;
            }

            DB.DBHelper.WriteCharacter(character);
        }
    }
}

