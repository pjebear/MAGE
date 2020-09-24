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
        private Optional<int> mSelectedQuestIdx = Optional<int>.Empty;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.ExplorationMenuFlowControl;
        }

        protected override void Setup()
        {
            UIManager.Instance.PostContainer(UIContainerId.ExplorationMenuView, this);
        }

        protected override void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.ExplorationMenuView);
            UIManager.Instance.RemoveOverlay(UIContainerId.QuestLogView);
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
                        //GameModesModule.Instance.Outfit();
                    }
                    break;
                    case (int)ExplorationMenuView.ComponentId.RandomEncounter:
                    {
                        //ExplorationModule.Instance.TriggerRandomEncounter();
                    }
                    break;
                    case (int)ExplorationMenuView.ComponentId.MapBtn:
                    {
                        //GameModesModule.Instance.Map();
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

                    List<GameSystems.Story.StoryArcInfo> activeQuests = GameSystems.StoryService.Get().GetActiveStoryArcs();
                    foreach (GameSystems.Story.StoryArcInfo activeQuest in activeQuests)
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

        public string Name()
        {
            return GetFlowControlId().ToString();
        }
    }


}
