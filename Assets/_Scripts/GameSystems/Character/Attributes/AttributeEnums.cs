using UnityEngine;
using System.Collections;


enum AttributeCategory
{
    Stat,
    Resource,
    Allignment,
    NUM
}

enum PrimaryStat
{
    Might,
    Finese,
    Magic,
    NUM = Magic - Might + 1
}

enum SecondaryStat
{
    Fortitude = PrimaryStat.Magic + 1,
    Attunement,
    NUM = Attunement - Fortitude + 1
}

enum TertiaryStat
{
    //MobilityStats

    Speed = SecondaryStat.Attunement + 1,
    Movement,
    Jump,

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

    NUM = EnduranceRecovery - Speed + 1
}

enum CharacterStat
{
    NUM = TertiaryStat.EnduranceRecovery + 1
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


