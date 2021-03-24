using MAGE.GameModes.SceneElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class AnimationDirector : MonoBehaviour
    {
        private Dictionary<CharacterActorController, DebugAnimationRig> mActorToAnimationRigLookup;
        private HashSet<CharacterActorController> mRotatingActors;

        void Awake()
        {
            mActorToAnimationRigLookup = new Dictionary<CharacterActorController, DebugAnimationRig>();
            mRotatingActors = new HashSet<CharacterActorController>();
        }

        public void RegisterActor(CharacterActorController actor)
        {
            GameObject canvas = GameObject.Find("Canvas");

            DebugAnimationRig rig = Instantiate(EncounterPrefabLoader.LoadAnimationRig(), canvas.transform);
            rig.ActorController = actor;
            mActorToAnimationRigLookup.Add(actor, rig);
        }

        public void UnRegisterActor(CharacterActorController actor)
        {
            if (mActorToAnimationRigLookup.ContainsKey(actor))
            {
                Destroy(mActorToAnimationRigLookup[actor].gameObject);
                mActorToAnimationRigLookup.Remove(actor);
            }
        }

        public void AnimateActor(CharacterActorController actor, AnimationInfo animation)
        {
            mActorToAnimationRigLookup[actor].DisplayAnimation(animation);
            actor.GetComponent<ActorAnimator>().Trigger(animation.TriggerName);
        }

        public void RotateActorTowards(CharacterActorController actor, Transform target, float rotationDuration)
        {

            EncounterFlowControl_Deprecated.MovementDirector.RotateActor(actor.transform, target, null);

            //mRotatingActors.Add(actor);
            //StartCoroutine(RotateActor(actor, target, rotationDuration));
        }

        private IEnumerator RotateActor(CharacterActorController actor, Transform target, float rotationDuration)
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

}
