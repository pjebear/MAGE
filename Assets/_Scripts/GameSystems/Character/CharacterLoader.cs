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
        Character character = new Character();

        // Character Info
        character.Id = dbCharacter.Id;
        character.Name = dbCharacter.CharacterInfo.Name;
        character.Level = dbCharacter.CharacterInfo.Level;
        character.Experience = dbCharacter.CharacterInfo.Experience;
        character.Attributes = new Attributes(dbCharacter.CharacterInfo.Attributes);
        character.Actions.Add(ActionId.SwordAttack);

        // Specialization
        character.Specialization = SpecializationFactory.CheckoutSpecialization((SpecializationType)dbCharacter.CharacterInfo.CurrentSpecialization, dbCharacter.Specializations[dbCharacter.CharacterInfo.CurrentSpecialization]);
        foreach (Talent talent in character.Specialization.Talents.Values)
        {
            foreach (AttributeModifier proficiencyModifier in talent.GetAttributeModifiers())
            {
                character.Attributes.Modify(proficiencyModifier);
            }

            character.Actions.AddRange(talent.GetActions());
            character.Auras.AddRange(talent.GetAuras());
            character.Listeners.AddRange(talent.GetActionResponses());
            character.ActionModifiers.AddRange(talent.GetActionModifiers());
        }

        // Equipment
        for (int i = 0; i < (int)Equipment.Slot.NUM; ++i)
        {
            if (dbCharacter.Equipment[i] != (int)EquippableId.INVALID)
            {
                character.Equipment[(Equipment.Slot)i] = ItemFactory.CreateEquipable((ItemId)dbCharacter.Equipment[i]);
            }
        }

        return character;
    }

    public static void ApplySpecialization(Character character, Specialization specialization)
    {
        
    }
}

