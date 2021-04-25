using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.UI.Views
{
    class EncounterResultsView : UIContainer
    {
        public enum ComponentId
        {
            ContinueBtn
        }

        public enum Stage
        {
            WinLossBanner,
            Rewards
        }
        private Stage mStage = Stage.WinLossBanner;

        public class DataProvider : IDataProvider
        {
            public bool DidUserWin;
            public int CurrentCoins = 0;
            public int CoinRewards = 0;
            public List<CharacterGrowth.DataProvider> CharacterGrowths = new List<CharacterGrowth.DataProvider>();
            public List<IconTextBase.DataProvider> ItemRewards = new List<IconTextBase.DataProvider>();
        }
        private DataProvider mPayload;

        public GameObject WinLossStage;
        public GameObject RewardsStage;

        public UIText WinLossTxt;
        public UIText CurrentCoinsTxt;
        public UIText CoinRewardsTxt;

        public UIGrid CharacterGrowthGrid;
        public UIGrid ItemRewardsGrid;

        public UIButton ContinueButton;

        public override void Publish(IDataProvider dataProvider)
        {
            mPayload = (DataProvider)dataProvider;

            PublishWinLossBanner();
        }

        protected override void InitChildren()
        {
            ContinueButton.Init((int)ComponentId.ContinueBtn, this);
        }

        public override void HandleInteraction(UIInteractionInfo interactionInfo)
        {
            if (interactionInfo.ComponentId == (int)ComponentId.ContinueBtn
                && interactionInfo.InteractionType == UIInteractionType.Click)
            {
                if (mStage == Stage.WinLossBanner && mPayload.DidUserWin)
                {
                    mStage = Stage.Rewards;
                    PublishRewards();
                }
                else
                {
                    base.HandleInteraction(interactionInfo);
                }
            }
        }

        private void PublishWinLossBanner()
        {
            WinLossStage.SetActive(true);
            RewardsStage.SetActive(false);

            WinLossTxt.Publish(mPayload.DidUserWin ? "Encounter Won!" : "Encounter Loss...");
        }

        private void PublishRewards()
        {
            WinLossStage.SetActive(false);
            RewardsStage.SetActive(true);

            CurrentCoinsTxt.Publish(mPayload.CurrentCoins.ToString());
            CoinRewardsTxt.Publish(mPayload.CoinRewards.ToString());
            CharacterGrowthGrid.Publish(new UIGrid.DataProvider(mPayload.CharacterGrowths.Select(x => x as IDataProvider).ToList()));
            ItemRewardsGrid.Publish(new UIGrid.DataProvider(mPayload.ItemRewards.Select(x => x as IDataProvider).ToList()));
        }
    }
}
