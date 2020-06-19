using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class CharacterUtil
{
    private static string TAG = "CharacterUtil";

    public static int ScenarioIdToDBId(ScenarioId scenarioId, int characterId)
    {
        return
            CharacterConstants.SCENARIO_CHARACTER_ID_OFFSET
            + CharacterConstants.SUB_CATEGORY_RANGE * (int)scenarioId
            + characterId;
    }

    public static int DBIdToScenarioId(int dbId)
    {
        return dbId % CharacterConstants.SUB_CATEGORY_RANGE;
    }

    public static DB.DBCharacter CreateEmptyDBCharacter()
    {
        DB.DBCharacter emptyCharacter = new DB.DBCharacter();

        // Appearance
        emptyCharacter.Appearance = new DB.Character.DBAppearance();

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

    public static DB.DBCharacter CreateBaseDBCharacter(int id, string name, BodyType bodyType, PortraitSpriteId portraitSprite, SpecializationType specialization)
    {
        DB.DBCharacter dbCharacter = CreateEmptyDBCharacter();

        // Info
        dbCharacter.Id = id;
        dbCharacter.CharacterInfo.Name = name;
        dbCharacter.CharacterInfo.CurrentSpecialization = (int)specialization;
        dbCharacter.CharacterInfo.Experience = 0;

        // Appearance
        dbCharacter.Appearance.BodyType = (int)bodyType;
        dbCharacter.Appearance.PortraitSpriteId = (int)portraitSprite;

        // Attributes
        PrimaryStat primaryStat = PrimaryStat.Might;
        switch (specialization)
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

    public static Optional<int> UnEquipCharacter(CharacterInfo toUnEquip, Equipment.Slot inSlot)
    {
        Optional<int> unequipped = Optional<int>.Empty;
        if (!EquipmentUtil.IsSlotEmpty(toUnEquip.Equipment, inSlot))
        {
            Equippable unEquiped = toUnEquip.Equipment[inSlot];

            unequipped = (int)unEquiped.EquipmentId;

            // Remove equipment modifiers
            foreach (AttributeModifier attributeModifier in unEquiped.EquipBonuses)
            {
                toUnEquip.Attributes.Revert(attributeModifier);
            }

            if (EquipmentUtil.IsHeld(inSlot)) // replace with fists
            {
                toUnEquip.Equipment[inSlot] = ItemFactory.LoadEquipable(EquippableId.Fists_0);
                // Apply modifiers to the equipment itself incase it changes how it is equipped
                foreach (EquippableModifier equippableModifier in toUnEquip.EquippableModifiers)
                {
                    equippableModifier.Modify(toUnEquip.Equipment[inSlot]);
                }
            }
            else
            {
                toUnEquip.Equipment[inSlot] = Equipment.NO_EQUIPMENT;
            }

            CharacterUpdateAppearanceAfterEquipmentChange(toUnEquip, inSlot);
        }

        return unequipped;
    }

    public static List<int> EquipCharacter(CharacterInfo toEquip, Equippable equippable, Equipment.Slot inSlot)
    {
        List<int> unequippedItems = new List<int>();

        bool fitsInSlot = EquipmentUtil.FitsInSlot(equippable.EquipmentTag.Category, inSlot);
        bool hasProficiency = EquipmentUtil.HasProficiencyFor(toEquip.CurrentSpecialization, equippable);
        Logger.Assert(fitsInSlot && hasProficiency, LogTag.GameSystems, TAG, string.Format("EquipCharacter() - Item [{0}] doesn't fit in slot [{1}].", equippable.EquipmentId.ToString(), inSlot.ToString()));
        if (fitsInSlot && hasProficiency)
        {
            // Apply modifiers to the equipment itself incase it changes how it is equipped
            foreach (EquippableModifier equippableModifier in toEquip.EquippableModifiers)
            {
                equippableModifier.Modify(equippable);
            }

            // unequip slots that new item will now occupy
            if (!EquipmentUtil.IsSlotEmpty(toEquip.Equipment, inSlot))
            {
                unequippedItems.Add(UnEquipCharacter(toEquip, inSlot).Value);
            }

            if (inSlot == Equipment.Slot.LeftHand || inSlot == Equipment.Slot.RightHand)
            {
                int numHands = (equippable as HeldEquippable).NumHandsRequired;
                if (numHands == 2)
                {
                    Equipment.Slot otherSlot = inSlot == Equipment.Slot.LeftHand ? Equipment.Slot.RightHand : Equipment.Slot.LeftHand;
                    if (!EquipmentUtil.IsSlotEmpty(toEquip.Equipment,otherSlot))
                    {
                        unequippedItems.Add(UnEquipCharacter(toEquip, otherSlot).Value);
                    }
                }
            }

            // Finall, equip the item
            toEquip.Equipment[inSlot] = equippable;

            // Update character with equipment modifiers
            foreach (AttributeModifier attributeModifier in equippable.EquipBonuses)
            {
                toEquip.Attributes.Modify(attributeModifier);
            }

            CharacterUpdateAppearanceAfterEquipmentChange(toEquip, inSlot);
        }
        else
        {
            unequippedItems.Add((int)equippable.EquipmentId);
        }

        return unequippedItems;
    }

    public static void CharacterUpdateAppearanceAfterEquipmentChange(CharacterInfo toRefresh, Equipment.Slot changedSlot)
    {
        AppearancePrefabId newPrefabId = toRefresh.Equipment[changedSlot] == Equipment.NO_EQUIPMENT ? AppearancePrefabId.prefab_none : toRefresh.Equipment[changedSlot].PrefabId;

        switch (changedSlot)
        {
            case Equipment.Slot.Accessory: /* empty */ break;
            case Equipment.Slot.Armor: toRefresh.Appearance.ArmorId = newPrefabId; break;
            case Equipment.Slot.LeftHand: toRefresh.Appearance.LeftHeldId = newPrefabId; break;
            case Equipment.Slot.RightHand: toRefresh.Appearance.RightHeldId = newPrefabId; break;
        }
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

