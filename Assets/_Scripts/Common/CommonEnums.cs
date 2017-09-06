using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    //------------------------------------------------------------
    namespace AttributeEnums
    {
        public enum AttributeType
        {
            Stat,
            Resource,
            Status,
            Allignment,
            NUM
        }

        public enum ModifierType
        {
            Additive,
            Multiplicative
        }

        //------------------------------------------------------------
        #region Stats
        // IF STAT NUMBER CHANGES, UPDATE CHARACTER HARDCODING
        public enum PrimaryStat
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
            PhyscialCritSusceptibility, // negative values indicate reduced chance to be crit
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

        enum CharacterStats
        {
            NUM = TertiaryStat.EnduranceRecovery + 1
        }

        public enum AllignmentType
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

        public enum Resource
        {
            INVALID = -1,
            Health,
            Mana,
            Endurance,
            MaxHealth,
            MaxMana,
            MaxEndurance,
            NUM
        }
        #endregion

        public enum StatusType
        {
            Rooted,
            Silenced,
            Disarmed,
            KO,
            KOCounter,
            Interupt,

            NUM
        }
    }

    //------------------------------------------------------------
    namespace ActionEnums
    {
        public enum ActionTargetType
        {
            Tile,
            Character
        }

        public enum ActionRootEffect
        {
            Harmful,
            Beneficial
        }

        public enum ActionBaseType
        {
            Physical,
            Magic,
            Pure
        }

        public enum ActionEffectType
        {
            Meele,
            Projectile,
            AOE
        }

        public enum PhysicalActionType
        {
            Pierce,
            Slash,
            Blunt
        }

        public enum InteractionResult
        {
            Hit,
            Crit,
            Miss
        }

        public enum AvoidanceResult
        {
            Invalid = -1,
            Block,
            Dodge,
            Parry,
            Miss,
            PartialResist,
            Resist,
            Absorb
        }

        public enum ActionOrientation
        {
            Frontal,
            Peripheral,
            Rear,
            NONE
        }

        public enum ActionIndex
        {
            INVALID = -1,

            MELEE_WEAPON,
            RANGED_WEAPON,
            SPELL_WEAPON,
            FIRE,
            FLAME_THROWER,
            FIRE_BLAST,
            HEAL,
            CLEANSE,
            REVIVE,
            PROTECT,
            SHELL,
            OPPORTUNITY_STRIKE,
            DEFEND,
            SHIELD_BASH,
            SHIELDWALL_ADVANCE,
            BERSERKER_BATTLECRY,
            BERSERKER_CLEAVE,
            BERSERKER_EXECUTE,
            NUM
        };

        public enum ActionContainerCategory
        {
            Attack,
            Primary,
            Secondary
        }
    }

    //------------------------------------------------------------
    namespace CharacterEnums
    {
        public enum CharacterGender
        {
            Male,
            Female
        }

        public enum UnitGroup
        {
            Player,
            AI,
            NUM
        }
    }
    //------------------------------------------------------------
    namespace EncounterEnums
    {
        enum EncounterFlowState
        {
            UnitPlacement = -3,
            OpeningCinematic,
            Setup,
            EncounterBegun,
            EncounterClock,
            Status,
            ActionCharge,
            ActionResolution,
            TurnCharge,
            TurnResolution,
            EncounterFinished,
            NUM
        }

        enum EncounterSuccessState
        {
            Invalid = -1,
            Loss,
            InProgess,
            Win
        }

        enum EncounterScenarioId
        {
            Debug = -2,
            Random = -1,

            Main_Opening,

            NUM
        }
    }

    //------------------------------------------------------------
    namespace CinematicEnums
    {
        public enum CinematicComponentType
        {
            Dialogue,
            Movement,
            Animation,
            UnitReveal
        }
    }
}

