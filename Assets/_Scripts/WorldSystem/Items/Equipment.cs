using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.EquipmentEnums
{

    public enum EquipmentType
    {
        Armor,
        Weapon,
        Shield,
        Accessory
    }

    public enum WeaponType
    {
        Unarmed,

        MeleeWeapon,
        Dagger,
        Sword,
        Axe,
        Mace,
        BastardSword,
        BattleAxe,
        Maul,
        Staff,
        MageSource,

        RangedWeapon,
        Bow,
        Longbow,
        Crossbow
    }

    enum ShieldType
    {
        Base,
        Tower
    }

    public enum ArmorType
    {
        None,
        Cloth,
        Leather,
        Chain,
        Plate
    }
}

namespace Common.EquipmentUtil
{
    using AttributeEnums;
    using AttributeTypes;
    using ActionEnums;
    using ActionTypes;
    using EquipmentEnums;
    using EncounterSystem.MapEnums;
    using EncounterSystem.MapTypes;

    static class EquipmentUtil
    {
        public static string GetWeaponAssetId(WeaponType type)
        {
            switch (type)
            {
                case (WeaponType.Unarmed):
                    return "";
                case (WeaponType.Dagger):
                    return "dagger_1";
                case (WeaponType.Sword):
                    return "sword_1";
                case (WeaponType.Axe):
                    return "axe_1";
                case (WeaponType.Mace):
                    return "mace_1";
                case (WeaponType.BattleAxe):
                    return "battleaxe_1";
                case (WeaponType.BastardSword):
                    return "bastardsword_1";
                case (WeaponType.Maul):
                    return "maul_1";
                case (WeaponType.Bow):
                    return "bow_1";
                case (WeaponType.Longbow):
                    return "longbow_1";
                case (WeaponType.Crossbow):
                    return "crossbow_1";
                case (WeaponType.Staff):
                    return "staff_1";
                case (WeaponType.MageSource):
                    return "rod_1";
                default:
                    UnityEngine.Debug.LogError("No weapon model loaded for weapon type " + type.ToString());
                    return "";
            }
        }

        public static string GetShieldModelId(ShieldType type)
        {
            switch (type)
            {
                case (ShieldType.Base):
                    return "shield_1";
                case (ShieldType.Tower):
                    return "towershield_1";
                default:
                    UnityEngine.Debug.LogError("No shield model loaded for shield type " + type.ToString());
                    return "";
            }
        }

        public static Dictionary<PhysicalActionType, float> GetWeaponDamageComposition(WeaponType type)
        {
            Dictionary<PhysicalActionType, float> damageComposition = new Dictionary<PhysicalActionType, float>();
            switch(type)
            {
                case (WeaponType.Unarmed):
                    break;
                case (WeaponType.Dagger):
                    damageComposition.Add(PhysicalActionType.Pierce, .75f);
                    damageComposition.Add(PhysicalActionType.Slash, .25f);
                    break;
                case (WeaponType.Sword):
                    damageComposition.Add(PhysicalActionType.Slash, 1f);
                    break;
                case (WeaponType.Axe):
                    damageComposition.Add(PhysicalActionType.Blunt, .25f);
                    damageComposition.Add(PhysicalActionType.Slash, .75f);
                    break;
                case (WeaponType.Mace):
                    damageComposition.Add(PhysicalActionType.Blunt, .5f);
                    damageComposition.Add(PhysicalActionType.Slash, .5f);
                    break;
                case (WeaponType.BattleAxe):
                    damageComposition.Add(PhysicalActionType.Blunt, 1f);
                    damageComposition.Add(PhysicalActionType.Slash, .25f);
                    break;
                case (WeaponType.BastardSword):
                    damageComposition.Add(PhysicalActionType.Blunt, .5f);
                    damageComposition.Add(PhysicalActionType.Slash, .75f);
                    break;
                case (WeaponType.Maul):
                    damageComposition.Add(PhysicalActionType.Blunt, 1.25f);
                    break;
                case (WeaponType.Bow):
                    damageComposition.Add(PhysicalActionType.Pierce, .75f);
                    break;
                case (WeaponType.Longbow):
                    damageComposition.Add(PhysicalActionType.Pierce, 1f);
                    break;
                case (WeaponType.Crossbow):
                    damageComposition.Add(PhysicalActionType.Pierce, 1.5f);
                    break;
                case (WeaponType.Staff):
                    break;
                case (WeaponType.MageSource):
                    break;
                default:
                    UnityEngine.Debug.LogError("No weapon damage composition loaded for weapon type " + type.ToString());
                    break;
            }
            return damageComposition;
        }

