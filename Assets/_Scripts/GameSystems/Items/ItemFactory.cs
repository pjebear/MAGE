using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems
{ 
    static class ItemFactory
    {
        public static Item LoadItem(int id)
        {
            ItemType type = ItemUtil.TypeFromId(id);

            Item toReturn = null;
            switch (type)
            {
                case ItemType.Equippable:
                    toReturn = LoadEquipable((EquippableId)id);
                    break;
                case ItemType.Story:
                    toReturn = LoadStoryItem((StoryItemId)id);
                    break;
                case ItemType.Vendor:
                    toReturn = LoadVendorItem((VendorItemId)id);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            return toReturn;
        }

        public static Item LoadStoryItem(StoryItemId id)
        {
            DB.DBItem dbItem = DBService.Get().LoadItem((int)id);

            UI.ItemIconSpriteId spriteId = (UI.ItemIconSpriteId)dbItem.SpriteId;

            return new StoryItem(id, spriteId);
        }

        public static Item LoadVendorItem(VendorItemId id)
        {
            DB.DBItem dbItem = DBService.Get().LoadItem((int)id);

            UI.ItemIconSpriteId spriteId = (UI.ItemIconSpriteId)dbItem.SpriteId;

            return new VendorItem(id, spriteId, dbItem.Value);
        }

        public static Equippable LoadEquipable(EquippableId id)
        {
            //Equippable equippable = new Equippable();
            DB.DBEquipment dbEquipment = DBService.Get().LoadEquipment((int)id);

            EquippableTag tag = new EquippableTag((EquippableCategory)dbEquipment.Category, dbEquipment.Type);

            // Appearance
            ApparelAssetId appearancePrefabId = (ApparelAssetId)dbEquipment.PrefabId;
            UI.ItemIconSpriteId spriteId = (UI.ItemIconSpriteId)dbEquipment.SpriteId;

            // Equip Bonuses
            List<AttributeModifier> equipBonuses = new List<AttributeModifier>();
            foreach (DB.DBAttributeModifier dbModifier in dbEquipment.EquipBonuses)
            {
                equipBonuses.Add(new AttributeModifier(
                    new AttributeIndex((AttributeCategory)dbModifier.AttributeCategory, dbModifier.AttributeId)
                    , (ModifierType)dbModifier.ModifierType
                    , dbModifier.Modifier));
            }

            // Proficiency Modifiers
            List<AttributeScalar> proficiencyBonuses = new List<AttributeScalar>();
            foreach (DB.DBAttributeScalar attributeScalar in dbEquipment.EffectivenessScalars)
            {
                proficiencyBonuses.Add(new AttributeScalar(
                    new AttributeIndex((AttributeCategory)attributeScalar.AttributeCategory, attributeScalar.AttributeId)
                    , attributeScalar.Scalar));
            }

            EquippableCategory category = (EquippableCategory)dbEquipment.Category;

            Equippable equippable = null;

            switch (category)
            {
                case EquippableCategory.Accessory:
                case EquippableCategory.Armor:
                    equippable = new Equippable(id, tag, appearancePrefabId, spriteId, equipBonuses, dbEquipment.Value);
                    break;

                case EquippableCategory.EmptyHandMelee:
                case EquippableCategory.OneHandMelee:
                case EquippableCategory.TwoHandMelee:
                case EquippableCategory.Ranged:
                case EquippableCategory.Shield:
                {

                    int numHandsRequired = (category == EquippableCategory.OneHandMelee || category == EquippableCategory.EmptyHandMelee || category == EquippableCategory.Shield) ? 1 : 2;
                    AnimationId animationId = (AnimationId)dbEquipment.AnimationId;
                    ProjectileId projectileId = (ProjectileId)dbEquipment.ProjectileId;

                    if (category == EquippableCategory.Shield)
                    {
                        equippable = new HeldEquippable(numHandsRequired, dbEquipment.BlockChance, dbEquipment.ParryChance, proficiencyBonuses, id, tag, appearancePrefabId, spriteId, equipBonuses, dbEquipment.Value);
                    }
                    else
                    {
                        RangeInfo range = new RangeInfo(dbEquipment.Range.Min, dbEquipment.Range.Max, dbEquipment.Range.Elevation, (AreaType)dbEquipment.Range.AreaType, TargetingType.Any);
                        equippable = new WeaponEquippable(animationId, projectileId, range, numHandsRequired, dbEquipment.BlockChance, dbEquipment.ParryChance, proficiencyBonuses, id, tag, appearancePrefabId, spriteId, equipBonuses, dbEquipment.Value);
                    }
                }
                break;

                default:
                    Debug.Assert(false);
                    break;
            }

            return equippable;
        }
    }
}


