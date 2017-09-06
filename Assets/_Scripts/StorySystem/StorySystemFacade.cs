using Common.CharacterEnums;
using Common.ProfessionEnums;
using StorySystem.Common;
using StorySystem.StoryArcSystem;
using StorySystem.StoryCast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Character;
using WorldSystem.Interface;

namespace StorySystem.Interface
{
    class StorySystemFacade
    {
        private StoryArcManager mStoryArcManager;

        private static StorySystemFacade mInstance;

        public static StorySystemFacade Instance()
        {
            if (mInstance == null)
            {
                mInstance = new StorySystemFacade();
                UnityEngine.Debug.Log("Accessing Story System Facade before properly initialized");
            }
            return mInstance;
        }

        public StorySystemFacade()
        {
            UnityEngine.Debug.Log("Creating StorySystemFacade...");
            mInstance = this;
            mStoryArcManager = new StoryArcManager();
        }

        public void Initialize()
        {
            WorldSystemFacade.Instance().AddCharacterToRoster(StoryCastFactory.CheckoutStoryCharacter(StoryCharacterId.Rheinhardt, 3, UnitGroup.Player));
            WorldSystemFacade.Instance().AddCharacterToRoster(StoryCastFactory.CheckoutStoryCharacter(StoryCharacterId.Asmund, 2, UnitGroup.Player));
            WorldSystemFacade.Instance().AddCharacterToRoster(StoryCastFactory.CheckoutStoryCharacter(StoryCharacterId.Thanos, 1, UnitGroup.Player));
            WorldSystemFacade.Instance().AddCharacterToRoster(StoryCastFactory.CheckoutStoryCharacter(StoryCharacterId.Magnus, 2, UnitGroup.Player));
            WorldSystemFacade.Instance().AddCharacterToRoster(new CharacterBase(UnitGroup.Player, 2000, "Soldier " + 1, (CharacterGender)UnityEngine.Random.Range(0, 1), ProfessionType.Footman, 2));
            WorldSystemFacade.Instance().AddCharacterToRoster(new CharacterBase(UnitGroup.Player, 2001, "Soldier " + 2, (CharacterGender)UnityEngine.Random.Range(0, 1), ProfessionType.Adept, 2));
            WorldSystemFacade.Instance().AddCharacterToRoster(new CharacterBase(UnitGroup.Player, 2002, "Soldier " + 3, (CharacterGender)UnityEngine.Random.Range(0, 1), ProfessionType.Archer, 2));
        }

        public void BeginNewStoryArc(StoryArcId arcId)
        {
            mStoryArcManager.BeginNewStoryArc(arcId);
        }

        public void NotifyStoryEvent(StoryEventPayload eventPayload)
        {
            mStoryArcManager.ProgressStoryArcs(eventPayload);
        }
    }
}
