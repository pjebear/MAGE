using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class CharacterUtil
{
    public static DB.DBCharacter CreateEmptyCharacter()
    {
        DB.DBCharacter emptyCharacter = new DB.DBCharacter();

        emptyCharacter.EquipmentInfo.EquipmentIds = Enumerable.Repeat((int)EquippableId.INVALID, (int)Equipment.Slot.NUM).ToList();

        emptyCharacter.CharacterInfo.Attributes.Add(Enumerable.Repeat(0f, (int)CharacterStat.NUM).ToList());
        emptyCharacter.CharacterInfo.Attributes.Add(Enumerable.Repeat(0f, (int)ResourceType.NUM).ToList());
        emptyCharacter.CharacterInfo.Attributes.Add(Enumerable.Repeat(0f, (int)AllignmentType.NUM).ToList());
        emptyCharacter.CharacterInfo.Attributes.Add(Enumerable.Repeat(0f, (int)ProficiencyType.NUM).ToList());

        for (int i = 0; i < (int)SpecializationType.NUM; ++i)
        {
            emptyCharacter.SpecializationsInfo.Specializations[i].Level = 0;
            SpecializationInfo info = SpecializationFactory.CheckoutSpecializationInfo((SpecializationType)i);
            emptyCharacter.SpecializationsInfo.Specializations[i].SpentTalentPoints = Enumerable.Repeat(0, info.Talents.Count).ToList();
        }

        return emptyCharacter;
    }

    public static DB.DBCharacter CreateBaseCharacter(string name, SpecializationType specialization, List<int> equipmentIds)
    {
        DB.DBCharacter dbCharacter = CreateEmptyCharacter();

        dbCharacter.CharacterInfo.Name = name;
        dbCharacter.CharacterInfo.CurrentSpecialization = specialization;
        dbCharacter.CharacterInfo.Experience = 0;

        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat][(int)PrimaryStat.Might] = 10;
        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat][(int)PrimaryStat.Magic] = 5;
        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat][(int)TertiaryStat.Movement] = 5;
        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Resource][(int)ResourceType.Health] = 20;

        dbCharacter.EquipmentInfo.EquipmentIds = equipmentIds;
        dbCharacter.SpecializationsInfo.Specializations[(int)specialization].SpentTalentPoints = new List<int>() { 3, 1 };

        return dbCharacter;
    }
}

