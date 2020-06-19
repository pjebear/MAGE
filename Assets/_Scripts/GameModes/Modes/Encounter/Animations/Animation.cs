using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AnimationId
{
    INVALID = -1,

    BowDraw,
    SwordSwing,
    Block,
    Dodge,
    Parry,
    Hurt,
    Cast,
    ChargeUp,
    Faint,
    Revive,
   
    NUM
}

class RuntimeAnimation
{
    public float Duration;
    public float Elapsed;
    public string Name;
    public float SyncPoint;

    // Update is called once per frame
    void Update(float dt)
    {
        Elapsed += dt;
    }
}

