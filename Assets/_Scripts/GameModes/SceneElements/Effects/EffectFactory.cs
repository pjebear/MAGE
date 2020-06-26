using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class EffectFactory
{
    public static EffectInfo GetEffectPlaceholder(EffectType effectType, Transform parent)
    {
        int numFrames = 120;
        int syncFrame = 60;
        return new EffectInfo(effectType, parent, syncFrame, numFrames);
    }
}

