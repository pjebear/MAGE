using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Characters.Internal
{
    static class CharacterCreator
    {
        private static readonly string TAG = "CharacterCreator";

        public static Character FromInfo(CharacterInfo characterInfo)
        {
            Character character = new Character(characterInfo);

            // Equipment
            for (int i = 0; i < (int)Equipment.Slot.NUM; ++i)
            {
                EquippableId equippableId = characterInfo.EquippedItems[i];
                if (EquipmentUtil.IsHeld((Equipment.Slot)i) && equippableId == EquippableId.INVALID)
                {
                    equippableId = EquippableId.Fists_0;
                }

                if (equippableId != EquippableId.INVALID)
                {
                    Equippable equippable = ItemFactory.LoadEquipable((EquippableId)equippableId);

                    character.Equip(equippable, (Equipment.Slot)i);
                }
            }

            return character;
        }

        public static DB.DBCharacter CreateEmptyDBCharacter()
        {
            DB.DBCharacter emptyCharacter = new DB.DBCharacter();

            // Appearance
            emptyCharacter.AppearanceId = -1;

            // Equipment
            emptyCharacter.Equipment = Enumerable.Repeat((int)EquippableId.INVALID, (int)Equipment.Slot.NUM).ToList();

            // Attributes
            emptyCharacter.CharacterInfo.Attributes = Enumerable.Repeat(new DB.DBAttributes(), (int)AttributeCategory.NUM).ToList();
            emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.PrimaryStat] = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.PrimaryStat, Attributes = Enumerable.Repeat(0f, (int)PrimaryStat.NUM).ToList() };
            emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.SecondaryStat] = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.SecondaryStat, Attributes = Enumerable.Repeat(0f, (int)SecondaryStat.NUM).ToList() };
            emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.TertiaryStat] = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.TertiaryStat, Attributes = Enumerable.Repeat(0f, (int)TertiaryStat.NUM).ToList() };
            emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Resource] = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.Resource, Attributes = Enumerable.Repeat(0f, (int)ResourceType.NUM).ToList() };
            emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Allignment] = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.Allignment, Attributes = Enumerable.Repeat(0f, (int)AllignmentType.NUM).ToList() };
            emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Status] = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.Status, Attributes = Enumerable.Repeat(0f, (int)StatusType.NUM).ToList() };

            // Sanity Check
            foreach (DB.DBAttributes attributes in emptyCharacter.CharacterInfo.Attributes)
            {
                Logger.Assert(attributes.Attributes.Count > 0, LogTag.Character, TAG, "Missing Attributes", LogLevel.Warning);
            }

            return emptyCharacter;
        }

        public static void CreateCharacter(CharacterCreateParams createParams, out DB.DBCharacter dbCharacter, out DB.DBAppearance dbAppearance)
        {
            dbCharacter = CreateEmptyDBCharacter();
            dbAppearance = new DB.DBAppearance();

            // Info
            if (createParams.id == -1)
            {
                dbCharacter.Id = GetNextAvailableCharacterId(createParams.characterType);
            }
            else
            {
                dbCharacter.Id = createParams.id;
            }
            
            dbCharacter.CharacterInfo.Name = createParams.name;
            dbCharacter.CharacterInfo.CurrentSpecialization = (int)createParams.currentSpecialization;
            dbCharacter.CharacterInfo.Experience = 0;

            // Appearance
            dbCharacter.AppearanceId = dbCharacter.Id;

            dbAppearance = AppearanceUtil.ToDB(createParams.appearanceOverrides);

            dbAppearance.Id = dbCharacter.Id;

            // Equipment
            for (int i = 0; i < (int)Equipment.Slot.NUM; ++i)
            {
                dbCharacter.Equipment[i] = (int)createParams.currentEquipment[i];
            }

            // Specializations
            // Specializations
            List<SpecializationType> specializations = SpecializationUtil.GetSpecializationsForCharacter(createParams.characterClass, createParams.currentSpecialization);
            for (int i = 0; i < specializations.Count; ++i)
            {
                Specialization specialization = SpecializationFactory.CheckoutSpecialization(specializations[i]);
                dbCharacter.Specializations.Add(CharacterDBUtil.ToDB(specialization));
            }

            // Attributes
            PrimaryStat primaryStat = PrimaryStat.Might;
            switch (createParams.currentSpecialization)
            {
                case SpecializationType.Archer:
                {
                    primaryStat = PrimaryStat.Finese;
                }
                break;
                case SpecializationType.Bear:
                case SpecializationType.Footman:
                {
                    primaryStat = PrimaryStat.Might;
                }
                break;
                case SpecializationType.Monk:
                {
                    primaryStat = PrimaryStat.Magic;
                }
                break;
            }

            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.PrimaryStat].Attributes[(int)PrimaryStat.Might] = primaryStat == PrimaryStat.Might ? 20 : 10;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.PrimaryStat].Attributes[(int)PrimaryStat.Finese] = primaryStat == PrimaryStat.Finese ? 20 : 10;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.PrimaryStat].Attributes[(int)PrimaryStat.Magic] = primaryStat == PrimaryStat.Magic ? 20 : 10;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.SecondaryStat].Attributes[(int)SecondaryStat.Fortitude] = 60;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.SecondaryStat].Attributes[(int)SecondaryStat.Attunement] = 40;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.TertiaryStat].Attributes[(int)TertiaryStat.Movement] = 4;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.TertiaryStat].Attributes[(int)TertiaryStat.Jump] = 2;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.TertiaryStat].Attributes[(int)TertiaryStat.Speed] = 7;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.TertiaryStat].Attributes[(int)TertiaryStat.MaxClockGuage] = 100;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Resource].Attributes[(int)ResourceType.Health] = 0;
        }

        public static int GetNextAvailableCharacterId(CharacterType characterType)
        {
            int nextId = CharacterUtil.GetIdOffsetFromType(characterType);

            IEnumerable<int> existingIds = DBService.Get().GetAllCharacterIds().Where(x => CharacterUtil.GetCharacterTypeFromId(x) == characterType);

            for (int i = 0; i < 1000; ++i)
            {
                if (!existingIds.Contains(nextId))
                {
                    break;
                }
                else
                {
                    nextId++;
                }
            }

            return nextId;
        }
    }
}
