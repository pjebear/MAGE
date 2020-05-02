using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class EquipmentDBLoader
{
    public static void LoadDB()
    {
        // ---------------------------------------------------------------------------------------------------------------------------------------
        { // Axe
            DB.DBEquipment entry = new DB.DBEquipment();
            entry.Id = (int)EquippableId.Mace_0;
            entry.Category = (int)EquippableCategory.OneHandWeapon;
            entry.Type = (int)OneHandWeaponType.Axe;
            entry.BlockChance = 0;
            entry.ParryChance = 15;
            entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = .75f}
                , new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Finese, Scalar = .25f}
            };
            entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                // empty
            };
            entry.AppearanceIds = new List<int>
            {
                (int)AppearancePrefabId.Axe_0
            };

            DB.DBHelper.WriteEquipment(entry.Id, entry);
        }

        // ---------------------------------------------------------------------------------------------------------------------------------------
        { // Chain Armor
            DB.DBEquipment entry = new DB.DBEquipment();
            entry.Id = (int)EquippableId.ChainArmor_0;
            entry.Category = (int)EquippableCategory.Armor;
            entry.Type = (int)ArmorType.Chain;
            entry.BlockChance = 0;
            entry.ParryChance = 0;
            entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                // empty
            };
            entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)TertiaryStat.PhysicalResistance, ModifierType = (int)ModifierType.Increment, Modifier = .4f}
            };
            entry.AppearanceIds = new List<int>
            {
                (int)AppearancePrefabId.Chain_0
            };

            DB.DBHelper.WriteEquipment(entry.Id, entry);
        }

        // ---------------------------------------------------------------------------------------------------------------------------------------
        { // Fists
            DB.DBEquipment entry = new DB.DBEquipment();
            entry.Id = (int)EquippableId.Fists_0;
            entry.Category = (int)EquippableCategory.OneHandWeapon;
            entry.Type = (int)OneHandWeaponType.Fist;
            entry.BlockChance = 0;
            entry.ParryChance = 0;

            entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = .75f}
            };
            entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                // empty

            };
            entry.AppearanceIds = new List<int>
            {
                // empty
            };

            DB.DBHelper.WriteEquipment(entry.Id, entry);
        }

        { // Axe
            DB.DBEquipment entry = new DB.DBEquipment();
            entry.Id = (int)EquippableId.Mace_0;
            entry.Category = (int)EquippableCategory.OneHandWeapon;
            entry.Type = (int)OneHandWeaponType.Axe;
            entry.BlockChance = 0;
            entry.ParryChance = 15;
            entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = .75f}
                , new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Finese, Scalar = .25f}
            };
            entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                // empty
            };
            entry.AppearanceIds = new List<int>
            {
                (int)AppearancePrefabId.Axe_0
            };

            DB.DBHelper.WriteEquipment(entry.Id, entry);
        }

        // ---------------------------------------------------------------------------------------------------------------------------------------
        { // Leather Armor
            DB.DBEquipment entry = new DB.DBEquipment();
            entry.Id = (int)EquippableId.LeatherArmor_0;
            entry.Category = (int)EquippableCategory.Armor;
            entry.Type = (int)ArmorType.Leather;
            entry.BlockChance = 0;
            entry.ParryChance = 0;
            entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                // empty
            };
            entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)TertiaryStat.PhysicalResistance, ModifierType = (int)ModifierType.Increment, Modifier = .3f}
            };
            entry.AppearanceIds = new List<int>
            {
                (int)AppearancePrefabId.Leather_0
            };

            DB.DBHelper.WriteEquipment(entry.Id, entry);
        }

        // ---------------------------------------------------------------------------------------------------------------------------------------
        { // Mace
            DB.DBEquipment entry = new DB.DBEquipment();
            entry.Id = (int)EquippableId.Mace_0;
            entry.Category = (int)EquippableCategory.OneHandWeapon;
            entry.Type = (int)OneHandWeaponType.Mace;
            entry.BlockChance = 0;
            entry.ParryChance = 10;
            entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = 1}
            };
            entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                // empty
            };
            entry.AppearanceIds = new List<int>
            {
                (int)AppearancePrefabId.Mace_0
            };

            DB.DBHelper.WriteEquipment(entry.Id, entry);
        }

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
            entry.AppearanceIds = new List<int>
            {
                (int)AppearancePrefabId.Shield_0
            };

            DB.DBHelper.WriteEquipment(entry.Id, entry);
        }

        // ---------------------------------------------------------------------------------------------------------------------------------------
        { // Sword
            DB.DBEquipment entry = new DB.DBEquipment();
            entry.Id = (int)EquippableId.Sword_0;
            entry.Category = (int)EquippableCategory.OneHandWeapon;
            entry.Type = (int)OneHandWeaponType.Sword;
            entry.BlockChance = 0;
            entry.ParryChance = 20;
            entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = .5f}
                , new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Finese, Scalar = .5f}
            };
            entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                // empty
            };
            entry.AppearanceIds = new List<int>
            {
                (int)AppearancePrefabId.Sword_0
            };

            DB.DBHelper.WriteEquipment(entry.Id, entry);
        }

        // ---------------------------------------------------------------------------------------------------------------------------------------
        { // Staff
            DB.DBEquipment entry = new DB.DBEquipment();
            entry.Id = (int)EquippableId.Staff_0;
            entry.Category = (int)EquippableCategory.TwoHandWeapon;
            entry.Type = (int)TwoHandWeaponType.Staff;
            entry.BlockChance = 0;
            entry.ParryChance = 0;
            entry.EffectivenessScalars = new List<DB.DBAttributeScalar>()
            {
                new DB.DBAttributeScalar() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Scalar = .5f}
            };

            entry.EquipBonuses = new List<DB.DBAttributeModifier>()
            {
                new DB.DBAttributeModifier(){ AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Magic, ModifierType = (int)ModifierType.Multiply, Modifier = .1f}
            };

            entry.AppearanceIds = new List<int>
            {
                (int)AppearancePrefabId.Staff_0
            };

            DB.DBHelper.WriteEquipment(entry.Id, entry);
        }

        DB.DBHelper.UpdateEquipmentDB();
    }
}

