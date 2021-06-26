using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class CharacterHoverInspector : UIContainer
    {
        private string TAG = "CharacterHoverInspector";

        public enum ComponentId
        {

        }

        public class DataProvider : IDataProvider
        {
            public Optional<bool> IsAlly;
            public Optional<string> PortraitAsset;
            public Optional<string> Name;
            public Optional<string> Specialization;
            public Optional<int> Level;
            public Optional<int> CurrentHP;
            public Optional<int> MaxHP;
            public List<StatusIcon.DataProvider> StatusEffects = new List<StatusIcon.DataProvider>();
        }

        public Image NameBacking;
        public UIImage HeadImg;
        public UIText NameTxt;
        public UIText LevelTxt;
        public UIImage SpecializationImg;
        public UIBar HPBar;
        public UIGrid StatusEffects;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            // Info Backing
            if (dp.IsAlly.HasValue) NameBacking.color = (dp.IsAlly.Value ? Color.blue : Color.red) * 0.5f;

            // Portrait
            if (dp.PortraitAsset.HasValue) HeadImg.Publish(dp.PortraitAsset.Value);

            // Name
            if (dp.Name.HasValue) NameTxt.Publish(dp.Name.Value);

            // Level
            if (dp.Level.HasValue) LevelTxt.Publish(string.Format("Lv.{0}", dp.Level.Value));

            // Specialization
            if (dp.Specialization.HasValue) SpecializationImg.Publish("Professions", dp.Specialization.Value);

            // HP
            if (dp.CurrentHP.HasValue && dp.MaxHP.HasValue) HPBar.Publish("HP", dp.CurrentHP.Value, dp.MaxHP.Value);

            // StatusEffects
            UIGrid.DataProvider statusEffectsDp = new UIGrid.DataProvider();
            foreach (StatusIcon.DataProvider statusEffectDp in dp.StatusEffects)
            {
                statusEffectsDp.Elements.Add(statusEffectDp);
            }
            StatusEffects.Publish(statusEffectsDp);
        }

        protected override void InitChildren()
        {
            // empty
        }
    }
}


