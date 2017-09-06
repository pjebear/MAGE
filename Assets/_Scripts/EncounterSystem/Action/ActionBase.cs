using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace EncounterSystem.Action
{
    using Character;
    using Animation;
    using Common.ActionTypes;
    using MapTypes;

    public abstract class ActionBase : MonoBehaviour
    {
        public ActionInfo ActionInfo { get; private set; }
        protected _GetActionPriority mActionPriority;
        public MapInteractionInfo MapInteractionInfo { get; private set; }
        public AnimationInfo AnimationInfo { get; private set; }
        protected ActionAnimationSystem mAnimationSystem;
            
        public bool FinishedExecution { get; protected set; }

        // Internal call made by child class to define base functionality
        public virtual void Initialize(ActionInfo actionInfo, MapInteractionInfo mapInterationInfo, AnimationInfo animationInfo, ActionAnimationSystem animationSystem, _GetActionPriority actionPriority)
        {
            ActionInfo = actionInfo;
            MapInteractionInfo = mapInterationInfo;
            AnimationInfo = animationInfo;
            mAnimationSystem = animationSystem;
            mActionPriority = actionPriority;
        }

        public virtual float GetActionPriority(CharacterManager caster, List<CharacterManager> actionTargets)
        {
            return mActionPriority(caster, actionTargets);
        }

        public abstract IEnumerator ExececuteAction(CharacterManager actionOwner, List<MapTile> targets, System.Action<ActionInteractionResult> _RecordActionResults);
    }
}


