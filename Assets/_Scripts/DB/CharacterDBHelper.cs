using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    static class CharacterHelper
    {
        private static string TAG = "CharacterHelper";

        public static int GetNextAvailableCreateCharacterId()
        {
            int nextCreateCharacterId = CharacterConstants.CREATE_CHARACTER_ID_OFFSET;

            IEnumerable<int> createCharacterIds = DB.DBHelper.GetAllCharacterIds().Where(x => CharacterUtil.GetCharacterTypeFromId(x) == CharacterType.Create);

            for (int i = 0; i < 1000; ++i)
            {
                if (!createCharacterIds.Contains(nextCreateCharacterId))
                {
                    break;
                }
                else
                {
                    nextCreateCharacterId++;
                }
            }

            return nextCreateCharacterId;
        }

        public static CharacterInfo FromDB(int characterId)
        {
            return FromDB(DB.DBHelper.LoadCharacter(characterId));
        }

        public static CharacterInfo FromDB(DB.DBCharacter dbCharacter)
        {
            CharacterInfo character = new CharacterInfo();

            // Character Info
            character.Id = dbCharacter.Id;
            character.Name = dbCharacter.CharacterInfo.Name;
            character.Level = dbCharacter.CharacterInfo.Level;
            character.Experience = dbCharacter.CharacterInfo.Experience;
            character.Attributes = FromDB(dbCharacter.CharacterInfo.Attributes);
            character.Actions.Add(ActionId.WeaponAttack);

            character.Appearance = FromDB(dbCharacter.Appearance);

            // Specializations
            
            for (int i = 0; i < (int)SpecializationType.NUM; ++i)
            {
                character.Specializations[i] = SpecializationFactory.CheckoutSpecialization((SpecializationType)i, dbCharacter.Specializations[i]);
            }

            // Apply current specialization
            {
                character.CurrentSpecializationType = (SpecializationType)dbCharacter.CharacterInfo.CurrentSpecialization;
                Specialization specialization = character.CurrentSpecialization;

                character.Actions.AddRange(specialization.Actions);
                character.Auras.AddRange(specialization.Auras);
                character.Listeners.AddRange(specialization.ActionResponses);

                foreach (Talent talent in specialization.Talents.Values)
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
            }

            // Equipment
            for (int i = 0; i < (int)Equipment.Slot.NUM; ++i)
            {
                if (EquipmentUtil.IsHeld((Equipment.Slot)i) && dbCharacter.Equipment[i] == (int)EquippableId.INVALID)
                {
                    dbCharacter.Equipment[i] = (int)EquippableId.Fists_0;
                }

                if (dbCharacter.Equipment[i] != (int)EquippableId.INVALID)
                {
                    Equippable equippable = ItemFactory.LoadEquipable((EquippableId)dbCharacter.Equipment[i]);

                    CharacterUtil.EquipCharacter(character, equippable, (Equipment.Slot)i);
                }
            }

            return character;
        }

        public static DB.DBCharacter ToDB(CharacterInfo character)
        {
            DB.DBCharacter dbCharacter = new DB.DBCharacter();

            // Character Info
            dbCharacter.Id = character.Id;
            dbCharacter.CharacterInfo.Name = character.Name;
            dbCharacter.CharacterInfo.Level = character.Level;
            dbCharacter.CharacterInfo.Experience = character.Experience;
            dbCharacter.CharacterInfo.Attributes = ToDB(character.Attributes);
            dbCharacter.CharacterInfo.CurrentSpecialization = (int)character.CurrentSpecializationType;

            // Appearance
            dbCharacter.Appearance = ToDB(character.Appearance);

            // Specializations
            for (int i = 0; i < (int)SpecializationType.NUM; ++i)
            {
                dbCharacter.Specializations.Add(ToDB(character.Specializations[i]));
            }

            // Equipment
            dbCharacter.Equipment = ToDB(character.Equipment);

            return dbCharacter;
        }

        // Appearance
        public static Appearance FromDB(DB.Character.DBAppearance dbAppearance)
        {
            Appearance fromDB = new Appearance();

            fromDB.PortraitSpriteId = (PortraitSpriteId)dbAppearance.PortraitSpriteId;
            fromDB.BodyType = (BodyType)dbAppearance.BodyType;
            fromDB.ArmorId = (AppearancePrefabId)dbAppearance.ArmorPrefabId;
            fromDB.LeftHeldId = (AppearancePrefabId)dbAppearance.HeldLeftPrefabId;
            fromDB.RightHeldId = (AppearancePrefabId)dbAppearance.HeldRightPrefabId;

            return fromDB;
        }

        public static DB.Character.DBAppearance ToDB(Appearance appearance)
        {
            DB.Character.DBAppearance toDB = new DB.Character.DBAppearance();

            toDB.PortraitSpriteId = (int)appearance.PortraitSpriteId;
            toDB.BodyType = (int)appearance.BodyType;
            toDB.ArmorPrefabId = (int)appearance.ArmorId;
            toDB.HeldLeftPrefabId = (int)appearance.LeftHeldId;
            toDB.HeldRightPrefabId = (int)appearance.RightHeldId;

            return toDB;
        }

        // Attributes
        public static Attributes FromDB(List<DB.DBAttributes> dBAttributes)
        {
            Attribute[][] fromDB = Attributes.Empty;

            for (int attributeCategory = 0; attributeCategory < dBAttributes.Count; ++attributeCategory)
            {
                Logger.Assert(dBAttributes[attributeCategory].AttributeCategory == attributeCategory, LogTag.Character, "Attributes",
                    string.Format("Invalid attribute category for db attributes. Expected {0}, Got {1}",
                    ((AttributeCategory)attributeCategory).ToString(), dBAttributes[attributeCategory].AttributeCategory.ToString()), LogLevel.Error);

                Logger.Assert(dBAttributes[attributeCategory].Attributes.Count == fromDB[attributeCategory].Length, LogTag.Character, "Attributes",
                    string.Format("Invalid attribute length from db for attribute type {0}. Expected {1}, Got {2}",
                    attributeCategory, fromDB[attributeCategory].Length, dBAttributes[attributeCategory].Attributes.Count), LogLevel.Error);

                if (dBAttributes[attributeCategory].Attributes.Count == fromDB[attributeCategory].Length
                    && dBAttributes[attributeCategory].AttributeCategory == attributeCategory)
                {
                    for (int attributeIdx = 0; attributeIdx < fromDB[attributeCategory].Length; ++attributeIdx)
                    {
                        fromDB[attributeCategory][attributeIdx].Set(dBAttributes[attributeCategory].Attributes[attributeIdx]);
                    }
                }
            }

            return new Attributes(fromDB);
        }

        public static List<DB.DBAttributes> ToDB(Attributes attributes)
        {
            List<DB.DBAttributes> toDB = new List<DB.DBAttributes>();

            for (int attributeCategory = 0; attributeCategory < (int)AttributeCategory.NUM; ++attributeCategory)
            {
                DB.DBAttributes dbAttributes = new DB.DBAttributes();
                dbAttributes.AttributeCategory = attributeCategory;

                for (int attributeIdx = 0; attributeIdx < attributes[(AttributeCategory)attributeCategory].Length; ++attributeIdx)
                {
                    DB.DBAttribute dbAttribute = new DB.DBAttribute();

                    dbAttributes.Attributes.Add(attributes[(AttributeCategory)attributeCategory][attributeIdx].Base);
                }

                toDB.Add(dbAttributes);
            }

            return toDB;
        }

        // Equipment
        public static List<int> ToDB(Equipment equipment)
        {
            List<int> toDB = new List<int>();

            for (int i = 0; i < (int)Equipment.Slot.NUM; ++i)
            {
                int equipmentId = EquipmentUtil.IsSlotEmpty(equipment, (Equipment.Slot)i) ?
                    (int)EquippableId.INVALID
                    : (int)equipment[(Equipment.Slot)i].EquipmentId;
                toDB.Add(equipmentId);
            }

            return toDB;
        }

        // Specialization 
        public static DB.Character.DBSpecializationInfo ToDB(Specialization specialization)
        {
            DB.Character.DBSpecializationInfo toDB = new DB.Character.DBSpecializationInfo();

            toDB.SpecializationType = (int)specialization.SpecializationType;
            toDB.Experience = specialization.Experience;
            toDB.Level = specialization.Level;
            foreach (var talent in specialization.Talents)
            {
                DB.Character.Talent dbTalent = new DB.Character.Talent();

                dbTalent.TalentId = (int)talent.Key;
                dbTalent.AssignedPoints = talent.Value.PointsAssigned;

                toDB.Talents.Add(dbTalent);
            }

            return toDB;
        }
    }
}


