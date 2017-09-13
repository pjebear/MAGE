using Common.EncounterEnums;
using EncounterSystem.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;
using WorldSystem.Common;
using WorldSystem.Interface;
using WorldSystem.Managers.WorldTransitions;

namespace WorldSystem.Managers
{
    class TransitionManager
    {
        private WorldTransitionBase mPreviousTransition;
        private WorldTransitionBase mNextTransition;

        public TransitionManager()
        {
            mPreviousTransition = null;
            mNextTransition = null;
        }

        public void Initialize(WorldTransitionBase defaultTransition)
        {
            mPreviousTransition = defaultTransition;
        }

        public void SetNextTransition(WorldTransitionBase transition)
        {
            UnityEngine.Debug.LogFormat("Setting new {0} transition. NextTransition was previously {1}", transition.TransitionType.ToString(), mNextTransition == null ? "null" : mNextTransition.ToString());
            mNextTransition = transition;
        }

        public void Transition()
        {
            UnityEngine.Debug.Assert(mNextTransition != null, "Attempting to transition to a null state");
            mNextTransition.Transition();
            if (mNextTransition.IsReverseTraversable)
            {
                mPreviousTransition = mNextTransition;
            }
            mNextTransition = null;
        }

        public void TransitionFinished()
        {
            if (mNextTransition != null)
            {
                Transition();
            }
            else
            {
                UnityEngine.Debug.Assert(mPreviousTransition != null, "No Transitions to make");
                mPreviousTransition.Transition();
            }
        }
    }
    
    namespace WorldTransitions
    {
        abstract class WorldTransitionBase
        {
            public WorldTransitionType TransitionType { get; private set; }
            public bool IsReverseTraversable { get; private set; }

            protected WorldTransitionBase(WorldTransitionType type, bool isReverseTraversable)
            {
                TransitionType = type;
                IsReverseTraversable = isReverseTraversable;
            }

            public abstract void Transition();
        }

        class EncounterTransition : WorldTransitionBase
        {
            public EncounterScenarioId ScenarioId { get; private set; }
            public EncounterTransition(EncounterScenarioId toLoad)
                : base(WorldTransitionType.Encounter, false)
            {
                ScenarioId = toLoad;
            }

            public override void Transition()
            {
                UnityEngine.Debug.Log("Transitioning to Encounter");
                EncounterSystemFacade.Instance().BeginEncounter(ScenarioId, MapLocationId.INVALID, WorldSystemFacade.Instance().GetPlayerRoster());
                SceneManager.LoadScene("EncounterScreen");
            }
        }

        class MapTransition : WorldTransitionBase
        {
            public MapTransition()
                : base(WorldTransitionType.Map, true)
            {

            }

            public override void Transition()
            {
                UnityEngine.Debug.Log("Transitioning to Map");
                SceneManager.LoadScene("WorldScreen");
            }
        }

        class RosterScreenTransition : WorldTransitionBase
        {
            public RosterScreenTransition()
                : base (WorldTransitionType.RosterScreen, false)
            {

            }

            public override void Transition()
            {
                UnityEngine.Debug.Log("Transitioning to RosterScreen");
                SceneManager.LoadScene("RosterScreen");
            }
        }

        class AreaTransition : WorldTransitionBase
        {
            // area to transition to
            public AreaTransition()
                : base(WorldTransitionType.Area, true)
            {
                
            }

            public override void Transition()
            {
                UnityEngine.Debug.Log("Transitioning to Area");
                UnityEngine.Debug.Assert(false, "Area Transition not setup");
            }
        }

        class CinematicTransition : WorldTransitionBase
        {
            public CinematicTransition()
                : base(WorldTransitionType.Cinematic, false)
            {

            }
            public override void Transition()
            {
                UnityEngine.Debug.Log("Transitioning to CinematicEvent");
                UnityEngine.Debug.Assert(false, "CinematicEvent Transition not setup");
            }
        }

        class TitleScreenTransition : WorldTransitionBase
        {
            public TitleScreenTransition()
                : base (WorldTransitionType.TitleScreen, false)
            {

            }

            public override void Transition()
            {
                UnityEngine.Debug.Log("Transitioning to TitleScreen");
                SceneManager.LoadScene("TitleScreen");
            }
        }
    }
   
}
