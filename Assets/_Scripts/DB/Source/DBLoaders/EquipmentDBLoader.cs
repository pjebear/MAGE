﻿using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class EquipmentDBLoader
    {
        private static DB.DBRangeInfo MELEE_RANGE = new DB.DBRangeInfo() { Min = 1, Max = 1, Elevation = 1, Type = (int)AreaType.Circle };
        private static DB.DBRangeInfo BOW_RANGE = new DB.DBRangeInfo() { Min = 3, Max = 6, Elevation = 7, Type = (int)AreaType.Expanding };

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
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)TertiaryStat.PhysicalResistance, ModifierType = (int)ModifierType.Increment, Modifier = .15f}
            };
                entry.SpriteId = (int)UI.ItemIconSpriteId.Cloth;
                entry.PrefabId = (int)AppearancePrefabId.Cloth_0;

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
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)TertiaryStat.PhysicalResistance, ModifierType = (int)ModifierType.Increment, Modifier = .3f}
            };
                entry.SpriteId = (int)UI.ItemIconSpriteId.Leather;
                entry.PrefabId = (int)AppearancePrefabId.Leather_0;

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
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)TertiaryStat.PhysicalResistance, ModifierType = (int)ModifierType.Increment, Modifier = .4f}
            };
                entry.SpriteId = (int)UI.ItemIconSpriteId.Chain;
                entry.PrefabId = (int)AppearancePrefabId.Chain_0;

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
                    new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)TertiaryStat.PhysicalResistance, ModifierType = (int)ModifierType.Increment, Modifier = .6f}
                };
                entry.SpriteId = (int)UI.ItemIconSpriteId.Plate;
                entry.PrefabId = (int)AppearancePrefabId.Plate_0;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }
            #endregion // Armor

            #region OneHands
            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Fists
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Fists_0;
                entry.Category = (int)EquippableCategory.OneHandWeapon;
                entry.Type = (int)OneHandWeaponType.Fist;
                entry.BlockChance = 0;
                entry.ParryChance = 0;
                entry.AnimationId = (int)AnimationId.SwordSwing;
                entry.Range = MELEE_RANGE;


                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = .75f}
            };

                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty

                };
                entry.SpriteId = (int)UI.ItemIconSpriteId.INVALID;
                entry.PrefabId = (int)AppearancePrefabId.prefab_none;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Axe
                DB.DBEquipment entry = new DB.DBEquipment();

                entry.Id = (int)EquippableId.Axe_0;
                entry.Name = EquippableId.Axe_0.ToString();
                entry.Category = (int)EquippableCategory.OneHandWeapon;
                entry.Type = (int)OneHandWeaponType.Axe;
                entry.BlockChance = 0;
                entry.ParryChance = 15;
                entry.AnimationId = (int)AnimationId.SwordSwing;
                entry.Range = MELEE_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = .75f}
                , new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Finese, Scalar = .25f}
            };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty
                };

                entry.SpriteId = (int)UI.ItemIconSpriteId.Axe;
                entry.PrefabId = (int)AppearancePrefabId.Axe_0;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Mace
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Mace_0;
                entry.Category = (int)EquippableCategory.OneHandWeapon;
                entry.Type = (int)OneHandWeaponType.Mace;
                entry.BlockChance = 0;
                entry.ParryChance = 10;
                entry.AnimationId = (int)AnimationId.SwordSwing;
                entry.Range = MELEE_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = 1}
            };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty
                };

                entry.SpriteId = (int)UI.ItemIconSpriteId.Mace;
                entry.PrefabId = (int)AppearancePrefabId.Mace_0;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Sword
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Sword_0;
                entry.Category = (int)EquippableCategory.OneHandWeapon;
                entry.Type = (int)OneHandWeaponType.Sword;
                entry.BlockChance = 0;
                entry.ParryChance = 20;
                entry.AnimationId = (int)AnimationId.SwordSwing;
                entry.Range = MELEE_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = .5f}
                , new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Finese, Scalar = .5f}
            };
                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty
                };

                entry.SpriteId = (int)UI.ItemIconSpriteId.Sword;
                entry.PrefabId = (int)AppearancePrefabId.Sword_0;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }
            #endregion // one hands

            #region TwoHands
            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Bow
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Bow_0;
                entry.Category = (int)EquippableCategory.TwoHandWeapon;
                entry.Type = (int)TwoHandWeaponType.Bow;
                entry.BlockChance = 0;
                entry.ParryChance = 0;
                entry.AnimationId = (int)AnimationId.BowDraw;
                entry.ProjectileId = (int)ProjectileId.Arrow;
                entry.Range = BOW_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = .5f},
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Finese, Scalar = .5f}
            };

                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
                {
                    // empty
                };

                entry.SpriteId = (int)UI.ItemIconSpriteId.Bow;
                entry.PrefabId = (int)AppearancePrefabId.Bow_0;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }
            // ---------------------------------------------------------------------------------------------------------------------------------------
            { // Staff
                DB.DBEquipment entry = new DB.DBEquipment();
                entry.Id = (int)EquippableId.Staff_0;
                entry.Category = (int)EquippableCategory.TwoHandWeapon;
                entry.Type = (int)TwoHandWeaponType.Staff;
                entry.BlockChance = 0;
                entry.ParryChance = 0;
                entry.AnimationId = (int)AnimationId.SwordSwing;
                entry.Range = MELEE_RANGE;

                entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = .5f}
            };

                entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Magic, ModifierType = (int)ModifierType.Multiply, Modifier = .1f}
            };

                entry.SpriteId = (int)UI.ItemIconSpriteId.Staff;
                entry.PrefabId = (int)AppearancePrefabId.Staff_0;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }
            #endregion // TwoHands

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
                entry.PrefabId = (int)AppearancePrefabId.Shield_0;

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
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, ModifierType = (int)ModifierType.Increment, Modifier = 15}
            };
                entry.SpriteId = (int)UI.ItemIconSpriteId.Relic;
                entry.PrefabId = (int)AppearancePrefabId.prefab_none;

                DBService.Get().WriteEquipment(entry.Id, entry);
            }
            #endregion

            DBService.Get().UpdateEquipmentDB();
        }
    }
}


