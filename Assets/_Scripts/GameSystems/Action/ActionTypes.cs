using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;

namespace MAGE.GameSystems.Actions
{
    [System.Serializable]
    struct RangeInfo
    {
        public static RangeInfo Unit { get { return new RangeInfo(0, 0, 0, AreaType.Circle, TargetingType.Any); } }
        public static float MELEE_RANGE = 2.5f;
        public float MinRange;
        public float MaxRange;
        public float MaxElevationChange;
        public AreaType AreaType;
        public TargetingType TargetingType;

        public RangeInfo(float minRange, float maxRange, float maxElevationChange, AreaType areaType, TargetingType targetingType)
        {
            MinRange = minRange;
            MaxRange = maxRange;
            MaxElevationChange = maxElevationChange;
            AreaType = areaType;
            TargetingType = targetingType;
        }
    }

    class ActionProjectileInfo
    {
        public ProjectileId ProjectileId = ProjectileId.INVALID;
    }

    class ActionAnimationInfo
    {
        public AnimationId AnimationId = AnimationId.INVALID;
        public AnimationSide AnimationSide = AnimationSide.None;
    }

    class ActionEffectInfo
    {
        public EffectType EffectId = EffectType.INVALID;
        public SFXId SFXId = SFXId.INVALID;
    }

    class ActionChainInfo
    {
        public int NumChainTargets = -1;
        public float ChainEffectFalloff = 0;
    }

    public enum SummonType
    {
        INVALID = -1,

        Bear,
        ScorchedEarth,
        Tree,

        NUM
    }

    class ActionSummonInfo
    {
        public SummonType SummonType;
        public int SummonCount = 1;
        public int MaxSummonCount = -1;
    }

    class EquipmentRequirement
    {
        public ProficiencyType Requirement = ProficiencyType.INVALID;
        public Equipment.Slot SlotRequirement = Equipment.Slot.INVALID;
    }

    class PositionalRequirement
    {
        public RelativeOrientation Requirement = RelativeOrientation.NUM;
    }
}

