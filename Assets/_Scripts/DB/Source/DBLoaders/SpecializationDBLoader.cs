using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class SpecializationDBLoader
    {
        public static void LoadDB()
        {
            // Adept
            {
                DB.DBSpecialization dbSpecialization = new DB.DBSpecialization();

                // Type
                dbSpecialization.SpecializationType = (int)SpecializationType.Adept;

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
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Value = 1},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Finese, Value = 1},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Magic, Value = 5},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Fortitude, Value = 1},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Attunement, Value = 5}
            };

                // talents
                dbSpecialization.TalentIds = new List<int>()
            {
                (int)TalentId.MagicIncrease,
            };

                // Actions
                dbSpecialization.ActionIds = new List<int>()
            {
                (int)ActionId.FireBall,
                (int)ActionId.ChainLightning,
                (int)ActionId.FlameStrike,
                (int)ActionId.EntanglingRoots,
                (int)ActionId.Sprout,
            };

                DBService.Get().WriteSpecialization(dbSpecialization);
            }

            // Archer
            {
                DB.DBSpecialization dbSpecialization = new DB.DBSpecialization();

                // Type
                dbSpecialization.SpecializationType = (int)SpecializationType.Archer;

                // Proficiencies
                dbSpecialization.Proficiencies = new List<int>()
            {
                (int)ProficiencyType.Fists,
                (int)ProficiencyType.Dagger,
                (int)ProficiencyType.Bow,
                (int)ProficiencyType.Accessorys,
                (int)ProficiencyType.Leather
            };

                // Level up modifiers
                dbSpecialization.LevelUpModifiers = new List<DB.DBAttribute>()
            {
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Value = 5},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Finese, Value = 2},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Magic, Value = 1},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Fortitude, Value = 5},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Attunement, Value = 1}
            };

                // talents
                dbSpecialization.TalentIds = new List<int>()
                {
                };

                // Listeners
                dbSpecialization.ResponseListenerIds = new List<int>()
                {
                };

                // Actions
                dbSpecialization.ActionIds = new List<int>()
                {
                    (int)ActionId.SummonBear
                };
                // none

                DBService.Get().WriteSpecialization(dbSpecialization);
            }

            // Bear
            {
                DB.DBSpecialization dbSpecialization = new DB.DBSpecialization();

                // Type
                dbSpecialization.SpecializationType = (int)SpecializationType.Bear;

                // Proficiencies
                dbSpecialization.Proficiencies = new List<int>()
                {
                    (int)ProficiencyType.Fists
                };

                // Level up modifiers
                dbSpecialization.LevelUpModifiers = new List<DB.DBAttribute>()
                {
                    new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Value = 4},
                    new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Finese, Value = 3},
                    new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Magic, Value = 2},
                    new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Fortitude, Value = 5},
                    new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Attunement, Value = 1}
                };

                // talents
                dbSpecialization.TalentIds = new List<int>()
                {
                    
                };

                // Listeners
                dbSpecialization.ResponseListenerIds = new List<int>()
                {
                    
                };

                // Actions
                dbSpecialization.ActionIds = new List<int>()
                {
                    
                };
                // none

                DBService.Get().WriteSpecialization(dbSpecialization);
            }

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
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Value = 4},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Finese, Value = 3},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Magic, Value = 2},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Fortitude, Value = 5},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Attunement, Value = 1}
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
                (int)ActionId.Defend,
                (int)ActionId.DoubleTime,
                (int)ActionId.ShieldBash,
                (int)ActionId.Swipe,
            };
                // none

                DBService.Get().WriteSpecialization(dbSpecialization);
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
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Value = 2},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Finese, Value = 1},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Magic, Value = 4},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Fortitude, Value = 1},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Attunement, Value = 5}
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
                (int)ActionId.Protection,
                (int)ActionId.Regen,
                (int)ActionId.Smite,
            };

                DBService.Get().WriteSpecialization(dbSpecialization);
            }

            // Paladin
            {
                DB.DBSpecialization dbSpecialization = new DB.DBSpecialization();

                // Type
                dbSpecialization.SpecializationType = (int)SpecializationType.Paladin;

                // Proficiencies
                dbSpecialization.Proficiencies = new List<int>()
            {
                (int)ProficiencyType.Hammer,
                (int)ProficiencyType.Sword,
                (int)ProficiencyType.Accessorys,
                (int)ProficiencyType.Sheild,
                (int)ProficiencyType.Chain,
                (int)ProficiencyType.Plate
            };

                // Level up modifiers
                dbSpecialization.LevelUpModifiers = new List<DB.DBAttribute>()
            {
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Might, Value = 5},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Finese, Value = 3},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.PrimaryStat, AttributeId = (int)PrimaryStat.Magic, Value = 4},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Fortitude, Value = 4},
                new DB.DBAttribute() { AttributeCategory = (int)AttributeCategory.SecondaryStat, AttributeId = (int)SecondaryStat.Attunement, Value = 4}
            };

                // talents
                dbSpecialization.TalentIds = new List<int>()
            {
                (int)TalentId.SpeedIncrease
                , (int)TalentId.MoveIncrease
                , (int)TalentId.MightIncrease
                //, (int)TalentId.HammerIncrease
                , (int)TalentId.MagicIncrease
                //, (int)TalentId.ArmorIncrease
            };

                // Listeners
                dbSpecialization.ResponseListenerIds = new List<int>()
            {
                (int)ActionResponseId.Avenger
            };

                // Actions
                dbSpecialization.ActionIds = new List<int>()
            {
                (int)ActionId.Smite
                ,(int)ActionId.Shackle
                ,(int)ActionId.Anvil
                //,(int)ActionId.HolyLight
                //,(int)ActionId.Raise
            };

                // Actions
                dbSpecialization.AuraIds = new List<int>()
            {
                (int)AuraType.RighteousGlory
            };


                DBService.Get().WriteSpecialization(dbSpecialization);
            }

            DBService.Get().UpdateSpecializationDB();
        }
    }
}


