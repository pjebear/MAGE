﻿using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class EquipmentDBLoader
    {
        private static DB.DBRangeInfo MELEE_RANGE = new DB.DBRangeInfo() { Min = 0, Max = 2.5f, Elevation = 1, AreaType = (int)AreaType.Circle };
        private static DB.DBRangeInfo BOW_RANGE = new DB.DBRangeInfo() { Min = 4, Max = 18f, Elevation = 7, AreaType = (int)AreaType.Circle };

        private static int ACCESSORY_COST = 700;
        private static int TWO_HAND_WEAPON_COST = 600;
        private static int ONE_HAND_WEAPON_COST = 400;
        private static int ARMOR_COST = 800;

        public static void LoadDB()
        {
            #region Armor
            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Cloth Armor
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.ClothArmor_0;
                entry.Category = (int)EquippableCategory.Armor;
                entry.Type = (int)ArmorType.Cloth;
                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
                {
                    // empty
                };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.TertiaryStat, AttributeId = (int)TertiaryStat.PhysicalResistance, ModifierType = (int)ModifierType.Increment, Modifier = .15f}
            };
                entry.SpriteId = (int)UI.ItemIconSpriteId.Cloth;
                entry.PrefabId = (int)ApparelAssetId.Cloth_0;
                entry.Value = ARMOR_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Leather Armor
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.LeatherArmor_0;
                entry.Category = (int)EquippableCategory.Armor;
                entry.Type = (int)ArmorType.Leather;
                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
                {
                    // empty
                };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.TertiaryStat, AttributeId = (int)TertiaryStat.PhysicalResistance, ModifierType = (int)ModifierType.Increment, Modifier = .3f}
            };
                entry.SpriteId = (int)UI.ItemIconSpriteId.Leather;
                entry.PrefabId = (int)ApparelAssetId.Leather_0;
                entry.Value = ARMOR_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Chain Armor
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.ChainArmor_0;
                entry.Category = (int)EquippableCategory.Armor;
                entry.Type = (int)ArmorType.Chain;
                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
                {
                    // empty
                };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.TertiaryStat, AttributeId = (int)TertiaryStat.PhysicalResistance, ModifierType = (int)ModifierType.Increment, Modifier = .4f}
            };
                entry.SpriteId = (int)UI.ItemIconSpriteId.Chain;
                entry.PrefabId = (int)ApparelAssetId.Mail_0;
                entry.Value = ARMOR_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Plate Armor
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.PlateArmor_0;
                entry.Category = (int)EquippableCategory.Armor;
                entry.Type = (int)ArmorType.Plate;
                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
                {
                    // empty
                };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.TertiaryStat, AttributeId = (int)TertiaryStat.PhysicalResistance, ModifierType = (int)ModifierType.Increment, Modifier = .6f}
                };
                entry.SpriteId = (int)UI.ItemIconSpriteId.Plate;
                entry.PrefabId = (int)ApparelAssetId.Plate_0;
                entry.Value = ARMOR_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }
            #endregion // Armor

            #region OneHands
            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Fists
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Fists_0;
                entry.Category = (int)EquippableCategory.EmptyHandMelee;
                entry.Type = (int)EmptyHandMeleeWeaponType.Fist;
                entry.BlockChance = 0;
                entry.ParryChance = 0;
                entry.AnimationId = (int)AnimationId.SwordSwing;
                entry.Range = MELEE_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Scalar = .75f}
            };

                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty

                };
                entry.SpriteId = (int)UI.ItemIconSpriteId.INVALID;
                entry.PrefabId = (int)ApparelAssetId.NONE;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Axe
                DB.DBEquipment entry = new DB.DBEquipment();

                entry.Id = (int)EquippableId.Axe_0;
                entry.Name = EquippableId.Axe_0.ToString();
                entry.Category = (int)EquippableCategory.OneHandMelee;
                entry.Type = (int)OneHandMeleeWeaponType.Axe;
                entry.BlockChance = 0;
                entry.ParryChance = 15;
                entry.AnimationId = (int)AnimationId.SwordSwing;
                entry.Range = MELEE_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Scalar = .6f}
                , new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Finese, Scalar = .2f}
            };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty
                };

                entry.SpriteId = (int)UI.ItemIconSpriteId.Axe;
                entry.PrefabId = (int)ApparelAssetId.Axe_0;
                entry.Value = ONE_HAND_WEAPON_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Dagger
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Dagger_0;
                entry.Category = (int)EquippableCategory.OneHandMelee;
                entry.Type = (int)OneHandMeleeWeaponType.Dagger;
                entry.BlockChance = 0;
                entry.ParryChance = 10;
                entry.AnimationId = (int)AnimationId.DaggerStrike;
                entry.Range = MELEE_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
                {
                    new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Scalar = .2f}
                    , new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Finese, Scalar = .6f}
                };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty
                };

                entry.SpriteId = (int)UI.ItemIconSpriteId.Dagger;
                entry.PrefabId = (int)ApparelAssetId.Dagger_0;
                entry.Value = ONE_HAND_WEAPON_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Mace
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Mace_0;
                entry.Category = (int)EquippableCategory.OneHandMelee;
                entry.Type = (int)OneHandMeleeWeaponType.Mace;
                entry.BlockChance = 0;
                entry.ParryChance = 10;
                entry.AnimationId = (int)AnimationId.SwordSwing;
                entry.Range = MELEE_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Scalar = .8f}
            };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty
                };

                entry.SpriteId = (int)UI.ItemIconSpriteId.Mace;
                entry.PrefabId = (int)ApparelAssetId.Mace_0;
                entry.Value = ONE_HAND_WEAPON_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Sword
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Sword_0;
                entry.Category = (int)EquippableCategory.OneHandMelee;
                entry.Type = (int)OneHandMeleeWeaponType.Sword;
                entry.BlockChance = 0;
                entry.ParryChance = 20;
                entry.AnimationId = (int)AnimationId.SwordSwing;
                entry.Range = MELEE_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Scalar = .4f}
                , new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Finese, Scalar = .4f}
            };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty
                };

                entry.SpriteId = (int)UI.ItemIconSpriteId.Sword;
                entry.PrefabId = (int)ApparelAssetId.Sword_0;
                entry.Value = ONE_HAND_WEAPON_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }
            #endregion // one hands

            #region TwoHands
            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Staff
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Staff_0;
                entry.Category = (int)EquippableCategory.TwoHandMelee;
                entry.Type = (int)TwoHandMeleeWeaponType.Staff;
                entry.BlockChance = 0;
                entry.ParryChance = 0;
                entry.AnimationId = (int)AnimationId.SwordSwing;
                entry.Range = MELEE_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Scalar = .25f}
            };

                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Magic, ModifierType = (int)ModifierType.Multiply, Modifier = .1f}
            };

                entry.SpriteId = (int)UI.ItemIconSpriteId.Staff;
                entry.PrefabId = (int)ApparelAssetId.Staff_0;
                entry.Value = TWO_HAND_WEAPON_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }
            #endregion // TwoHands

            #region Ranged
            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Bow
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Bow_0;
                entry.Category = (int)EquippableCategory.Ranged;
                entry.Type = (int)RangedWeaponType.Bow;
                entry.BlockChance = 0;
                entry.ParryChance = 0;
                entry.AnimationId = (int)AnimationId.BowDraw;
                entry.ProjectileId = (int)ProjectileId.Arrow;
                entry.Range = BOW_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Scalar = .4f},
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Finese, Scalar = .4f}
            };

                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty
                };

                entry.SpriteId = (int)UI.ItemIconSpriteId.Bow;
                entry.PrefabId = (int)ApparelAssetId.Bow_0;
                entry.Value = TWO_HAND_WEAPON_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }
            #endregion

            #region Shields
            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Shield
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Shield_0;
                entry.Category = (int)EquippableCategory.Shield;
                entry.Type = (int)ShieldType.Shield;
                entry.BlockChance = 20;
                entry.ParryChance = 0;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
                {
                    // empty
                };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty
                };
                entry.SpriteId = (int)UI.ItemIconSpriteId.Shield;
                entry.PrefabId = (int)ApparelAssetId.Shield_0;
                entry.Value = ONE_HAND_WEAPON_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }
            #endregion

            #region Accessories
            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Relic
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Relic;
                entry.Category = (int)EquippableCategory.Accessory;
                entry.Type = (int)AccessoryType.StatBonus;
                entry.BlockChance = 0;
                entry.ParryChance = 0;
                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
                {
                    // empty
                };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, ModifierType = (int)ModifierType.Increment, Modifier = 5}
                ,new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.Resource, AttributeId = (int)ResourceType.Health, ModifierType = (int)ModifierType.Increment, Modifier = 10}
            };
                entry.SpriteId = (int)UI.ItemIconSpriteId.Relic;
                entry.PrefabId = (int)ApparelAssetId.NONE;
                entry.Value = ACCESSORY_COST;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }
            #endregion

            DBService.Get().UpdateEquipmentDB();
        }
    }
}



