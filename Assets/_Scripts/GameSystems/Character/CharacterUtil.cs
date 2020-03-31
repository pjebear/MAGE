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

        // Equipment
        emptyCharacter.Equipment = Enumerable.Repeat((int)EquippableId.INVALID, (int)Equipment.Slot.NUM).ToList();

        // Attributes
        emptyCharacter.CharacterInfo.Attributes = Enumerable.Repeat(new DB.DBAttributes(), (int)AttributeCategory.NUM).ToList();
        emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat]        = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.Stat,           Attributes = Enumerable.Repeat(0f, (int)CharacterStat.NUM).ToList() };
        emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Resource]    = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.Resource,       Attributes = Enumerable.Repeat(0f, (int)ResourceType.NUM).ToList() };
        emptyCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Allignment]  = new DB.DBAttributes() { AttributeCategory = (int)AttributeCategory.Allignment,     Attributes = Enumerable.Repeat(0f, (int)AllignmentType.NUM).ToList() };
        
        // Sanity Check
        foreach (DB.DBAttributes attributes in emptyCharacter.CharacterInfo.Attributes)
        {
            Logger.Assert(attributes.Attributes.Count > 0, LogTag.Character, TAG, "Missing Attributes", LogLevel.Warning);
        }

        // Specializations
        for (int i = 0; i < (int)SpecializationType.NUM; ++i)
        {
            emptyCharacter.Specializations.Add(new DB.Character.DBSpecializationInfo() { SpecializationType = i });
        }

        return emptyCharacter;
    }

    public static DB.DBCharacter CreateBaseCharacter(int id, string name, SpecializationType specialization, List<int> equipmentIds)
    {
        DB.DBCharacter dbCharacter = CreateEmptyCharacter();

        dbCharacter.Id = id;
        dbCharacter.CharacterInfo.Name = name;
        dbCharacter.CharacterInfo.CurrentSpecialization = (int)specialization;
        dbCharacter.CharacterInfo.Experience = 0;

        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)PrimaryStat.Might] = 10;
        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)PrimaryStat.Magic] = 5;
        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Stat].Attributes[(int)TertiaryStat.Movement] = 5;
        dbCharacter.CharacterInfo.Attributes[(int)AttributeCategory.Resource].Attributes[(int)ResourceType.Health] = 20;

        dbCharacter.Equipment = equipmentIds;

        return dbCharacter;
    }

    public static int GetNextAvailableCreateCharacterId()
    {
        int nextCreateCharacterId = CharacterConstants.CREATE_CHARACTER_ID_OFFSET;

        IEnumerable<int> createCharacterIds = DB.DBHelper.GetAllCharacterIds().Where(x => GetCharacterTypeFromId(x) == CharacterType.Create);

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

    public static CharacterType GetCharacterTypeFromId(int id)
    {
        CharacterType characterType = CharacterType.Temporary;

        if (id >= CharacterConstants.STORY_CHARACTER_ID_OFFSET)
        {
            characterType = CharacterType.Story;
        }
        else if (id >= CharacterConstants.CREATE_CHARACTER_ID_OFFSET)
        {
            characterType = CharacterType.Create;
        }

        return characterType;
    }

    public static ActorSpawnParams ActorParamsForCharacter(DB.DBCharacter dBCharacter)
    {
        ActorSpawnParams spawnParams = new ActorSpawnParams();

        spawnParams.BodyType[AppearanceType.Prefab] = (int)AppearancePrefabId.Body_0;

        int slotId = (int)Equipment.Slot.Armor;
        if (dBCharacter.Equipment[slotId] != (int)EquippableId.INVALID)
        {
            spawnParams.Worn = ItemFactory.LoadEquipable((EquippableId)dBCharacter.Equipment[slotId]).Appearance;
        }

        slotId = (int)Equipment.Slot.LeftHand;
        if (dBCharacter.Equipment[slotId] != (int)EquippableId.INVALID)
        {
            spawnParams.HeldLeftHand = ItemFactory.LoadEquipable((EquippableId)dBCharacter.Equipment[slotId]).Appearance;
        }

        slotId = (int)Equipment.Slot.RightHand;
        if (dBCharacter.Equipment[slotId] != (int)EquippableId.INVALID)
        {
            spawnParams.HeldRightHand = ItemFactory.LoadEquipable((EquippableId)dBCharacter.Equipment[slotId]).Appearance;
        }

        return spawnParams;
    }

    public static void RefreshCharactersEquipment(int characterId)
    {
        DB.DBCharacter dBCharacter = DB.DBHelper.LoadCharacter(characterId);

        SpecializationType currentSpecializationType = (SpecializationType)dBCharacter.CharacterInfo.CurrentSpecialization;

        Specialization specialization = SpecializationFactory.CheckoutSpecialization(currentSpecializationType, dBCharacter.Specializations[(int)currentSpecializationType]);

        bool triggerRefresh = false;

        // TODO: Update when Proficiencies have been updated
        for(int i = 0; i < (int)Equipment.Slot.NUM; ++i)
        {
            if (dBCharacter.Equipment[i] != (int)EquippableId.INVALID)
            {
                Equippable equipable = ItemFactory.LoadEquipable((EquippableId)dBCharacter.Equipment[i]);
                if (!EquipmentUtil.HasProficiencyFor(specialization, equipable))
                {
                    Logger.Log(LogTag.Character, TAG, string.Format("::RefreshCharactersEquipment() - Removed Equipable [{0}] from character [{1}]", equipable.EquipmentId.ToString(), characterId), LogLevel.Notify);

                    dBCharacter.Equipment[i] = (int)EquippableId.INVALID;

                    triggerRefresh = true;
                }
            }
        }

        if (triggerRefresh)
        {
            DB.DBHelper.WriteCharacter(dBCharacter);
        }
    }
}

