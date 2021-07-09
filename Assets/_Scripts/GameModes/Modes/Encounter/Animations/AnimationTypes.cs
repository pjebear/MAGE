
using MAGE.GameModes.SceneElements;
using System;
using System.Collections.Generic;

class AnimationConstants
{
    public static readonly int FRAMES_PER_SECOND = 30;
    public static readonly float SECONDS_PER_FRAME = 1 / (float)FRAMES_PER_SECOND;
    public static int FRAMES_IN_DURATION(float duration)
    {
        return (int)Math.Ceiling(duration * FRAMES_PER_SECOND);
    }
    public static readonly int DEBUG_PRINT_FRAME_INCREMENT = 3;

    public static readonly int SPELL_DURATION = FRAMES_IN_DURATION(1.7f);
    public static readonly int SPELL_SYNC_POINT = FRAMES_IN_DURATION(1.5f);

    public static readonly int RANGED_ABILITY_DURATION = FRAMES_IN_DURATION(2.5f);
    public static readonly int RANGED_INTERACTION_POINT = FRAMES_IN_DURATION(2.4f);

    public static readonly int MEELE_ABILITY_DURATION = FRAMES_IN_DURATION(1.5f) * 3;
    public static readonly int MEELE_INTERACTION_POINT = FRAMES_IN_DURATION(1.1f) * 3;

    public static readonly int REACTION_ABILITY_DURATION = FRAMES_IN_DURATION(1.2f) * 3;
    public static readonly int REACTION_INTERACTION_POINT = FRAMES_IN_DURATION(.7f) * 3;

    public static readonly int QUICKCAST_ABILITY_DURATION = FRAMES_IN_DURATION(.9f);
    public static readonly int QUICKCAST_INTERACTION_POINT = FRAMES_IN_DURATION(.7f);

    public static readonly int AUGMENT_ABILITY_DURATION = FRAMES_IN_DURATION(1.0f);
    public static readonly int AUGMENT_INTERACTION_POINT = FRAMES_IN_DURATION(.9f);

    //public static void FRAMES_IN_ANIMATION(AnimationId animationId, ref int numFrames, ref int syncFrame)
    //{
    //    switch (animationId)
    //    {
    //        case AnimationId.SwordAttack:
    //            numFrames = MEELE_ABILITY_DURATION;
    //            syncFrame = MEELE_INTERACTION_POINT;
    //            break;

    //        case AnimationId.Block:
    //            numFrames = REACTION_ABILITY_DURATION;
    //            syncFrame = REACTION_INTERACTION_POINT;
    //            break;

    //        case AnimationId.Dodge:
    //            numFrames = REACTION_ABILITY_DURATION;
    //            syncFrame = REACTION_INTERACTION_POINT;
    //            break;

    //        case AnimationId.Parry:
    //            numFrames = REACTION_ABILITY_DURATION;
    //            syncFrame = REACTION_INTERACTION_POINT;
    //            break;

    //        case AnimationId.Hurt:
    //            numFrames = REACTION_ABILITY_DURATION;
    //            syncFrame = REACTION_INTERACTION_POINT;
    //            break;

    //        case AnimationId.BowDraw:
    //            numFrames = RANGED_ABILITY_DURATION;
    //            syncFrame = RANGED_INTERACTION_POINT;
    //            break;

    //        case AnimationId.QuickCast:
    //            numFrames = QUICKCAST_ABILITY_DURATION;
    //            syncFrame = QUICKCAST_INTERACTION_POINT;
    //            break;
    //        case AnimationId.INVALID:
    //            numFrames = 0;
    //            syncFrame = 0;
    //            break;
    //        default:
    //            throw new Exception("PETER COME ON");

    //    }
    //}
}

class AnimationInfo : ISynchronizable
{
    public SyncPoint Parent { get; set; }
    public int NumFrames { get; private set; }
    public int SyncedFrame { get; private set; }
    public string TriggerName;
    public SFXId SFXId = SFXId.INVALID;
    public HumanoidActorConstants.HeldApparelState ApparelStateInAnimation = HumanoidActorConstants.HeldApparelState.NUM;

    public AnimationInfo(string triggerName, int numFrames, int syncedFrame, SFXId sFXId, HumanoidActorConstants.HeldApparelState apparelStateInAnimation = HumanoidActorConstants.HeldApparelState.NUM)
    {
        NumFrames = numFrames;
        SyncedFrame = syncedFrame;
        TriggerName = triggerName;
        SFXId = sFXId;
        ApparelStateInAnimation = apparelStateInAnimation;
    }
}






