using System;
using System.Collections.Generic;

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