﻿using UnityEngine;
using System.Collections;


enum AttributeCategory
{
    Stat,
    Resource,
    Allignment,
    Proficiency,
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
    FrontalBlock,
    PeriferalBlock,
    FrontalParry,
    PeriferalParry,
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
    Sword,
    Axe,
    Hammer,
    Dagger,

    // Two hands
    Staff,

    // Sheilds
    Sheild,

    // Armor
    Cloth,
    Leather,
    Chain,
    Plate,

    // Multi
    OneHands,

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


