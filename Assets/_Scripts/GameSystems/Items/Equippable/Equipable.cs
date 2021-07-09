using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    class EquippableTag
    {
        public EquippableCategory Category;
        public int Type;

        public EquippableTag(ArmorType type) : this(EquippableCategory.Armor, (int)type) { }
        public EquippableTag(OneHandMeleeWeaponType type) : this(EquippableCategory.OneHandMelee, (int)type) { }
        public EquippableTag(TwoHandMeleeWeaponType type) : this(EquippableCategory.TwoHandMelee, (int)type) { }
        public EquippableTag(RangedWeaponType type) : this(EquippableCategory.Ranged, (int)type) { }
        public EquippableTag(ShieldType type) : this(EquippableCategory.Shield, (int)type) { }
        public EquippableTag(AccessoryType type) : this(EquippableCategory.Accessory, (int)type) { }
        public EquippableTag(EquippableCategory category, int type) { Category = category; Type = type; }
    }

    class Equippable : Item
    {
        public EquippableTag EquipmentTag;
        public EquippableId EquipmentId { get { return (EquippableId)ItemTag.ItemId; } }
        public ApparelAssetId PrefabId;

        public List<AttributeModifier> EquipBonuses;

        public Equippable(EquippableId id, EquippableTag tag, ApparelAssetId prefabId, UI.ItemIconSpriteId spriteId, List<AttributeModifier> equipBonuses, int value)
            : base(new ItemTag(id), spriteId, id.ToString(), value)
        {
            EquipmentTag = tag;
            EquipBonuses = equipBonuses;
            PrefabId = prefabId;
        }
    }

    class HeldEquippable : Equippable
    {
        public float BlockChance = 0;
        public float ParryChance = 0;
        public List<AttributeScalar> EffectivenessScalars = new List<AttributeScalar>();
        public int NumHandsRequired;

        public HeldEquippable(int numHandsRequired, float blockChance, float parryChance, List<AttributeScalar> effectivenessScalars, EquippableId id, EquippableTag tag, ApparelAssetId prefabId, UI.ItemIconSpriteId spriteId, List<AttributeModifier> modifiers, int value)
            : base(id, tag, prefabId, spriteId, modifiers, value)
        {
            BlockChance = blockChance;
            ParryChance = parryChance;
            EffectivenessScalars = effectivenessScalars;
            NumHandsRequired = numHandsRequired;
        }
    }

    class WeaponEquippable : HeldEquippable
    {
        public RangeInfo Range;
        public ActionProjectileInfo ProjectileInfo = new ActionProjectileInfo();
        public ActionAnimationInfo AnimationInfo = new ActionAnimationInfo();

        public WeaponEquippable(AnimationId animationId, ProjectileId projectileId, RangeInfo range, int numHandsRequired, float blockChance, float parryChance, List<AttributeScalar> effectivenessScalars, EquippableId id, EquippableTag tag, ApparelAssetId prefabId, UI.ItemIconSpriteId spriteId, List<AttributeModifier> modifiers, int value)
           : base(numHandsRequired, blockChance, parryChance, effectivenessScalars, id, tag, prefabId, spriteId, modifiers, value)
        {
            Range = range;
            ProjectileInfo.ProjectileId = projectileId;
            AnimationInfo.AnimationId = animationId;
        }
    }
}
