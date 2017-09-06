using Common.EncounterEnums;
using StorySystem.Common;
using StorySystem.StoryCast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Interface;
using WorldSystem.Managers.WorldTransitions;

namespace StorySystem.StoryArcSystem
{
    static class StoryArcFactory
    {
        public static void CheckOutStoryArc(StoryArcId arcId, out StoryArc toCheckout)
        {
            switch (arcId)
            {
                case (StoryArcId.Main):
                    // do stuff
                    toCheckout = new StoryArc(arcId, () =>
                    {
                        WorldSystemFacade worldSystem = WorldSystemFacade.Instance();
                        worldSystem.SetNextTransition(new EncounterTransition(EncounterScenarioId.Main_Opening));
                        worldSystem.ProgressStory();
                    },
                    new List<StoryArcNode>()
                    {
                        //new StoryArcNode(StoryEventType.Encounter, (int)EncounterScenarioId.Main_Opening, () =>
                        //{
                        //    WorldSystemFacade worldSystem = WorldSystemFacade.Instance();
                        //    worldSystem.SetNextTransition(new EncounterTransition(EncounterScenarioId.Main_Opening));
                        //})
                        //, new StoryArcNode(StoryEventType.Encounter, (int)EncounterScenarioId.Main_Opening, () =>
                        //{
                        //    WorldSystemFacade worldSystem = WorldSystemFacade.Instance();
                        //    worldSystem.SetNextTransition(new EncounterTransition(EncounterScenarioId.Main_Opening));
                        //})
                    });
                    break;

                default:
                    UnityEngine.Debug.LogError("No Arc defined for " + arcId.ToString() + " in StoryArcFactory");
                    toCheckout = null;
                    break;
            }
        }
    }
}
