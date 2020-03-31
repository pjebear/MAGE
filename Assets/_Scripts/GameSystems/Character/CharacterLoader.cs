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
        character.Actions.AddRange(character.Specialization.Actions);
        character.Auras.AddRange(character.Specialization.Auras);
        character.Listeners.AddRange(character.Specialization.ActionResponses);

        foreach (Talent talent in character.Specialization.Talents.Values)
        {
            foreach (AttributeModifier proficiencyModifier in talent.GetAttributeModifiers())
            {
                character.Attributes.Modify(proficiencyModifier);
            }

            character.Actions.AddRange(talent.GetActions());
            character.EquippableModifiers.AddRange(talent.GetEquippableModifiers());
            character.Auras.AddRange(talent.GetAuras());
            character.Listeners.AddRange(talent.GetActionResponses());
            character.ActionModifiers.AddRange(talent.GetActionModifiers());
        }

        // Equipment
        for (int i = 0; i < (int)Equipment.Slot.NUM; ++i)
        {
            if (dbCharacter.Equipment[i] == (int)EquippableId.INVALID 
                && (i == (int)Equipment.Slot.LeftHand || i == (int)Equipment.Slot.RightHand))
            {
                dbCharacter.Equipment[i] = (int)EquippableId.Fists_0;
            }

            if (dbCharacter.Equipment[i] != (int)EquippableId.INVALID)
            {
                Equippable equippable = ItemFactory.LoadEquipable((EquippableId)dbCharacter.Equipment[i]);

                foreach (EquippableModifier equippableModifier in character.EquippableModifiers)
                {
                    equippableModifier.Modify(equippable);
                }

                character.Equipment[(Equipment.Slot)i] = equippable;

                foreach (AttributeModifier attributeModifier in equippable.EquipBonuses)
                {
                    character.Attributes.Modify(attributeModifier);
                }
            }
        }

        return character;
    }
}

