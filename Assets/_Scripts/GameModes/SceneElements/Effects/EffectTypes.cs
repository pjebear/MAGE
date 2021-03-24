using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    [System.Serializable]
    struct EffectDurationInfo
    {
        public float DurationSeconds;
        public float SyncPointSeconds;
    }
}
enum EffectType
{
    INVALID,

    AOE_Heal,
    Fire, 
    FlameStrike, 
    LightningBolt,
    Regen,

    NUM
}

class EffectInfo : ISynchronizable
{
    public SyncPoint Parent { get; set; }
    public int NumFrames { get; private set; }
    public int SyncedFrame { get; private set; }

    public EffectType EffectType;
    public Transform SpawnParent;

    public EffectInfo(EffectType effectType, Transform parent, int syncedFrame, int numFrames)
    {
        EffectType = effectType;
        NumFrames = numFrames;
        SyncedFrame = syncedFrame;
        SpawnParent = parent;
    }
}

