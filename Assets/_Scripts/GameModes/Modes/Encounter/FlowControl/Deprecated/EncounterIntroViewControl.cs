﻿using MAGE.GameModes.Encounter;
using MAGE.GameSystems;
using MAGE.GameSystems.World;
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
    class EncounterIntroViewControl
    : MonoBehaviour,
    UIContainerControl
    {
        private string Tag = "EncounterIntroViewControl";

        public void Init()
        {

        }

        public void Show()
        {
            UIManager.Instance.PostContainer(UIContainerId.EncounterIntroView, this);
        }

        public void Hide()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterIntroView);
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch (containerId)
            {
                case (int)UIContainerId.EncounterIntroView:
                {
                    if (interactionInfo.ComponentId == (int)EncounterIntroView.ComponentId.ContinueBtn
                        && interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.IntroComplete));
                    }
                }
                break;
            }
        }

        public string Name()
        {
            return Tag;
        }

        public IDataProvider Publish(int containerId)
        {
            EncounterIntroView.DataProvider dp = new EncounterIntroView.DataProvider();

            EncounterContext context = null;// GameModel.Encounter.EncounterContext;

            dp.EncounterType = context.EncounterType.ToString();
            dp.EncounterInfo = "";

            foreach (EncounterCondition encounterCondition in context.WinConditions)
            {
                dp.WinConditions.Add(encounterCondition.ToString());
            }

            foreach (EncounterCondition encounterCondition in context.LoseConditions)
            {
                dp.LoseConditions.Add(encounterCondition.ToString());
            }

            dp.Rewards.Add(context.Rewards.Currency.ToString() + " gil");
            foreach (var itemRewardPair in context.Rewards.Items)
            {
                Item item = ItemFactory.LoadItem(itemRewardPair.Key);
                dp.Rewards.Add(string.Format("{0} x {1}", item.Name, itemRewardPair.Value));
            }

            return dp;
        }
    }
}


