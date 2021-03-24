using UnityEngine;
using System.Collections;

namespace MAGE.GameSystems.Stats
{
    enum AttributeCategory
    {
        PrimaryStat,
        SecondaryStat,
        TertiaryStat,
        Resource,
        Allignment,
        Status,
        NUM
    }

    enum PrimaryStat
    {
        INVALID = -1,
        Might,
        Finese,
        Magic,

        NUM
    }

    enum SecondaryStat
    {
        INVALID = -1,

        Fortitude,
        Attunement,

        NUM 
    }

    enum TertiaryStat
    {
        INVALID = -1,

        //MobilityStats
        Speed,
        Movement,
        Jump,
        MaxClockGuage,

        //Defensive stats
        Block,
        Parry,
        Dodge,

        PhysicalResistance,
        MagicalResistance,
        PhysicalCritSusceptibility, // negative values indicate reduced chance to be crit
        MagicalCritSusceptibility,

        //Offensive stats
        PhysicalCritChance,
        MagicalCritChance,
        PhysicalMultiplier, // 0.1f = 10 percent increase
        MagicalMultiplier,
        CastSpeed, // 0 = regular cast speed, 1 * (1 + castSpeed) ticks removed per charge cycle 
        EnduranceRecovery, // .10 = 10 percent of max endurance per turn

       NUM
    }

    enum AllignmentType
    {
        Unalligned,
        Light,
        Dark,
        Earth,
        Fire,
        Sky,
        Water,
        NUM,
    }

    enum ResourceType
    {
        INVALID = -1,

        Health,
        Mana,
        Endurance,
        Clock,

        NUM
    }

    enum ProficiencyType
    {
        INVALID = -1,

        // One hands
        Fists,
        Sword,
        Axe,
        Hammer,
        Dagger,
        Crossbow,

        // Two hands
        Staff,
        Bow,
        BastardSword,
        BattleAxe,
        Maul,

        // Sheilds
        Sheild,
        TowerShield,

        // Armor
        Cloth,
        Leather,
        Chain,
        Plate,

        // Multi
        OneHands,
        TwoHands,
        Armors,
        Sheilds,
        Accessorys,

        NUM
    }

    enum StatusType
    {
        ActionsAvailable,
        MovesAvailable,
        Rooted,
        Silenced,
        Disarmed,
        KO,
        KOCountDown,
        Interupt,

        NUM
    }

    enum ModifierType
    {
        Increment,
        Multiply,

        NUM
    }
}