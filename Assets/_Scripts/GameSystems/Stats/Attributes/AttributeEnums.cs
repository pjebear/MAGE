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
        Actions,
        Movement,
        MaxClockGuage,

        //Defensive stats
        Block,
        Parry,
        Dodge,
        AvoidanceMultiplier,  // 0-1

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
        ResourceRecovery, // .10 = 10 percent of max resource per turn

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
        Actions,
        MovementRange,

        NUM
    }

    enum ProficiencyType
    {
        INVALID = -1,

        // Held
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
        Ranged,

        // Extras
        DualWeild,

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