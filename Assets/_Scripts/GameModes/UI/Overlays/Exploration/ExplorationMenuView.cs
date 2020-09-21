using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace MAGE.UI.Views
{
    class ExplorationMenuView
       : UIContainer
    {
        public enum ComponentId
        {
            OutfiterBtn,
            SaveBtn,
            ExitBtn,
            MapBtn,
            RandomEncounter,
            QuestLog,
        }

        public UIButton OutfiterBtn;
        public UIButton SaveBtn;
        public UIButton ExitBtn;
        public UIButton MapBtn;
        public UIButton QuestLog;
        public UIButton RandomEncounter;

        public override void Publish(IDataProvider dataProvider)
        {
            // empty
        }

        protected override void InitChildren()
        {
            OutfiterBtn.Init((int)ComponentId.OutfiterBtn, this);
            SaveBtn.Init((int)ComponentId.SaveBtn, this);
            ExitBtn.Init((int)ComponentId.ExitBtn, this);
            MapBtn.Init((int)ComponentId.MapBtn, this);
            QuestLog.Init((int)ComponentId.QuestLog, this);
            RandomEncounter.Init((int)ComponentId.RandomEncounter, this);
        }
    }

}

