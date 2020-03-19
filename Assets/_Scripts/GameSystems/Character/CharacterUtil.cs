using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class CharacterUtil
{
    private static string TAG = "CharacterUtil";

    public static DB.DBCharacter CreateEmptyCharacter()
    {
        DB.DBCharacter emptyCharacter = new DB.DBCharacter();

        emptyCharacter.Equipment.EquipmentIds = Enumerable.Repeat((int)EquippableId.INVALID, (int)Equipment.Slot.NUM).ToList();

        emptyCharacter.CharacterInfo.Attributes = Enumerable.Repeat(new DB.DBAttributes(), (int)AttributeCategory.NUM).ToList();

        emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat]        = new DB.DBAttributes() { AttributeCategory = AttributeCategory.Stat,           Attributes = Enumerable.Repeat(0f, (int)CharacterStat.NUM).ToList() };
        emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Resource]    = new DB.DBAttributes() { AttributeCategory = AttributeCategory.Resource,       Attributes = Enumerable.Repeat(0f, (int)ResourceType.NUM).ToList() };
        emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Allignment]  = new DB.DBAttributes() { AttributeCategory = AttributeCategory.Allignment,     Attributes = Enumerable.Repeat(0f, (int)AllignmentType.NUM).ToList() };
        emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Proficiency] = new DB.DBAttributes() { AttributeCategory = AttributeCategory.Proficiency,    Attributes = Enumerable.Repeat(0f, (int)ProficiencyType.NUM).ToList() };

        foreach (DB.DBAttributes attributes in emptyCharacter.CharacterInfo.Attributes)
        {
            Logger.Assert(attributes.Attributes.Count > 0, LogTag.Character, TAG, "Missing Attributes", LogLevel.Warning);
        }
        

        for (int i = 0; i < (int)SpecializationType.NUM; ++i)
        {
            emptyCharacter.Specializations.Specializations[i].Level = 0;
            SpecializationInfo info = SpecializationFactory.CheckoutSpecializationInfo((SpecializationType)i);
            emptyCharacter.Specializations.Specializations[i].SpentTalentPoints = Enumerable.Repeat(0, info.Talents.Count).ToList();
        }

        return emptyCharacter;
    }

    public static DB.DBCharacter CreateBaseCharacter(string name, SpecializationType specialization, List<int> equipmentIds)
    {
        DB.DBCharacter dbCharacter = CreateEmptyCharacter();

        dbCharacter.CharacterInfo.Name = name;
        dbCharacter.CharacterInfo.CurrentSpecialization = specialization;
        dbCharacter.CharacterInfo.Experience = 0;

        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)PrimaryStat.Might] = 10;
        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)PrimaryStat.Magic] = 5;
        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)TertiaryStat.Movement] = 5;
        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Resource].Attributes[(int)ResourceType.Health] = 20;

        dbCharacter.Equipment.EquipmentIds = equipmentIds;
        dbCharacter.Specializations.Specializations[(int)specialization].SpentTalentPoints = new List<int>() { 3, 1 };

        return dbCharacter;
    }
}

