using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class CharacterLoader
{
    private static string TAG = "CharacterLoader";

    public static Character LoadCharacter(int characterId)
    {
        return LoadCharacter(DB.DBHelper.LoadCharacter(characterId));
    }

    public static Character LoadCharacter(DB.DBCharacter dbCharacter)
    {
        Character character = new Character(dbCharacter.Id, dbCharacter.CharacterInfo);

        character.Actions.Add(ActionId.SwordAttack);

        ApplySpecialization(character, character.Specialization, dbCharacter.SpecializationsInfo.Specializations[(int)character.Specialization].SpentTalentPoints);

        Debug.Assert(dbCharacter.EquipmentInfo.EquipmentIds.Count == (int)Equipment.Slot.NUM);
        for (int i = 0; i < (int)Equipment.Slot.NUM; ++i)
        {
            if (dbCharacter.EquipmentInfo.EquipmentIds[i] != (int)EquippableId.INVALID)
            {
                character.Equipment[(Equipment.Slot)i] = ItemFactory.CreateEquipable((ItemId)dbCharacter.EquipmentInfo.EquipmentIds[i]);
            }
        }

        return character;
    }

    public static void ApplySpecialization(Character character, SpecializationType specializationType, List<int> assignedTalents)
    {
        SpecializationInfo info = SpecializationFactory.CheckoutSpecializationInfo(specializationType);


        Logger.Assert(assignedTalents.Count == info.Talents.Count, LogTag.Character, TAG,
            string.Format("::ApplySpecialization() - Found talent size mismatch for Character {0}. Expected {1} Got {2}", character.Id, info.Talents.Count, assignedTalents.Count));

        foreach (AttributeModifier proficiencyModifier in info.BaseProficiencyModifiers)
        {
            character.Attributes.Modify(proficiencyModifier);
        }

        character.Actions.AddRange(info.Actions);
        character.Auras.AddRange(info.Auras);
        character.Listeners.AddRange(info.Listeners);
        character.ActionModifiers.AddRange(info.ActionModifiers);


        if (assignedTalents.Count == info.Talents.Count)
        {
            for (int i = 0; i < info.Talents.Count; ++i)
            {
                
                Talent talent = TalentFactory.CheckoutTalent(info.Talents[i], assignedTalents[i]);

                foreach (AttributeModifier proficiencyModifier in talent.GetAttributeModifiers())
                {
                    character.Attributes.Modify(proficiencyModifier);
                }

                character.Actions.AddRange(talent.GetActions());
                character.Auras.AddRange(talent.GetAuras());
                character.Listeners.AddRange(talent.GetActionResponses());
                character.ActionModifiers.AddRange(talent.GetActionModifiers());
            }
        }
    }
}

