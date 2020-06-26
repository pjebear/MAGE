using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class CharacterDBLoader
{
    public static void LoadDB()
    {
        // Story Characters
        DB.DBCharacter dbCharacter = CharacterUtil.CreateBaseDBCharacter((int)StoryCharacterId.Rheinhardt, StoryCharacterId.Rheinhardt.ToString(), BodyType.Body_0, PortraitSpriteId.Rheinhardt, SpecializationType.Footman);
        DB.DBHelper.WriteCharacter(dbCharacter);
        {
            CharacterInfo character = DB.CharacterHelper.FromDB(dbCharacter);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.ChainArmor_0), Equipment.Slot.Armor);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.Shield_0), Equipment.Slot.LeftHand);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.Mace_0), Equipment.Slot.RightHand);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.Relic), Equipment.Slot.Accessory);

            DB.DBHelper.WriteCharacter(DB.CharacterHelper.ToDB(character));
        }

        dbCharacter = CharacterUtil.CreateBaseDBCharacter((int)StoryCharacterId.Asmund, StoryCharacterId.Asmund.ToString(), BodyType.Body_0, PortraitSpriteId.Asmund, SpecializationType.Monk);
        DB.DBHelper.WriteCharacter(dbCharacter);
        {
            CharacterInfo character = DB.CharacterHelper.FromDB(dbCharacter);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.ClothArmor_0), Equipment.Slot.Armor);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.Staff_0), Equipment.Slot.RightHand);

            DB.DBHelper.WriteCharacter(DB.CharacterHelper.ToDB(character));
        }

        dbCharacter = CharacterUtil.CreateBaseDBCharacter((int)StoryCharacterId.Lothar, StoryCharacterId.Lothar.ToString(), BodyType.Body_0, PortraitSpriteId.Lothar, SpecializationType.Footman);
        DB.DBHelper.WriteCharacter(dbCharacter);
        {
            CharacterInfo character = DB.CharacterHelper.FromDB(dbCharacter);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.LeatherArmor_0), Equipment.Slot.Armor);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.Axe_0), Equipment.Slot.RightHand);

            DB.DBHelper.WriteCharacter(DB.CharacterHelper.ToDB(character));
        }

        dbCharacter = CharacterUtil.CreateBaseDBCharacter((int)StoryCharacterId.Maric, StoryCharacterId.Maric.ToString(), BodyType.Body_0, PortraitSpriteId.Maric, SpecializationType.Footman);
        DB.DBHelper.WriteCharacter(dbCharacter);
        {
            CharacterInfo character = DB.CharacterHelper.FromDB(dbCharacter);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.ChainArmor_0), Equipment.Slot.Armor);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.Shield_0), Equipment.Slot.LeftHand);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.Sword_0), Equipment.Slot.RightHand);

            DB.DBHelper.WriteCharacter(DB.CharacterHelper.ToDB(character));
        }

        // Scenario Characters
        int scenarioCharacterId = 0;
        int dbId = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, scenarioCharacterId++);
        dbCharacter = CharacterUtil.CreateBaseDBCharacter(dbId, "BanditLeader", BodyType.Body_0, PortraitSpriteId.BanditLeader, SpecializationType.Footman);
        DB.DBHelper.WriteCharacter(dbCharacter);
        {
            CharacterInfo character = DB.CharacterHelper.FromDB(dbCharacter);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.ChainArmor_0), Equipment.Slot.Armor);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.Shield_0), Equipment.Slot.LeftHand);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.Sword_0), Equipment.Slot.RightHand);

            DB.DBHelper.WriteCharacter(DB.CharacterHelper.ToDB(character));
        }

        dbId = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, scenarioCharacterId++);
        dbCharacter = CharacterUtil.CreateBaseDBCharacter(dbId, "Bandit", BodyType.Body_0, PortraitSpriteId.Bandit_0, SpecializationType.Footman);
        DB.DBHelper.WriteCharacter(dbCharacter);
        {
            CharacterInfo character = DB.CharacterHelper.FromDB(dbCharacter);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.LeatherArmor_0), Equipment.Slot.Armor);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.Sword_0), Equipment.Slot.RightHand);

            DB.DBHelper.WriteCharacter(DB.CharacterHelper.ToDB(character));
        }

        dbId = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, scenarioCharacterId++);
        dbCharacter = CharacterUtil.CreateBaseDBCharacter(dbId, "Bandit", BodyType.Body_0, PortraitSpriteId.Bandit_0, SpecializationType.Footman);
        DB.DBHelper.WriteCharacter(dbCharacter);
        {
            CharacterInfo character = DB.CharacterHelper.FromDB(dbCharacter);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.LeatherArmor_0), Equipment.Slot.Armor);
            CharacterUtil.EquipCharacter(character, ItemFactory.LoadEquipable(EquippableId.Sword_0), Equipment.Slot.RightHand);

            DB.DBHelper.WriteCharacter(DB.CharacterHelper.ToDB(character));
        }
    }
}