        public static List<ActionModifier> GetWeaponDamageModifiers(WeaponType type)
        {
            switch (type)
            {
                case (WeaponType.Unarmed):
                    return new List<ActionModifier>()
                    {
                        new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Might] * .5f; }, ModifierType.Additive)
                    };
                case (WeaponType.Staff):
                    return new List<ActionModifier>()
                    {
                        new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Might] * .75f; }, ModifierType.Additive)
                    };
                case (WeaponType.Dagger):
                    return new List<ActionModifier>()
                    {
                        new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Finese] * 1f; }, ModifierType.Additive)
                    };
                case (WeaponType.Sword):
                    return new List<ActionModifier>()
                    {
                        new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Might] * 0.5f; }, ModifierType.Additive)
                        , new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Finese] * 0.5f; }, ModifierType.Additive)
                    };
                case (WeaponType.Axe):
                    return new List<ActionModifier>()
                    {
                        new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Might] * 0.75f; }, ModifierType.Additive)
                        , new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Finese] * 0.25f; }, ModifierType.Additive)
                    };
                case (WeaponType.Mace):
                    return new List<ActionModifier>()
                    {
                        new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Might]; }, ModifierType.Additive)
                    };
                case (WeaponType.BastardSword):
                    return new List<ActionModifier>()
                    {
                        new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Might] * 1f; }, ModifierType.Additive)
                        , new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Finese] * 0.5f; }, ModifierType.Additive)
                    };
                case (WeaponType.BattleAxe):
                    return new List<ActionModifier>()
                    {
                        new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Might] * 1.25f; }, ModifierType.Additive)
                        , new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Finese] * 0.25f; }, ModifierType.Additive)
                    };
                case (WeaponType.Maul):
                    return new List<ActionModifier>()
                    {
                        new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Might] * 1.5f; }, ModifierType.Additive)
                    };
                case (WeaponType.Bow):
                    return new List<ActionModifier>()
                    {
                        new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Might] * 0.25f; }, ModifierType.Additive)
                        , new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Finese] * 0.75f; }, ModifierType.Additive)
                    };
                case (WeaponType.Longbow):
                    return new List<ActionModifier>()
                    {
                        new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Might] * 0.75f; }, ModifierType.Additive)
                        , new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Finese] * 0.75f; }, ModifierType.Additive)
                    };
                case (WeaponType.Crossbow):
                    return new List<ActionModifier>()
                    {
                         new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Finese] * 1f; }, ModifierType.Additive)
                    };
                case (WeaponType.MageSource):
                    return new List<ActionModifier>()
                    {
                         new ActionModifier((AttributeContainer container) => {return container[AttributeType.Stat][(int)PrimaryStat.Magic] * .25f; }, ModifierType.Additive),
                         new ActionModifier((AttributeContainer container) => {return 1 + container[AttributeType.Stat][(int)SecondaryStat.Attunement] * 0.02f; }, ModifierType.Multiplicative)
                    };
                default:
                    UnityEngine.Debug.LogError("No weapon damage type loaded for weapon type " + type.ToString());
                    return new List<ActionModifier>();
            }
        }

        public static int GetNumHandsRequired(WeaponType type)
        {
            switch (type)
            {
                case (WeaponType.Unarmed):
                    return 1;
                case (WeaponType.Dagger):
                    return 1;
                case (WeaponType.Sword):
                    return 1;
                case (WeaponType.Axe):
                    return 1;
                case (WeaponType.Mace):
                    return 1;
                case (WeaponType.BastardSword):
                    return 2;
                case (WeaponType.BattleAxe):
                    return 2;
                case (WeaponType.Maul):
                    return 2;
                case (WeaponType.Bow):
                    return 2;
                case (WeaponType.Longbow):
                    return 2;
                case (WeaponType.Crossbow):
                    return 2;
                case (WeaponType.Staff):
                    return 2;
                case (WeaponType.MageSource):
                    return 2;
                default:
                    UnityEngine.Debug.LogError("Num hands required for weapon type " + type.ToString() + " not defined");
                    return 1;
            }
        }

        public static bool IsRangedWeapon(WeaponType type)
        {
            if (type == WeaponType.Bow
                || type == WeaponType.Longbow
                || type == WeaponType.Crossbow)
            {
                return true;
            }
            return false;
        }

        public static bool IsMeleeWeapon(WeaponType type)
        {
            if (!IsRangedWeapon(type) && type != WeaponType.Unarmed)
            {
                return true;
            }
            return false;
        }

        public static float GetArmorPhysicalResistance(ArmorType type)
        {
            switch(type)
            {
                case (ArmorType.Cloth):
                    return 0.1f;
                case (ArmorType.Leather):
                    return 0.2f;
                case (ArmorType.Chain):
                    return 0.25f;
                case (ArmorType.Plate):
                    return 0.35f;
                default:
                    UnityEngine.Debug.LogError("Physical Resistance not defined for armor type " + type.ToString());
                    return 0.0f;
            }
        }

        public static float GetShieldBlockChance(ShieldType type)
        {
            switch (type)
            {
                case (ShieldType.Base):
                    return 0.3f;
                case (ShieldType.Tower):
                    return 0.55f;
                default:
                    UnityEngine.Debug.LogError("Physical Resistance not defined for armor type " + type.ToString());
                    return 0.0f;
            }
        }

        public static MapInteractionInfo GetAttackAreaInfoForWeapon(WeaponType type)
        {
            int minRange, maxRange, rangeElevation, minAoe, maxAoe, aoeElevation;
            TileAreaType rangeAreaType, aoeAreaType;

            if (type == WeaponType.Longbow)
            {
                rangeAreaType = TileAreaType.Ring;
                aoeAreaType = TileAreaType.Circle;
                minRange = 3; maxRange = 5; rangeElevation = 3; minAoe = maxAoe = aoeElevation = 0;
            }
            else if (type == WeaponType.Bow) 
            {
                rangeAreaType = TileAreaType.Ring;
                aoeAreaType = TileAreaType.Circle;
                minRange = 3; maxRange = 4; rangeElevation = 3; minAoe = maxAoe = aoeElevation = 0;
            }
            else if (type == WeaponType.Crossbow)
            {
                rangeAreaType = TileAreaType.Circle;
                aoeAreaType = TileAreaType.Circle;
                minRange = 3; maxRange = 4; rangeElevation = 3; minAoe = maxAoe = aoeElevation = 0;
            }
            else if (type == WeaponType.MageSource)
            {
                rangeAreaType = TileAreaType.Circle;
                aoeAreaType = TileAreaType.Circle;
                minRange = 1; maxRange = 3; rangeElevation = 3; minAoe = maxAoe = aoeElevation = 0;
            }
            else
            {
                rangeAreaType = TileAreaType.Cross;
                aoeAreaType = TileAreaType.Circle;
                minRange = 1; maxRange = 1; rangeElevation = 1; minAoe = maxAoe = aoeElevation = 0;
            }
            return new MapInteractionInfo(TargetSelectionType.Targeted, rangeAreaType, aoeAreaType, maxRange, minRange, rangeElevation, maxAoe, minAoe, aoeElevation);
        }

        public static ActionIndex GetActionIndexForWeapon(WeaponType type)
        {
            switch (type)
            {
                case (WeaponType.Unarmed):
                    return ActionIndex.MELEE_WEAPON;
                case (WeaponType.Staff):
                    return ActionIndex.MELEE_WEAPON;
                case (WeaponType.Dagger):
                    return ActionIndex.MELEE_WEAPON;
                case (WeaponType.Sword):
                    return ActionIndex.MELEE_WEAPON;
                case (WeaponType.Axe):
                    return ActionIndex.MELEE_WEAPON;
                case (WeaponType.Mace):
                    return ActionIndex.MELEE_WEAPON;
                case (WeaponType.BastardSword):
                    return ActionIndex.MELEE_WEAPON;
                case (WeaponType.BattleAxe):
                    return ActionIndex.MELEE_WEAPON;
                case (WeaponType.Maul):
                    return ActionIndex.MELEE_WEAPON;
                case (WeaponType.Bow):
                    return ActionIndex.RANGED_WEAPON;
                case (WeaponType.Longbow):
                    return ActionIndex.RANGED_WEAPON;
                case (WeaponType.Crossbow):
                    return ActionIndex.RANGED_WEAPON;
                case (WeaponType.MageSource):
                    return ActionIndex.SPELL_WEAPON;
                default:
                    UnityEngine.Debug.LogError("ActionIndex for weapon type " + type.ToString() + " not defined");
                    return ActionIndex.NUM;
            }
        }
    }
}

