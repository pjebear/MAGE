using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Characters.Internal
{
    static class MobFactory
    {
        public static CharacterCreateParams GetCreateParamsForMob(Mobs.MobId mobId, int level)
        {
            CharacterCreateParams createParams = new CharacterCreateParams();
            createParams.characterType = CharacterType.Temporary;
            createParams.characterClass = CharacterClass.MonoSpecialization;
            createParams.level = level;

            switch (mobId)
            {
                case Mobs.MobId.DEMO_Bear:
                {
                    createParams.currentSpecialization = SpecializationType.Bear;
                    createParams.name = "Grizzly Bear";
                    createParams.appearanceOverrides = new Appearance();
                    createParams.appearanceOverrides.BodyType = BodyType.Bear_0;
                    createParams.appearanceOverrides.OverridePortraitSpriteId = PortraitSpriteId.Bear;
                }
                break;

                case Mobs.MobId.DEMO_Bandit:
                {
                    createParams.name = "Bandit";
                    createParams.appearanceOverrides = new Appearance();
                    createParams.appearanceOverrides.OverridePortraitSpriteId = PortraitSpriteId.Bandit_0;

                    createParams.currentEquipment[(int)Equipment.Slot.Armor] = EquippableId.LeatherArmor_0;

                    // Soldier
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        createParams.currentSpecialization = SpecializationType.Footman;
                        createParams.currentEquipment[(int)Equipment.Slot.RightHand] = EquippableId.Sword_0;
                        createParams.currentEquipment[(int)Equipment.Slot.LeftHand] = EquippableId.Shield_0;
                    }
                    else // Archer
                    {
                        createParams.currentSpecialization = SpecializationType.Archer;
                        createParams.currentEquipment[(int)Equipment.Slot.RightHand] = EquippableId.Bow_0;
                    }
                }
                break;
            }

            return createParams;
        }
    }
}
