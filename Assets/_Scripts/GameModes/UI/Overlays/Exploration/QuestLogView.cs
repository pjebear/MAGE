using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MAGE.UI.Views
{
    class QuestLogView
        : UIContainer
    {
        public enum ComponentId
        {
            Exit,
            QuestSelections
        }

        public class DataProvider : IDataProvider
        {
            public List<string> QuestNames = new List<string>();
            public List<string> QuestObjectives = new List<string>();
            public List<string> QuestDescriptions = new List<string>();
            public List<bool> IsQuestUpdated = new List<bool>();

            public Optional<int> SelectedQuest;
        }

        public UIButton ExitBtn;
        public Transform QuestInfoContainer; 
        public UIText QuestInfoNameTxt;
        public UIText QuestInfoObjectiveTxt;
        public UIText QuestInfoDescriptionTxt;
        public UIList QuestList;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            // QuestList
            UIList.DataProvider questListDP = new UIList.DataProvider();
            foreach (string questName in dp.QuestNames)
            {
                questListDP.Elements.Add(new IconTextBase.DataProvider() { Text = questName });
            }
            QuestList.Publish(questListDP);

            // Info
            QuestInfoContainer.gameObject.SetActive(dp.SelectedQuest.HasValue);
            if (dp.SelectedQuest.HasValue)
            {
                int questIdx = dp.SelectedQuest.Value;
                QuestInfoNameTxt.Publish(dp.QuestNames[questIdx]);
                QuestInfoObjectiveTxt.Publish(dp.QuestObjectives[questIdx]);
                QuestInfoDescriptionTxt.Publish(dp.QuestDescriptions[questIdx]);
            }
        }

        protected override void InitChildren()
        {
            ExitBtn.Init((int)ComponentId.Exit, this);
            QuestList.Init((int)ComponentId.QuestSelections, this);
        }
    }
}