namespace Common.EquipmentTypes
{

    using EquipmentUtil;
    using EquipmentEnums;
    using AttributeTypes;
    using ActionTypes;
    using AttributeEnums;
    using ActionEnums;
    using StatusTypes;
    using StatusEnums;
    using EncounterSystem.Action;
    using EncounterSystem.MapTypes;
    using WorldSystem.Items;

    public class EquipmentProficienciesContainer
    {
        public bool HasDualWieldProficiency;
        public bool HasShieldProficiency { get { return mProficiencies[EquipmentType.Shield].Count > 0; } }
        private Dictionary<EquipmentType, List<int>> mProficiencies;

        public List<int> this[EquipmentType type] { get { return mProficiencies[type]; } }
        public EquipmentProficienciesContainer()
        {
            mProficiencies = new Dictionary<EquipmentType, List<int>>();
            foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
            {
                mProficiencies.Add(equipmentType, new List<int>());
            }
            HasDualWieldProficiency = false;
        }

        public EquipmentProficienciesContainer(EquipmentProficienciesContainer copy)
            : this()
        {
            foreach (var proficiency in copy.mProficiencies)
            {
                mProficiencies[proficiency.Key].AddRange(proficiency.Value);
            }
        }

        public void AddProficiency(EquipmentType type, int area)
        {
            if (!mProficiencies[type].Contains(area))
            {
                mProficiencies[type].Add(area);
            }
        }

