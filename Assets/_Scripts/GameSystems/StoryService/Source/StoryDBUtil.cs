using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Story.Internal
{
    static class StoryDBUtil
    {
        public static StoryMutatorParams FromDB(DB.DBStoryMutatorParams dbMutatorParams)
        {
            return new StoryMutatorParams((StoryMutatorType)dbMutatorParams.MutateType, dbMutatorParams.Params);
        }

        public static DB.DBStoryMutatorParams ToDB(StoryMutatorParams mutatorParams)
        {
            DB.DBStoryMutatorParams dBStoryMutatorParams = new DB.DBStoryMutatorParams();
            dBStoryMutatorParams.MutateType = (int)mutatorParams.StoryMutatorType;
            dBStoryMutatorParams.Params.AddRange(mutatorParams.Params);

            return dBStoryMutatorParams;
        }

        public static StoryObjective FromDB(DB.DBStoryObjective dbStoryCondition)
        {
            return new StoryObjective(
                (StoryEventType)dbStoryCondition.EventType
                , dbStoryCondition.Param
                , dbStoryCondition.Progress
                , dbStoryCondition.Goal);

        }

        public static DB.DBStoryObjective ToDB(StoryObjective storyCondition)
        {
            DB.DBStoryObjective dbStoryCondition = new DB.DBStoryObjective();
            dbStoryCondition.EventType = (int)storyCondition.ListeningForEvent;
            dbStoryCondition.Param = storyCondition.ListeningForParam;
            dbStoryCondition.Progress = storyCondition.Progress;
            dbStoryCondition.Goal = storyCondition.Goal;

            return dbStoryCondition;
        }

        public static StoryNode FromDB(DB.DBStoryNodeInfo dbStoryNodeInfo)
        {
            StoryNode storyNode = new StoryNode();

            storyNode.Name = dbStoryNodeInfo.Name;
            storyNode.Description = dbStoryNodeInfo.Description;

            foreach (DB.DBStoryObjective objective in dbStoryNodeInfo.CompletionObjectives)
            {
                storyNode.Objectives.Add(FromDB(objective));
            }
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
            StoryObjective activationCondition = FromDB(dbStoryArc.ActivationCondition);

            return new StoryArc(storyArcId, storyArc, activationCondition);
        }

        public static DB.DBStoryNodeInfo CreateConversationStoryNode(string name, string description, int conversationId, int conversationOwner)
        {
            DB.DBStoryNodeInfo node = new DB.DBStoryNodeInfo();
            node.Name = name;
            node.Description = description;

            { // Condition
                node.CompletionObjectives.Add(ToDB(new StoryObjective(StoryEventType.ConversationComplete, conversationId, 0, 1)));
            }

            {// OnActivate
                node.OnActivateChanges.Add(ToDB(StoryMutatorParams.PropMutateParams(
                    conversationOwner,
                    (int)PropMutateType.Conversation,
                    conversationId,
                    MutatorConstants.ADD)));
                node.OnActivateChanges.Add(ToDB(StoryMutatorParams.PropMutateParams(
                    conversationOwner,
                    (int)PropMutateType.Interactible,
                    MutatorConstants.TRUE)));
            }

            {// OnComplete
                node.OnCompleteChanges.Add(ToDB(StoryMutatorParams.PropMutateParams(
                    conversationOwner,
                    (int)PropMutateType.Conversation,
                    conversationId,
                    MutatorConstants.REMOVE)));
            }

            return node;
        }

        public static DB.DBStoryNodeInfo CreateCompleteEncounterStoryNode(string name, string description, int encounterId)
        {
            DB.DBStoryNodeInfo node = new DB.DBStoryNodeInfo();
            node.Name = name;
            node.Description = description;

            { // Condition
                node.CompletionObjectives.Add(ToDB(new StoryObjective(StoryEventType.EncounterComplete, encounterId, 0, 1)));
            }

            {// OnActivate
                node.OnActivateChanges.Add(ToDB(StoryMutatorParams.EncounterMutateParams(
                    encounterId,
                    (int)EncounterMutateType.Active,
                    MutatorConstants.TRUE)));
            }

            {// OnComplete
                node.OnCompleteChanges.Add(ToDB(StoryMutatorParams.EncounterMutateParams(
                    encounterId,
                    (int)EncounterMutateType.Active,
                    MutatorConstants.FALSE)));
            }

            return node;
        }

        public static DB.DBStoryNodeInfo CreateViewCinematicStoryNode(string name, string description, int cinematicId)
        {
            DB.DBStoryNodeInfo node = new DB.DBStoryNodeInfo();
            node.Name = name;
            node.Description = description;

            { // Condition
                node.CompletionObjectives.Add(ToDB(new StoryObjective(StoryEventType.CinematicComplete, cinematicId, 0, 1)));
            }

            {// OnActivate
                node.OnActivateChanges.Add(ToDB(StoryMutatorParams.CinematicMutatorParams(
                    cinematicId,
                    (int)CinematicMutateType.Active,
                    MutatorConstants.TRUE)));
            }

            {// OnComplete
                node.OnCompleteChanges.Add(ToDB(StoryMutatorParams.CinematicMutatorParams(
                    cinematicId,
                    (int)CinematicMutateType.Active,
                    MutatorConstants.FALSE)));
            }

            return node;
        }

        public static DB.DBStoryNodeInfo CreateItemRetrievalStoryNode(string name, string description, int itemId, int containerId)
        {
            DB.DBStoryNodeInfo node = new DB.DBStoryNodeInfo();
            node.Name = name;
            node.Description = description;

            { // Condition
                node.CompletionObjectives.Add(ToDB(new StoryObjective(StoryEventType.ItemAddedToInventory, itemId, 0, 1)));
            }

            {// OnActivate
                node.OnActivateChanges.Add(ToDB(StoryMutatorParams.PropMutateParams(
                    containerId,
                    (int)PropMutateType.Item,
                    itemId,
                    MutatorConstants.ADD)));
                node.OnActivateChanges.Add(ToDB(StoryMutatorParams.PropMutateParams(
                    containerId,
                    (int)PropMutateType.Interactible,
                    MutatorConstants.TRUE)));
            }

            {// OnComplete
                node.OnCompleteChanges.Add(ToDB(StoryMutatorParams.PropMutateParams(
                    containerId,
                    (int)PropMutateType.Interactible,
                    MutatorConstants.FALSE)));
            }

            return node;
        }

        public static DB.DBStoryNodeInfo CreateGatherItem(string name, string description, int itemId, int dropChance, int dropAmount, int dropVarience, int numToGather, int mobId)
        {
            DB.DBStoryNodeInfo node = new DB.DBStoryNodeInfo();
            node.Name = name;
            node.Description = description;

            { // Condition
                node.CompletionObjectives.Add(ToDB(new StoryObjective(StoryEventType.ItemAddedToInventory, itemId, 0, numToGather)));
            }

            {// OnActivate
                if (mobId != -1)
                {
                    node.OnActivateChanges.Add(ToDB(StoryMutatorParams.LootTableParams(
                    (int)LootTableMutateType.Mob,
                    mobId,
                    itemId,
                    MutatorConstants.ADD,
                    dropChance,
                    dropAmount,
                    dropVarience)));
                }
            }

            {// OnComplete
                if (mobId != -1)
                {
                    node.OnCompleteChanges.Add(ToDB(StoryMutatorParams.LootTableParams(
                    (int)LootTableMutateType.Mob,
                    mobId,
                    itemId,
                    MutatorConstants.REMOVE,
                    dropChance,
                    dropAmount,
                    dropVarience)));
                }
            }

            return node;
        }
    }
}
