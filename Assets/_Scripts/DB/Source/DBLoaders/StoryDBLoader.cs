using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Story;
using MAGE.GameSystems.Story.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class StoryDBLoader
    {
        public static void LoadDB()
        {
            // Test Story Arc
            {
                StoryArcId storyArcId = StoryArcId.Test;

                DBStoryArcInfo storyArcInfo = new DBStoryArcInfo();
                storyArcInfo.Id = (int)storyArcId;
                storyArcInfo.Name = storyArcId.ToString();

                // Activate Condition
                {
                    DBStoryCondition activateCondition = new DBStoryCondition();
                    activateCondition.EventType = (int)StoryEventType.NewGame;
                    storyArcInfo.ActivationCondition = activateCondition;
                }

                // Nodes
                {
                    { // Conversation with field vendor
                        storyArcInfo.StoryArc.Add(StoryDBUtil.CreateConversationStoryNode(
                            "Quieter than usual..."
                            , "Talk with the locals to figure out what's going on."
                            , (int)ConversationId.BanditsInTheHills
                            , (int)NPCPropId.FieldVendor));
                    }

                    { // Get the Key
                        storyArcInfo.StoryArc.Add(StoryDBUtil.CreateItemRetrievalStoryNode(
                            "Where the blazes is that key..."
                            , "Find the key to the gate."
                            , (int)StoryItemId.GateKey
                            , (int)ContainerPropId.FieldVendorContainer));
                    }

                    { // Conversation with field vendor
                        storyArcInfo.StoryArc.Add(StoryDBUtil.CreateConversationStoryNode(
                            "Definitely Wasn't where he said it was..."
                            , "Return to the captain to unlock the gate."
                            , (int)ConversationId.UnlockTheGate
                            , (int)NPCPropId.FieldVendor));
                    }
                }

                DBService.Get().WriteStoryArcInfo(storyArcInfo.Id, storyArcInfo);
            }

            // Debug Level Story Arc
            {
                StoryArcId storyArcId = StoryArcId.DebugLevel;

                DBStoryArcInfo storyArcInfo = new DBStoryArcInfo();
                storyArcInfo.Id = (int)storyArcId;
                storyArcInfo.Name = storyArcId.ToString();

                // Activate Condition
                {
                    DBStoryCondition activateCondition = new DBStoryCondition();
                    activateCondition.EventType = (int)StoryEventType.NewGame;
                    storyArcInfo.ActivationCondition = activateCondition;
                }

                // Nodes
                {
                    { // Opening Cinematic
                        DBStoryNodeInfo storyNode = StoryDBUtil.CreateViewCinematicStoryNode(
                            ""
                            , ""
                            , (int)CinematicId.Demo_IntroCinematic);

                        storyNode.OnCompleteChanges.Add(StoryDBUtil.ToDB(new StoryMutatorParams(
                           PropMutatorType.StateChange
                           , (int)DoorPropId.DemoLevel_TownGateFront
                           , MutatorConstants.OPEN)));

                        storyArcInfo.StoryArc.Add(storyNode);
                    }

                    { // Find the magistrate
                        DBStoryNodeInfo storyNode = StoryDBUtil.CreateConversationStoryNode(
                            "Who's in charge here?"
                            , "Find the Magistrates office and talk with the town Magistrate"
                            , (int)ConversationId.Demo_LayDownTheLaw
                            , (int)NPCPropId.DemoLevel_Magistrate);

                        storyNode.OnActivateChanges.Add(StoryDBUtil.ToDB(new StoryMutatorParams(
                            CinematicMutatorType.Activate
                            , (int)CinematicId.Demo_TownHallCinematic
                            , MutatorConstants.TRUE)));

                        storyArcInfo.StoryArc.Add(storyNode);
                    }

                    { // Talk with captain to trigger tutorial battle
                        storyArcInfo.StoryArc.Add(StoryDBUtil.CreateConversationStoryNode(
                            "One last training session"
                            , "Talk with Balgrid and run the recruits through the paces"
                            , (int)ConversationId.Demo_TrainingTime
                            , (int)NPCPropId.DemoLevel_Captain));
                    }

                    { // Training Battles
                        storyArcInfo.StoryArc.Add(StoryDBUtil.CreateCompleteEncounterStoryNode(
                            ""
                            , ""
                            , (int)EncounterScenarioId.Demo_TrainingGrounds));
                    }

                    { // Talk with captain to trigger tutorial battle
                        storyArcInfo.StoryArc.Add(StoryDBUtil.CreateConversationStoryNode(
                            "Training complete"
                            , "See Balgrid now that the training is complete"
                            , (int)ConversationId.Demo_TrainingComplete
                            , (int)NPCPropId.DemoLevel_Captain));
                    }

                    //{ // Tutorial battles with captain
                    //    storyArcInfo.StoryArc.Add(StoryDBUtil.CreateConversationStoryNode(
                    //        "One last training session"
                    //        , "Talk with Balgrid and run the recruits through the paces"
                    //        , (int)ConversationId.Demo_TrainingTime
                    //        , (int)NPCPropId.DemoLevel_Captain));
                    //}
                }

                DBService.Get().WriteStoryArcInfo(storyArcInfo.Id, storyArcInfo);
            }

            DBService.Get().UpdateStoryDB();
        }
    }
}


