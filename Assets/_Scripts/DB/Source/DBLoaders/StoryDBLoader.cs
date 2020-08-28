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
            // Debug Story Arc
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

            DBService.Get().UpdateStoryDB();
        }
    }
}


