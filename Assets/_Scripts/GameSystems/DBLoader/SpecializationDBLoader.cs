using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class SpecializationDBLoader
{
    public static void LoadDB()
    {
        // Footman
        {
            DB.DBSpecialization dbSpecialization = new DB.DBSpecialization();

            // Type
            dbSpecialization.SpecializationType = (int)SpecializationType.Footman;

            // Proficiencies
            dbSpecialization.Proficiencies = new List<int>()
            {
                (int)ProficiencyType.OneHands,
                (int)ProficiencyType.Accessorys,
                (int)ProficiencyType.Sheild,
                (int)ProficiencyType.Chain,
                (int)ProficiencyType.Leather
            };

            // Level up modifiers
            dbSpecialization.LevelUpModifiers = new List<DB.DBAttribute>()
            {
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Value = 4},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Finese, Value = 3},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Magic, Value = 2},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)SecondaryStat.Fortitude, Value = 5},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)SecondaryStat.Attunement, Value = 1}
            };

            // talents
            dbSpecialization.TalentIds = new List<int>()
            {
                (int)TalentId.BlockIncrease,
                (int)TalentId.MightyBlow
            };

            // Listeners
            dbSpecialization.ResponseListenerIds = new List<int>()
            {
                (int)ActionResponseId.BloodScent
            };

            // Actions
            dbSpecialization.ActionIds = new List<int>()
            {
                (int)ActionId.MightyBlow
            };
                // none

            DB.DBHelper.WriteSpecialization(dbSpecialization);
        }

        // Monk
        {
            DB.DBSpecialization dbSpecialization = new DB.DBSpecialization();

            // Type
            dbSpecialization.SpecializationType = (int)SpecializationType.Monk;

            // Proficiencies
            dbSpecialization.Proficiencies = new List<int>()
            {
                (int)ProficiencyType.Fists,
                (int)ProficiencyType.Staff,
                (int)ProficiencyType.Cloth,
                (int)ProficiencyType.Accessorys
            };

            // Level up modifiers
            dbSpecialization.LevelUpModifiers = new List<DB.DBAttribute>()
            {
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Might, Value = 2},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Finese, Value = 1},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)PrimaryStat.Magic, Value = 4},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)SecondaryStat.Fortitude, Value = 1},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.Stat, AttributeId = (int)SecondaryStat.Attunement, Value = 5}
            };

            // talents
            dbSpecialization.TalentIds = new List<int>()
            {
                (int)TalentId.HealIncrease,
                (int)TalentId.HealOnHurt
            };

            // Actions
            dbSpecialization.ActionIds = new List<int>()
            {
                (int)ActionId.Heal,
                (int)ActionId.Protection
            };

            DB.DBHelper.WriteSpecialization(dbSpecialization);
        }

        DB.DBHelper.UpdateSpecializationDB();
    }
}

