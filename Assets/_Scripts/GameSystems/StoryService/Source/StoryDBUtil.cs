using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Story.Internal
{
    static class StoryDBUtil
    {
        public static StoryMutatorParams FromDB(DB.DBStoryMutatorParams dbMutatorParams)
        {
            return new StoryMutatorParams(
                (StoryMutatorType)dbMutatorParams.Param0
                , dbMutatorParams.Param1
                , dbMutatorParams.Param2
                , dbMutatorParams.Param3
                );
        }

        public static DB.DBStoryMutatorParams ToDB(StoryMutatorParams mutatorParams)
        {
            DB.DBStoryMutatorParams dBStoryMutatorParams = new DB.DBStoryMutatorParams();
            dBStoryMutatorParams.Param0 = (int)mutatorParams.StoryMutatorType;
            dBStoryMutatorParams.Param1 = mutatorParams.Param1;
            dBStoryMutatorParams.Param2 = mutatorParams.Param2;
            dBStoryMutatorParams.Param3 = mutatorParams.Param3;

            return dBStoryMutatorParams;
        }

        public static StoryCondition FromDB(DB.DBStoryCondition dbStoryCondition)
        {
            return new StoryCondition(
                (StoryEventType)dbStoryCondition.EventType
                , dbStoryCondition.Param);
        }

        public static DB.DBStoryCondition ToDB(StoryCondition storyCondition)
        {
            DB.DBStoryCondition dbStoryCondition = new DB.DBStoryCondition();
            dbStoryCondition.EventType = (int)storyCondition.StoryEventType;
            dbStoryCondition.Param = storyCondition.EventParam;

            return dbStoryCondition;
        }

        public static StoryNode FromDB(DB.DBStoryNodeInfo dbStoryNodeInfo)
        {
            StoryNode storyNode = new StoryNode();

            storyNode.Name = dbStoryNodeInfo.Name;
            storyNode.Description = dbStoryNodeInfo.Description;
            storyNode.Requirement = FromDB(dbStoryNodeInfo.CompletionCondition);
            foreach (DB.DBStoryMutatorParams activationChange in dbStoryNodeInfo.OnActivateChanges)
            {
                storyNode.ChangesOnActivation.Add(FromDB(activationChange));
            }
            foreach (DB.DBStoryMutatorParams completionChange in dbStoryNodeInfo.OnCompleteChanges)
            {
                storyNode.ChangesOnCompletion.Add(FromDB(completionChange));
            }

            return storyNode;
        }

        public static StoryArc FromDB(DB.DBStoryArcInfo dbStoryArc)
        {
            StoryArcId storyArcId = (StoryArcId)dbStoryArc.Id;
            List<StoryNode> storyArc = new List<StoryNode>();
            foreach (DB.DBStoryNodeInfo nodeInfo in dbStoryArc.StoryArc)
            {
                storyArc.Add(FromDB(nodeInfo));
            }
            StoryCondition activationCondition = FromDB(dbStoryArc.ActivationCondition);

            return new StoryArc(storyArcId, storyArc, activationCondition);
        }

        public static DB.DBStoryNodeInfo CreateConversationStoryNode(string name, string description, int conversationId, int conversationOwner)
        {
            DB.DBStoryNodeInfo node = new DB.DBStoryNodeInfo();
            node.Name = name;
            node.Description = description;

            { // Condition
                node.CompletionCondition = ToDB(new StoryCondition(StoryEventType.ConversationComplete, conversationId));
            }

            {// OnActivate
                node.OnActivateChanges.Add(ToDB(new StoryMutatorParams(
                    PropMutatorType.Conversation_Add,
                    conversationOwner,
                    conversationId)));
            }

            {// OnComplete
                node.OnCompleteChanges.Add(ToDB(new StoryMutatorParams(
                    PropMutatorType.Conversation_Remove,
                    conversationOwner,
                    conversationId)));
            }

            return node;
        }

        public static DB.DBStoryNodeInfo CreateItemRetrievalStoryNode(string name, string description, int itemId, int containerId)
        {
            DB.DBStoryNodeInfo node = new DB.DBStoryNodeInfo();
            node.Name = name;
            node.Description = description;

            { // Condition
                node.CompletionCondition = ToDB(new StoryCondition(StoryEventType.ItemAddedToInventory, itemId));
            }

            {// OnActivate
                node.OnActivateChanges.Add(ToDB(new StoryMutatorParams(
                    PropMutatorType.Interactible,
                    containerId,
                    (int)MutatorConstants.TRUE)));
                node.OnActivateChanges.Add(ToDB(new StoryMutatorParams(
                    PropMutatorType.Item_Add,
                    containerId,
                    itemId)));
            }

            {// OnComplete
                node.OnCompleteChanges.Add(ToDB(new StoryMutatorParams(
                    PropMutatorType.Interactible,
                    containerId,
                    (int)MutatorConstants.FALSE)));
            }

            return node;
        }
    }
}
