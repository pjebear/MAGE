using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

enum EffectType
{
    Heal,
    Fire, 

    NUM
}

class EffectPlaceholder : ISynchronizable
{
    public SyncPoint Parent { get; set; }
    public int NumFrames { get; private set; }
    public int SyncedFrame { get; private set; }

    public EffectType EffectType;
    public Transform SpawnParent;

    public EffectPlaceholder(EffectType effectType, Transform parent, int syncedFrame, int numFrames)
    {
        EffectType = effectType;
        NumFrames = numFrames;
        SyncedFrame = syncedFrame;
        SpawnParent = parent;
    }
}

