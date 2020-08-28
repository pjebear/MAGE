using System;
using System.Collections.Generic;

namespace MAGE.GameSystems.Actions
{
    enum AreaType
    {
        Circle,
        //Ring,
        Cone,
        //Line,
        Cross,
        Expanding,

        NUM
    }

    struct RangeInfo
    {
        public static RangeInfo Unit { get { return new RangeInfo(0, 0, 0, AreaType.Circle); } }
        public int MinRange;
        public int MaxRange;
        public int MaxElevationChange;
        public AreaType AreaType;

        public RangeInfo(int minRange, int maxRange, int maxElevationChange, AreaType areaType)
        {
            MinRange = minRange;
            MaxRange = maxRange;
            MaxElevationChange = maxElevationChange;
            AreaType = areaType;
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
    }

    class ActionChainInfo
    {
        public int NumChainTargets = -1;
        public float ChainEffectFalloff = 0;
    }
}

