using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Character.Internal
{
    static class CharacterCreator
    {
        private static readonly string TAG = "CharacterCreator";

        public static DB.DBCharacter CreateEmptyDBCharacter()
        {
            DB.DBCharacter emptyCharacter = new DB.DBCharacter();

            // Appearance
            emptyCharacter.AppearanceId = -1;

            // Equipment
            emptyCharacter.Equipment = Enumerable.Repeat((int)EquippableId.INVALID, (int)Equipment.Slot.NUM).ToList();

            // Attributes
            emptyCharacter.CharacterInfo.Attributes = Enumerable.Repeat(new DB.DBAttributes(), (int)AttributeCategory.NUM).ToList();
            emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat] = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.Stat, Attributes = Enumerable.Repeat(0f, (int)CharacterStat.NUM).ToList() };
            emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Resource] = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.Resource, Attributes = Enumerable.Repeat(0f, (int)ResourceType.NUM).ToList() };
            emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Allignment] = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.Allignment, Attributes = Enumerable.Repeat(0f, (int)AllignmentType.NUM).ToList() };
            emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Status] = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.Status, Attributes = Enumerable.Repeat(0f, (int)StatusType.NUM).ToList() };

            // Sanity Check
            foreach (DB.DBAttributes attributes in emptyCharacter.CharacterInfo.Attributes)
            {
                Logger.Assert(attributes.Attributes.Count > 0, LogTag.Character, TAG, "Missing Attributes", LogLevel.Warning);
            }

            // Specializations
            for (int i = 0; i < (int)SpecializationType.NUM; ++i)
            {
                emptyCharacter.Specializations.Add(new DB.DBSpecializationProgress() { SpecializationType = i });
            }

            return emptyCharacter;
        }

        public static void CreateCharacter(CharacterCreateParams createParams, out DB.DBCharacter dbCharacter, out DB.DBAppearance dbAppearance)
        {
            dbCharacter = CreateEmptyDBCharacter();
            dbAppearance = new DB.DBAppearance();

            // Info
            if (createParams.characterType == CharacterType.Create)
            {
                dbCharacter.Id = GetNextAvailableCreateCharacterId();
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
            dbAppearance.Id = dbCharacter.Id;
            dbAppearance.BodyType = (int)BodyType.Body_0;
            dbAppearance.PortraitSpriteId = (int)createParams.portraitSpriteId;
            // Remaining Appearance will be updated on 'RefreshCharacterAppearance()'

            // Equipment
            for (int i = 0; i < (int)Equipment.Slot.NUM; ++i)
            {
                dbCharacter.Equipment[i] = (int)createParams.currentEquipment[i];
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

            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)PrimaryStat.Might] = primaryStat == PrimaryStat.Might ? 20 : 10;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)PrimaryStat.Finese] = primaryStat == PrimaryStat.Finese ? 20 : 10;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)PrimaryStat.Magic] = primaryStat == PrimaryStat.Magic ? 20 : 10;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)SecondaryStat.Fortitude] = 60;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)SecondaryStat.Attunement] = 40;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)TertiaryStat.Movement] = 5;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)TertiaryStat.Jump] = 2;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)TertiaryStat.Speed] = 7;
            dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Resource].Attributes[(int)ResourceType.Health] = 20;
        }

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
    }
}