        public void RemoveProficiency(EquipmentType type, int area)
        {
            if (mProficiencies.ContainsKey(type))
            {
                mProficiencies[type].Remove(area);
            }
        }

        public bool CanEquip(EquipmentBase equipment)
        {
            int area = -1;
            switch (equipment.EquipmentType)
            {
                case (EquipmentType.Weapon):
                    area = (int)(equipment as WeaponBase).WeaponType;
                    break;
                case (EquipmentType.Armor):
                    area = (int)(equipment as ArmorBase).ArmorType;
                    break;
                case (EquipmentType.Shield):
                    area = (int)(equipment as ShieldBase).ShieldType;
                    break;
                case (EquipmentType.Accessory):
                    return true; //TODO: For now?
            }
            if (area != -1 && mProficiencies.ContainsKey(equipment.EquipmentType))
            {
                return mProficiencies[equipment.EquipmentType].Contains(area);
            }
            else
            {
                UnityEngine.Debug.LogError("Equipment Type " + equipment.EquipmentType.ToString() + " Not initialized in Can Equip!");
            }
            UnityEngine.Debug.Log("Equipment Type " + equipment.EquipmentType.ToString() + " fell through in Can Equip!");
            return true;
        }
    }

    public abstract class EquipmentBase : ItemBase
    {
        public EquipmentType EquipmentType { protected set; get; }
        public List<StatusEffect> EquipmentEffects { protected set; get; }     
        public EquipmentBase(EquipmentType type, string name, string inventoryAssetId)
            : base (name, ItemType.Equippable, inventoryAssetId)
        {
            EquipmentType = type;
            EquipmentEffects = new List<StatusEffect>();
        }
    }

