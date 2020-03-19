using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AnimationDirector : MonoBehaviour
{
    public DebugAnimationRig AnimationRigPrefab;

    private Dictionary<ActorController, DebugAnimationRig> mActorToAnimationRigLookup;

    void Awake()
    {
        mActorToAnimationRigLookup = new Dictionary<ActorController, DebugAnimationRig>();
    }

    public void RegisterActor(ActorController actor)
    {
        GameObject canvas = GameObject.Find("Canvas");

        DebugAnimationRig rig = Instantiate(AnimationRigPrefab, canvas.transform);
        rig.ActorController = actor;
        mActorToAnimationRigLookup.Add(actor, rig);
    }

    public void AnimateActor(ActorController actor, AnimationPlaceholder animation)
    {
        mActorToAnimationRigLookup[actor].DisplayAnimation(animation);
        actor.GetComponent<Animator>().SetTrigger(animation.TriggerName);
    }
}
