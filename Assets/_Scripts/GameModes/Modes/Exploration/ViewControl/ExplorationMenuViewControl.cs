using MAGE.GameSystems;
using MAGE.GameSystems.Story;
using MAGE.Messaging;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.FlowControl
{
    class ExplorationMenuViewControl
        : FlowControlBase
        , UIContainerControl
    {
        private bool mIsQuestLogOpen = false;

        private List<StoryArcInfo> mRecentlyUpdatedArcs = new List<StoryArcInfo>();
        private static HashSet<StoryArcId> mUpdatedStoryArcs = new HashSet<StoryArcId>();

        private static StoryProgress mStoryProgress = null;

        private Optional<int> mSelectedQuestIdx = Optional<int>.Empty;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.ExplorationMenuFlowControl;
        }

        protected override void Setup()
        {
            UIManager.Instance.PostContainer(UIContainerId.ExplorationMenuView, this);

            StoryProgress updatedStoryProgress = StoryService.Get().GetStoryProgress();
            if (mStoryProgress != null)
            {
                foreach (StoryArcInfo storyArc in updatedStoryProgress.StoryArcs.Values)
                {
                    if (storyArc.Status == StoryArcStatus.Active)
                    {
                        StoryArcInfo existingInfo = mStoryProgress.StoryArcs[storyArc.StoryArcId];
                        if (existingInfo.Status != storyArc.Status
                            || existingInfo.Stage != storyArc.Stage)
                        {
                            mRecentlyUpdatedArcs.Add(storyArc);

                            if (storyArc.Status == StoryArcStatus.Active)
                            {
                                mUpdatedStoryArcs.Add(storyArc.StoryArcId);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (StoryArcInfo storyArc in updatedStoryProgress.StoryArcs.Values)
                {
                    if (storyArc.Status == StoryArcStatus.Active)
                    {
                        mRecentlyUpdatedArcs.Add(storyArc);
                        mUpdatedStoryArcs.Add(storyArc.StoryArcId);
                    }
                }
            }

            mStoryProgress = updatedStoryProgress;
        }

        protected override void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.ExplorationMenuView);
            UIManager.Instance.RemoveOverlay(UIContainerId.QuestLogView);
        }

        public override void HandleMessage(MessageInfoBase eventInfoBase)
        {
            if (eventInfoBase.MessageId == StoryMessage.Id)
            {
                StoryMessage storyMessage = eventInfoBase as StoryMessage;

                if (storyMessage.Type == GameSystems.Story.MessageType.StoryArcUpdated)
                {
                    StoryArcInfo info = storyMessage.Arg<StoryArcInfo>();
                    mRecentlyUpdatedArcs.Add(info);

                    if (info.Status == StoryArcStatus.Active)
                    {
                        mUpdatedStoryArcs.Add(info.StoryArcId);
                    }

                    if (!mIsQuestLogOpen)
                    {
                        UIManager.Instance.Publish(UIContainerId.ExplorationMenuView);
                    }
                }
            }
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch (containerId)
            {
                case (int)UIContainerId.ExplorationMenuView:
                {
                    HandleMenuInteraction(interactionInfo);
                }
                break;
                case (int)UIContainerId.QuestLogView:
                {
                    HandleQuestLogInteraction(interactionInfo);
                }
                break;
            }
        }

        private void HandleMenuInteraction(UIInteractionInfo interactionInfo)
        {
            if (interactionInfo.InteractionType == UIInteractionType.Click)
            {
                switch (interactionInfo.ComponentId)
                {
                    case (int)ExplorationMenuView.ComponentId.SaveBtn:
                    {
                        MAGE.GameSystems.WorldService.Get().Save();
                    }
                    break;

                    case (int)ExplorationMenuView.ComponentId.ExitBtn:
                    {
                        //GameModesModule.Instance.Quit();
                    }
                    break;
                    case (int)ExplorationMenuView.ComponentId.OutfiterBtn:
                    {
                        SendFlowMessage("outfit");
                    }
                    break;
                    case (int)ExplorationMenuView.ComponentId.MapBtn:
                    {
                        //GameModesModule.Instance.Map();
                    }
                    break;
                    case (int)ExplorationMenuView.ComponentId.QuestLog:
                    {
                        RefreshStoryArcs();
                        mSelectedQuestIdx.Reset();

                        UIManager.Instance.PostContainer(UIContainerId.QuestLogView, this);
                    }
                    break;
                }
            }
        }

        private void HandleQuestLogInteraction(UIInteractionInfo interactionInfo)
        {
            if (interactionInfo.InteractionType == UIInteractionType.Click)
            {
                switch (interactionInfo.ComponentId)
                {
                    case (int)QuestLogView.ComponentId.Exit:
                    {
                        mSelectedQuestIdx = Optional<int>.Empty;
                        UIManager.Instance.RemoveOverlay(UIContainerId.QuestLogView);
                    }
                    break;

                    case (int)QuestLogView.ComponentId.QuestSelections:
                    {
                        ListInteractionInfo listInteractionInfo = interactionInfo as ListInteractionInfo;

                        mSelectedQuestIdx = listInteractionInfo.ListIdx;

                        StoryArcId hoveredArc = mStoryProgress.InProgress[listInteractionInfo.ListIdx];
                        mUpdatedStoryArcs.Remove(hoveredArc);

                        UIManager.Instance.Publish(UIContainerId.QuestLogView);
                    }
                    break;
                }
            }
        }

        public IDataProvider Publish(int containerId)
        {
            IDataProvider dp = IDataProvider.Empty;

            switch ((UIContainerId)containerId)
            {
                case UIContainerId.QuestLogView:
                {
                    QuestLogView.DataProvider questLogDP = new QuestLogView.DataProvider();
                    questLogDP.SelectedQuest = mSelectedQuestIdx;

                    foreach (StoryArcId activeId in mStoryProgress.InProgress)
                    {
                        StoryArcInfo activeQuest = mStoryProgress.StoryArcs[activeId];

                        questLogDP.IsQuestUpdated.Add(mUpdatedStoryArcs.Contains(activeQuest.StoryArcId));
                        questLogDP.QuestNames.Add(activeQuest.StoryArcName);
                        questLogDP.QuestObjectives.Add(activeQuest.CurrentObjective);
                        questLogDP.QuestDescriptions.Add(activeQuest.CurrentDescription);
                    }

                    dp = questLogDP;
                }
                break;

                case UIContainerId.ExplorationMenuView:
                {
                    ExplorationMenuView.DataProvider menuDP = new ExplorationMenuView.DataProvider();

                    menuDP.HasUpdatedQuests = mUpdatedStoryArcs.Count > 0;
                    
                    foreach (StoryArcInfo storyArcInfo in mRecentlyUpdatedArcs)
                    {
                        string message = storyArcInfo.StoryArcName;
                        string info = "";
                        if (storyArcInfo.Status == StoryArcStatus.Active)
                        {
                            if (storyArcInfo.Stage == 0)
                            {
                                message += ": Started";
                            }
                            else
                            {
                                message += ": Updated!";
                            }

                            info = storyArcInfo.CurrentDescription;
                        }
                        else
                        {
                            info = ": Complete!";
                        }

                        menuDP.UpdateMessages.Add(new KeyValuePair<string, string>(message, info));
                    }

                    mRecentlyUpdatedArcs.Clear();

                    dp = menuDP;
                }
                break;
            }

            return dp;
        }

        public string Name()
        {
            return GetFlowControlId().ToString();
        }

        private void RefreshStoryArcs()
        {
        //    Dictionary<StoryArcId, StoryArcInfo> infoLookup = new Dictionary<StoryArcId, StoryArcInfo>();
        //    HashSet<StoryArcId> updatedArcs = new HashSet<StoryArcId>();

        //    foreach (StoryArcInfo storyArcInfo in StoryService.Get().GetActiveStoryArcs())
        //    {
        //        infoLookup.Add(storyArcInfo.StoryArcId, storyArcInfo);

        //        bool isArcUpdated = false;

        //        if (!mUpdatedStoryArcs.Contains(storyArcInfo.StoryArcId))
        //        {
        //            if (mStoryArcs.ContainsKey(storyArcInfo.StoryArcId))
        //            {
        //                if (mStoryArcs[storyArcInfo.StoryArcId].Stage != storyArcInfo.Stage)
        //                {
        //                    isArcUpdated = true;
        //                }
        //            }
        //            else
        //            {
        //                isArcUpdated = true;
        //            }
        //        }
        //        else
        //        {
        //            isArcUpdated = true;
        //        }

        //        if (isArcUpdated)
        //        {
        //            updatedArcs.Add(storyArcInfo.StoryArcId);
        //        }
        //    }

        //    mStoryArcs = infoLookup;
        //    mUpdatedStoryArcs = updatedArcs;
        }
    }
}