    public abstract class HeldEquipment : EquipmentBase
    {
        public int NumHandsRequired { protected set; get; }
        public string ModelId { protected set; get; }
        protected HeldEquipment(int numHandsRequired, EquipmentType type, string id, string name, string inventoryAssetId)
            : base (type, name, inventoryAssetId)
        {
            ModelId = id;
            NumHandsRequired = numHandsRequired;
        }
    }

    public class WeaponBase : HeldEquipment
    {
        public WeaponType WeaponType { get; protected set; }
        public List<ActionModifier> DamageModifiers;
        public Dictionary<PhysicalActionType, float> DamageComposition;
        public ActionIndex ActionIndex;
        public MapInteractionInfo AttackAreaInfo;

        public WeaponBase(WeaponType type)
            :base (EquipmentUtil.GetNumHandsRequired(type), EquipmentType.Weapon, EquipmentUtil.GetWeaponAssetId(type),
                 type.ToString(), EquipmentUtil.GetWeaponAssetId(type))
        {
            WeaponType = type;

            DamageModifiers = EquipmentUtil.GetWeaponDamageModifiers(type);
            DamageComposition = EquipmentUtil.GetWeaponDamageComposition(type);
            ActionIndex = EquipmentUtil.GetActionIndexForWeapon(type);
            AttackAreaInfo = EquipmentUtil.GetAttackAreaInfoForWeapon(type);
        }
    }

    class ShieldBase : HeldEquipment
    {
        public ShieldType ShieldType { get; private set; }
        public ShieldBase(ShieldType type)
            : base(1, EquipmentType.Shield, EquipmentUtil.GetShieldModelId(type), type.ToString(), EquipmentUtil.GetShieldModelId(type))
        {
            ShieldType = type;
            // Frontal block chance
            EquipmentEffects.Add(
                new StatusEffect(StatusEffectIndex.EQUIPPED_SHIELD_FRONTAL_BLOCK, "", StatusEffectProgression.INVALID, -1, true, false, false,
                (stackCount) =>
                {
                    return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.FrontalBlock), ModifierType.Additive,
                        (AttributeContainer container) => { return EquipmentUtil.GetShieldBlockChance(type); });
                }, null, null, null, true
               ));
            EquipmentEffects.Add(
                new StatusEffect(StatusEffectIndex.EQUIPPED_SHIELD_PERIFERAL_BLOCK, "", StatusEffectProgression.INVALID, -1, true, false, false,
                (stackCount) =>
                {
                    return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.PeriferalBlock), ModifierType.Additive,
                        (AttributeContainer container) => { return EquipmentUtil.GetShieldBlockChance(type) / 2f; });
                }, null, null, null, true
               ));

        }
    }

    public class ArmorBase : EquipmentBase
    {
        public ArmorType ArmorType { get; protected set; }
        public ArmorBase(ArmorType type)
            : base (EquipmentType.Armor, type.ToString(), "default")
        {
            ArmorType = type;
            EquipmentEffects.Add(
                new StatusEffect(StatusEffectIndex.EQUIPPED_ARMOR_PHYSICAL_RESISTANCE, "", StatusEffectProgression.INVALID, -1, true, false, false,
                (stackCount) =>
                {
                    return new AttributeModifier(new AttributeIndex(AttributeType.Stat, (int)TertiaryStat.PhysicalResistance), ModifierType.Additive,
                        (AttributeContainer container) => { return EquipmentUtil.GetArmorPhysicalResistance(type); });
                }, null, null, null, true
               ));

        }
    }
}
