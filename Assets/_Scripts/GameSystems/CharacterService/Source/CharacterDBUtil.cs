
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attribute = MAGE.GameSystems.Stats.Attribute;

namespace MAGE.GameSystems.Characters.Internal
{
    static class CharacterDBUtil
    {
        public static int GetNextAvailableCreateCharacterId()
        {
            int nextCreateCharacterId = CharacterConstants.CREATE_CHARACTER_ID_OFFSET;

            IEnumerable<int> createCharacterIds = DBService.Get().GetAllCharacterIds().Where(x => CharacterUtil.GetCharacterTypeFromId(x) == CharacterType.Create);

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
            return FromDB(DBService.Get().LoadCharacter(characterId));
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
           
            character.AppearanceId = dbCharacter.AppearanceId;

            // Specializations
            character.CurrentSpecializationType = (SpecializationType)dbCharacter.CharacterInfo.CurrentSpecialization;
            for (int i = 0; i < dbCharacter.Specializations.Count; ++i)
            {
                DB.DBSpecializationProgress progress = dbCharacter.Specializations[i];
                character.SpecializationsProgress.Add((SpecializationType)progress.SpecializationType, FromDB(progress));
            }
            
            // Equipment
            for (int i = 0; i < (int)Equipment.Slot.NUM; ++i)
            {
                character.EquippedItems[i] = (EquippableId)dbCharacter.Equipment[i];
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
            dbCharacter.AppearanceId = character.AppearanceId;

            // Specializations
            foreach (SpecializationProgress progress in character.SpecializationsProgress.Values)
            {
                dbCharacter.Specializations.Add(ToDB(progress));
            }

            // Equipment
            foreach (EquippableId equippableId in character.EquippedItems)
            {
                dbCharacter.Equipment.Add((int)equippableId);
            }

            return dbCharacter;
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
                    AttributeIndex attributeIndex = new AttributeIndex((AttributeCategory)attributeCategory, attributeIdx);
                    dbAttributes.Attributes.Add(attributes[attributeIndex]);
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
        public static DB.DBSpecializationProgress ToDB(Specialization specialization)
        {
            DB.DBSpecializationProgress toDB = new DB.DBSpecializationProgress();

            toDB.SpecializationType = (int)specialization.SpecializationType;
            toDB.Experience = specialization.Experience;
            toDB.Level = specialization.Level;
            foreach (var talent in specialization.Talents)
            {
                DB.DBTalentProgress dbTalent = new DB.DBTalentProgress();

                dbTalent.TalentId = (int)talent.Key;
                dbTalent.AssignedPoints = talent.Value.PointsAssigned;
                dbTalent.MaxPoints = talent.Value.MaxPoints;

                toDB.Talents.Add(dbTalent);
            }

            return toDB;
        }

        public static DB.DBSpecializationProgress ToDB(SpecializationProgress progress)
        {
            DB.DBSpecializationProgress toDB = new DB.DBSpecializationProgress();

            toDB.SpecializationType = (int)progress.SpecializationType;
            toDB.Experience = progress.Experience;
            toDB.Level = progress.Level;
            foreach (var talent in progress.TalentProgress)
            {
                toDB.Talents.Add(ToDB(talent.Value));
            }

            return toDB;
        }

        public static SpecializationProgress FromDB(DB.DBSpecializationProgress dbProgress)
        {
            SpecializationProgress fromDB = new SpecializationProgress();

            fromDB.SpecializationType = (SpecializationType)dbProgress.SpecializationType;
            fromDB.Experience = dbProgress.Experience;
            fromDB.Level = dbProgress.Level;
            foreach (var dbTalent in dbProgress.Talents)
            {
                fromDB.TalentProgress.Add((TalentId)dbTalent.TalentId, FromDB(dbTalent));
            }

            return fromDB;
        }

        public static TalentProgress FromDB(DB.DBTalentProgress dbTalentProgress)
        {
            TalentProgress talentProgress = new TalentProgress();

            talentProgress.TalentId = (TalentId)dbTalentProgress.TalentId;
            talentProgress.CurrentPoints = dbTalentProgress.AssignedPoints;
            talentProgress.MaxPoints = dbTalentProgress.MaxPoints;

            return talentProgress;
        }

        public static DB.DBTalentProgress ToDB(TalentProgress talentProgress)
        {
            DB.DBTalentProgress toDB = new DB.DBTalentProgress();

            toDB.TalentId = (int)talentProgress.TalentId;
            toDB.AssignedPoints = talentProgress.CurrentPoints;
            toDB.MaxPoints = talentProgress.MaxPoints;

            return toDB;
        }
    }
}


