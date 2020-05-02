using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AnimationDirector : MonoBehaviour
{
    public DebugAnimationRig AnimationRigPrefab;

    private Dictionary<EncounterActorController, DebugAnimationRig> mActorToAnimationRigLookup;
    private HashSet<EncounterActorController> mRotatingActors;

    void Awake()
    {
        mActorToAnimationRigLookup = new Dictionary<EncounterActorController, DebugAnimationRig>();
        mRotatingActors = new HashSet<EncounterActorController>();
    }

    public void RegisterActor(EncounterActorController actor)
    {
        GameObject canvas = GameObject.Find("Canvas");

        DebugAnimationRig rig = Instantiate(AnimationRigPrefab, canvas.transform);
        rig.ActorController = actor;
        mActorToAnimationRigLookup.Add(actor, rig);
    }

    public void UnRegisterActor(EncounterActorController actor)
    {
        if (mActorToAnimationRigLookup.ContainsKey(actor))
        {
            Destroy(mActorToAnimationRigLookup[actor].gameObject);
            mActorToAnimationRigLookup.Remove(actor);
        }
    }

    public void AnimateActor(EncounterActorController actor, AnimationPlaceholder animation)
    {
        mActorToAnimationRigLookup[actor].DisplayAnimation(animation);
        actor.GetComponent<Animator>().SetTrigger(animation.TriggerName);
    }

    public void RotateActorTowards(EncounterActorController actor, Transform target, float rotationDuration)
    {
        mRotatingActors.Add(actor);
        StartCoroutine(RotateActor(actor, target, rotationDuration));
    }

    private IEnumerator RotateActor(EncounterActorController actor, Transform target, float rotationDuration)
    {
        mRotatingActors.Add(actor);
        float dur = 0;

        Vector3 currentForward = actor.transform.forward;
        Vector3 newForward = target.transform.position - actor.transform.position;

        while (dur < rotationDuration)
        {
            dur += Time.deltaTime;
            if (dur > rotationDuration) dur = rotationDuration;

            actor.transform.forward = Vector3.Slerp(currentForward, newForward, dur / rotationDuration);

            yield return new WaitForEndOfFrame();
        }
        mRotatingActors.Remove(actor);
    }
}
