using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Mobs;
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
            //{
            //    StoryArcId storyArcId = StoryArcId.Test;

            //    DBStoryArcInfo storyArcInfo = new DBStoryArcInfo();
            //    storyArcInfo.Id = (int)storyArcId;
            //    storyArcInfo.Name = storyArcId.ToString();

            //    // Activate Condition
            //    {
            //        DBStoryObjective activateCondition = new DBStoryObjective();
            //        activateCondition.EventType = (int)StoryEventType.NewGame;
            //        storyArcInfo.ActivationCondition = activateCondition;
            //    }

            //    // Nodes
            //    {
            //        { // Conversation with field vendor
            //            storyArcInfo.StoryArc.Add(StoryDBUtil.CreateConversationStoryNode(
            //                "Quieter than usual..."
            //                , "Talk with the locals to figure out what's going on."
            //                , (int)ConversationId.BanditsInTheHills
            //                , (int)NPCPropId.FieldVendor));
            //        }

            //        { // Get the Key
            //            storyArcInfo.StoryArc.Add(StoryDBUtil.CreateItemRetrievalStoryNode(
            //                "Where the blazes is that key..."
            //                , "Find the key to the gate."
            //                , (int)StoryItemId.GateKey
            //                , (int)ContainerPropId.FieldVendorContainer));
            //        }

            //        { // Conversation with field vendor
            //            storyArcInfo.StoryArc.Add(StoryDBUtil.CreateConversationStoryNode(
            //                "Definitely Wasn't where he said it was..."
            //                , "Return to the captain to unlock the gate."
            //                , (int)ConversationId.UnlockTheGate
            //                , (int)NPCPropId.FieldVendor));
            //        }
            //    }

            //    DBService.Get().WriteStoryArcInfo(storyArcInfo.Id, storyArcInfo);
            //}

            // Debug Level Gather Bear Pelts
            {
                StoryArcId storyArcId = StoryArcId.OnBoardingBlacksmith;

                DBStoryArcInfo storyArcInfo = new DBStoryArcInfo();
                storyArcInfo.Id = (int)storyArcId;
                storyArcInfo.Name = storyArcId.ToString();

                // Activate Condition
                {
                    DBStoryObjective activateCondition = new DBStoryObjective();
                    activateCondition.EventType = (int)StoryEventType.NewGame;
                    storyArcInfo.ActivationCondition = activateCondition;
                }

                { // Talk with blacksmith to start quest
                    storyArcInfo.StoryArc.Add(StoryDBUtil.CreateConversationStoryNode(
                        "Learn to use the black smith feature"
                        , "Talk with the blacksmith"
                        , (int)ConversationId.Demo_BlackSmithTraining_Start
                        , (int)NPCPropId.DemoLevel_BlackSmith));
                }

                { // Gather bear pelts
                    storyArcInfo.StoryArc.Add(StoryDBUtil.CreateGatherItem(
                          "These pelts will go a long way"
                          , "Gather pelts from the bears in the hills"
                          , (int)StoryItemId.DEMO_GoldenBearPelt
                          , 100
                          , 1
                          , 0
                          , 1
                          , (int)MobId.DEMO_Bear));
                }

                { // Return quest
                    DBStoryNodeInfo storyNode = StoryDBUtil.CreateConversationStoryNode(
                        "Return to the Blacksmith"
                        , "Return to the Blacksmith"
                        , (int)ConversationId.Demo_BlackSmithTraining_Return
                        , (int)NPCPropId.DemoLevel_BlackSmith);

                    // Remove the bear pelts
                    storyNode.OnCompleteChanges.Add(StoryDBUtil.ToDB(StoryMutatorParams.RemovePartyItem(
                    (int)StoryItemId.DEMO_GoldenBearPelt)));

                    // Reward for quest
                    storyNode.OnCompleteChanges.Add(StoryDBUtil.ToDB(StoryMutatorParams.PartyMutateParams(
                        (int)PartyMutateType.Item
                        , (int)EquippableId.Relic
                        , MutatorConstants.ADD)));
                    
                    storyArcInfo.StoryArc.Add(storyNode);
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
                    DBStoryObjective activateCondition = new DBStoryObjective();
                    activateCondition.EventType = (int)StoryEventType.NewGame;
                    storyArcInfo.ActivationCondition = activateCondition;
                }

                
                  // Nodes
                {
                    { // Opening Cinematic
                          DBStoryNodeInfo storyNode = StoryDBUtil.CreateViewCinematicStoryNode(
                              ""
                              , ""
                              , (int)CinematicId.Intro);

                        storyNode.OnCompleteChanges.Add(StoryDBUtil.ToDB(StoryMutatorParams.PropMutateParams(
                           (int)DoorPropId.DemoLevel_TownGateFront
                           , (int)PropMutateType.StateChange
                           , MutatorConstants.OPEN)));

                        storyArcInfo.StoryArc.Add(storyNode);
                      }

                      { // Find the magistrate
                          DBStoryNodeInfo storyNode = StoryDBUtil.CreateConversationStoryNode(
                              "Who's in charge here?"
                              , "Find the Magistrates office and talk with the town Magistrate"
                              , (int)ConversationId.Demo_LayDownTheLaw
                              , (int)NPCPropId.DemoLevel_Magistrate);

                          storyNode.OnActivateChanges.Add(StoryDBUtil.ToDB(StoryMutatorParams.CinematicMutatorParams(
                              (int)CinematicId.MeetTheMayor
                              , (int)CinematicMutateType.Active
                              , MutatorConstants.TRUE)));

                          storyArcInfo.StoryArc.Add(storyNode);
                      }

                      { // Talk with captain to trigger tutorial battle
                            DBStoryNodeInfo storyNode = StoryDBUtil.CreateConversationStoryNode(
                              "One last training session"
                              , "Talk with Balgrid and run the recruits through the paces"
                              , (int)ConversationId.Demo_TrainingTime
                              , (int)NPCPropId.DemoLevel_Captain);
                        storyNode.OnActivateChanges.Add(StoryDBUtil.ToDB(StoryMutatorParams.PropMutateParams(
                            (int)NPCPropId.DemoLevel_Captain,
                            (int)PropMutateType.Activate,
                            MutatorConstants.TRUE)));
                        storyNode.OnCompleteChanges.Add(StoryDBUtil.ToDB(StoryMutatorParams.PropMutateParams(
                            (int)NPCPropId.DemoLevel_Captain,
                            (int)PropMutateType.Activate,
                            MutatorConstants.FALSE)));


                        storyArcInfo.StoryArc.Add(storyNode);
                      }

                      { // Training Battles
                          storyArcInfo.StoryArc.Add(StoryDBUtil.CreateCompleteEncounterStoryNode(
                              ""
                              , ""
                              , (int)EncounterScenarioId.Demo_TrainingGrounds));
                      }

                    { // Lothar is under attack!
                        DBStoryNodeInfo storyNode = StoryDBUtil.CreateViewCinematicStoryNode(
                            ""
                            , ""
                            , (int)CinematicId.PostTraining);

                        storyArcInfo.StoryArc.Add(storyNode);
                    }

                    { // Talk to Lothar
                        DBStoryNodeInfo storyNode = StoryDBUtil.CreateViewCinematicStoryNode(
                            "Convoy under attack!"
                            , "Travel to the Forest and save convoy from bandits"
                            , (int)CinematicId.LotharUnderAttack);

                        storyArcInfo.StoryArc.Add(storyNode);
                    }

                    { // Save Lothar
                        DBStoryNodeInfo storyNode = StoryDBUtil.CreateCompleteEncounterStoryNode(
                            ""
                            , ""
                            , (int)EncounterScenarioId.Demo_LotharUnderAttack);

                        storyArcInfo.StoryArc.Add(storyNode);
                    }

                    { // Talk to Lothar
                        DBStoryNodeInfo storyNode = StoryDBUtil.CreateViewCinematicStoryNode(
                            ""
                            , ""
                            , (int)CinematicId.LotharSaved);

                        storyArcInfo.StoryArc.Add(storyNode);
                    }
                    // TODO: Lothar saved
                    // 

                    //{ // Talk with captain to review what happens next
                    //    storyArcInfo.StoryArc.Add(StoryDBUtil.CreateConversationStoryNode(
                    //        "Training complete"
                    //        , "See Balgrid now that the training is complete"
                    //        , (int)ConversationId.Demo_TrainingComplete
                    //        , (int)NPCPropId.DemoLevel_Captain));
                    //}

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


