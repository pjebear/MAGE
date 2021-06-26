using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class TurnOrderCharacterPanel : UIContainer
    {
        private string TAG = "TurnOrderCharacterPanel";

        public enum ComponentId
        {

        }

        public class DataProvider : IDataProvider
        {
            public Optional<bool> IsCurrentTurn;
            public Optional<bool> IsAlly;
            public Optional<string> PortraitAsset;
            public Optional<int> CurrentHP;
            public Optional<int> MaxHP;
        }

        public Image BackingImg;
        public Image IsCurrentTurnImg;
        public UIImage HeadImg;
        public UIBar HPBar;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            // Info Backing
            if (dp.IsAlly.HasValue) BackingImg.color = (dp.IsAlly.Value ? Color.blue : Color.red) * 0.5f;

            // Current Turn
            IsCurrentTurnImg.gameObject.SetActive(dp.IsCurrentTurn.ValueOr(false));

            // Portrait
            if (dp.PortraitAsset.HasValue) HeadImg.Publish(dp.PortraitAsset.Value);

            // HP
            if (dp.CurrentHP.HasValue && dp.MaxHP.HasValue) HPBar.Publish("HP", dp.CurrentHP.Value, dp.MaxHP.Value);
        }

        protected override void InitChildren()
        {
            // empty
        }
    }
}


