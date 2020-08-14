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
        : UIContainerControl
    {
        private Optional<int> mSelectedQuestIdx = Optional<int>.Empty;

        public string Name()
        {
            return "ExplorationMenuViewControl";
        }

        public void Show()
        {
            UIManager.Instance.PostContainer(UIContainerId.ExplorationMenuView, this);
        }

        public void Hide()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.ExplorationMenuView);
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
                        MAGE.GameServices.WorldService.Get().Save();
                    }
                    break;

                    case (int)ExplorationMenuView.ComponentId.ExitBtn:
                    {
                        GameModesModule.Instance.Quit();
                    }
                    break;
                    case (int)ExplorationMenuView.ComponentId.OutfiterBtn:
                    {
                        GameModesModule.Instance.Outfit();
                    }
                    break;
                    case (int)ExplorationMenuView.ComponentId.RandomEncounter:
                    {
                        ExplorationModule.Instance.TriggerRandomEncounter();
                    }
                    break;
                    case (int)ExplorationMenuView.ComponentId.QuestLog:
                    {
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

                    List<GameServices.Story.StoryArcInfo> activeQuests = GameServices.StoryService.Get().GetActiveStoryArcs();
                    foreach (GameServices.Story.StoryArcInfo activeQuest in activeQuests)
                    {
                        questLogDP.IsQuestUpdated.Add(false);
                        questLogDP.QuestNames.Add(activeQuest.StoryArcName);
                        questLogDP.QuestObjectives.Add(activeQuest.CurrentObjective);
                        questLogDP.QuestDescriptions.Add(activeQuest.CurrentDescription);
                    }

                    dp = questLogDP;
                }
                break;
            }

            return dp;
        }
    }


}
