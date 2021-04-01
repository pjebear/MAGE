using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.UI.Views
{
    class EncounterIntroView : UIContainer
    {
        public enum ComponentId
        {
            ContinueBtn
        }

        public class DataProvider : IDataProvider
        {
            public string EncounterType;
            public string EncounterInfo;
            public List<string> WinConditions = new List<string>();
            public List<string> LoseConditions = new List<string>();
            public List<string> Rewards = new List<string>();
        }

        public UIText EncounterTypeText;
        public UIText EncounterInfoText;
        public UIText EncounterConditionsText;
        public UIText RewardsText;

        public UIButton ContinueButton;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            EncounterTypeText.Publish(new UIText.DataProvider(dp.EncounterType));
            EncounterInfoText.Publish(new UIText.DataProvider(dp.EncounterInfo));

            int counter = 1;
            string conditionsText = "Win Conditions:" + Console.Out.NewLine;
            foreach (string condition in dp.WinConditions)
            {
                conditionsText += string.Format("{0}. {1}", counter++, condition) + Console.Out.NewLine;
            }

            counter = 1;
            conditionsText += "Lose Conditions:" + Console.Out.NewLine;
            foreach (string condition in dp.LoseConditions)
            {
                conditionsText += string.Format("{0}. {1}", counter++, condition) + Console.Out.NewLine;
            }

            EncounterConditionsText.Publish(new UIText.DataProvider(conditionsText));

            string rewardsText = "Rewards:" + Console.Out.NewLine;
            foreach (string reward in dp.Rewards)
            {
                rewardsText += string.Format("{0}. {1}", counter++, reward) + Console.Out.NewLine;
            }
            RewardsText.Publish(new UIText.DataProvider(rewardsText));
        }

        protected override void InitChildren()
        {
            ContinueButton.Init((int)ComponentId.ContinueBtn, this);
        }
    }
}
