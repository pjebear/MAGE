﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    static class ActionConstants
    {
        public static readonly int INSTANT_CAST_SPEED = -1;
        public static readonly int FAST_CAST_SPEED = 8;
        public static readonly int NORMAL_CAST_SPEED = 16;
        public static readonly int SLOW_CAST_SPEED = 24;
    }

    enum TargetingType
    {
        Any,
        Allies,
        Enemies,
        Empty,

        NUM
    }

    enum AreaType
    {
        Point,
        Circle,
        Chain,
        //Ring,
        Cone,
        //Line,
        Cross,
        Expanding,
        MultiLine,

        NUM
    }

    enum CastSpeed
    {
        INVALID = -1,

        Instant,
        Slow,
        Normal,
        Fast,

        NUM
    }

    enum ActionMedium
    {
        Physical,
        Magical,
        Pure,

        NUM
    }

    enum ActionRange
    {
        Meele,
        Projectile,
        AOE,

        NUM
    }

    enum ActionSource
    {
        Weapon,
        Cast,

        NUM
    }

    enum WeaponType
    {
        Sword,
        Bow,

        NUM
    }

    enum ActionResultType
    {
        Hit,
        Parry,
        Block,
        Dodge,
        Resist,

        NUM
    }

    enum RoleType
    {
        Targeted,
        Interacting,

        NUM
    }

    enum ActionType
    {
        Weapon,
        Cast,
        NUM
    }

    enum InteractionResultType
    {
        None,

        Hit,
        Partial,
        Miss,

        Dodge,
        Parry,
        Block,
        Resist,

        NUM
    }

    enum RelativeOrientation
    {
        Behind,
        Left,
        Right,
        Front,

        NUM
    }

    enum ActionId
    {
        INVALID = -1,

        MightyBlow,

        // Assasin
        Backstab,
        PoisonStrike,
        //Garrote,
        //Eviscerate,

        // Footman
        Defend,
        DoubleTime,
        ShieldBash,
        Swipe,

        //Paladin
        Smite,
        Shackle,
        Anvil,
        HolyLight,
        Raise,

        // Ranger
        SummonBear,

        // spells
        EntanglingRoots,
        FireBall,
        FlameStrike,
        ChainLightning,
        Regen,
        Sprout,
        Heal,
        SpotHeal,
        Protection,

        MeleeAttack,
        RangedAttack,

        // Followup
        Riptose,
        ExplodeOnDeath,

        NUM
    }

    enum ActionResponseId
    {
        Riptose,
        HealOnHurtListener,
        BloodScent,
        Avenger,

        NUM
    }
}

