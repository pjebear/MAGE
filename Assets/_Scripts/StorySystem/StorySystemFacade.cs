using Common.CharacterEnums;
using Common.EquipmentEnums;
using Common.EquipmentTypes;
using Common.ProfessionEnums;
using StorySystem.Common;
using StorySystem.StoryArcSystem;
using StorySystem.StoryCast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Character;
using WorldSystem.Interface;

namespace StorySystem.Interface
{
    class StorySystemFacade
    {
        private StoryArcManager mStoryArcManager;

        private static StorySystemFacade mInstance;

        public static StorySystemFacade Instance()
        {
            if (mInstance == null)
            {
                mInstance = new StorySystemFacade();
                UnityEngine.Debug.Log("Accessing Story System Facade before properly initialized");
            }
            return mInstance;
        }

        public StorySystemFacade()
        {
            UnityEngine.Debug.Log("Creating StorySystemFacade...");
            mInstance = this;
            mStoryArcManager = new StoryArcManager();
        }

        public void Initialize()
        {
            WorldSystemFacade instance = WorldSystemFacade.Instance();
            instance.AddCharacterToRoster(StoryCastFactory.CheckoutStoryCharacter(StoryCharacterId.Rheinhardt, 3, UnitGroup.Player));
            instance.AddCharacterToRoster(StoryCastFactory.CheckoutStoryCharacter(StoryCharacterId.Asmund, 2, UnitGroup.Player));
            instance.AddCharacterToRoster(StoryCastFactory.CheckoutStoryCharacter(StoryCharacterId.Thanos, 1, UnitGroup.Player));
            instance.AddCharacterToRoster(StoryCastFactory.CheckoutStoryCharacter(StoryCharacterId.Magnus, 2, UnitGroup.Player));
            instance.AddCharacterToRoster(new CharacterBase(UnitGroup.Player, 2000, "Soldier " + 1, (CharacterGender)UnityEngine.Random.Range(0, 1), ProfessionType.Footman, 2));
            instance.AddCharacterToRoster(new CharacterBase(UnitGroup.Player, 2001, "Soldier " + 2, (CharacterGender)UnityEngine.Random.Range(0, 1), ProfessionType.Adept, 2));
            instance.AddCharacterToRoster(new CharacterBase(UnitGroup.Player, 2002, "Soldier " + 3, (CharacterGender)UnityEngine.Random.Range(0, 1), ProfessionType.Archer, 2));
            instance.AddEquipmentToInventory(new WeaponBase(WeaponType.Axe));
            instance.AddEquipmentToInventory(new WeaponBase(WeaponType.Sword));
            instance.AddEquipmentToInventory(new WeaponBase(WeaponType.Mace));
            instance.AddEquipmentToInventory(new WeaponBase(WeaponType.Longbow));
            instance.AddEquipmentToInventory(new ShieldBase(ShieldType.TowerShield));
            instance.AddEquipmentToInventory(new ArmorBase(ArmorType.Cloth));
            instance.AddEquipmentToInventory(new ArmorBase(ArmorType.Leather));
            instance.AddEquipmentToInventory(new ArmorBase(ArmorType.Chain));
            instance.AddEquipmentToInventory(new ArmorBase(ArmorType.Plate));
        }

        public void BeginNewStoryArc(StoryArcId arcId)
        {
            mStoryArcManager.BeginNewStoryArc(arcId);
        }

        public void NotifyStoryEvent(StoryEventPayload eventPayload)
        {
            mStoryArcManager.ProgressStoryArcs(eventPayload);
        }
    }
}
