using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;

namespace MAGE.GameSystems.Actions
{
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

    [System.Serializable]
    struct RangeInfo
    {
        public static RangeInfo Unit { get { return new RangeInfo(0, 0, 0, AreaType.Circle, TargetingType.Any); } }
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
}

