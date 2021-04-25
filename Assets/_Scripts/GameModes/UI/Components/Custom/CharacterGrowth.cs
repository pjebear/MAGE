using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class CharacterGrowth : UIContainer
    {
        private string TAG = "CharacterGrowth";

        public enum ComponentId
        {

        }

        public enum DisplayStage
        {
            ExperienceGrowth,
            AttributeGrowth
        }

        public class EXPGrowth
        {
            public int Level;
            public int Current;
            public int Max;
            public int Growth;
        }

        public class StatGrowth
        {
            public int Current;
            public int Growth;
        }

        public class DataProvider : IDataProvider
        {
            public string PortraitAsset;
            public string Name;

            public EXPGrowth LevelGrowth = new EXPGrowth();

            public string Specialization;
            public EXPGrowth SpecializationGrowth = new EXPGrowth();

            public StatGrowth MightGrowth = new StatGrowth();
            public StatGrowth FineseGrowth = new StatGrowth();
            public StatGrowth MagicGrowth = new StatGrowth();
            public StatGrowth FortitudeGrowth = new StatGrowth();
            public StatGrowth AttunementGrowth = new StatGrowth();
        }

        public DataProvider mPayload;

        public GameObject ExperienceGrowthStage;
        public GameObject StatGrowthStage;

        public UIImage PortraitImg;
        public UIText NameTxt;
        public UIText LevelTxt;
        public UIText ExpTxt;
        public UIText ExpGrowthTxt;
        public UIText SpecializationLabelTxt;
        public UIText SpecializationLvlTxt;
        public UIText SpecializationExpTxt;
        public UIText SpecializationExpGrowthTxt;

        public UIText MightTxt;
        public UIText MightGrowthTxt;
        public UIText FinesseTxt;
        public UIText FinesseGrowthTxt;
        public UIText MagicTxt;
        public UIText MagicGrowthTxt;
        public UIText FortitudeTxt;
        public UIText FortitudeGrowthTxt;
        public UIText AttunementTxt;
        public UIText AttunementGrowthTxt;
        

        public override void Publish(IDataProvider dataProvider)
        {
            mPayload = (DataProvider)dataProvider;
            // Portrait
            PortraitImg.Publish(mPayload.PortraitAsset);

            // Name
            NameTxt.Publish(mPayload.Name);

            Invoke("PublishCurrentExperienceStage", 0);
            Invoke("PublishExperienceAppliedStage", 3f);
            Invoke("PublishStatGrowthStage", 6f);
        }

        protected override void InitChildren()
        {
            // empty
        }

        private void OnDestroy()
        {
            CancelInvoke();
        }

        protected void PublishCurrentExperienceStage()
        {
            StatGrowthStage.gameObject.SetActive(false);
            ExperienceGrowthStage.gameObject.SetActive(true);

            // Level
            LevelTxt.Publish(mPayload.LevelGrowth.Level.ToString());
            ExpTxt.Publish(string.Format("{0}/{1}", mPayload.LevelGrowth.Current, mPayload.LevelGrowth.Max));
            ExpGrowthTxt.Publish(string.Format("+{0}", mPayload.LevelGrowth.Growth));

            // Specialization
            SpecializationLabelTxt.Publish(mPayload.Specialization);
            SpecializationLvlTxt.Publish(mPayload.SpecializationGrowth.Level.ToString());
            SpecializationExpTxt.Publish(string.Format("{0}/{1}", mPayload.SpecializationGrowth.Current, mPayload.SpecializationGrowth.Max));
            SpecializationExpGrowthTxt.Publish(string.Format("+{0}", mPayload.SpecializationGrowth.Growth));
        }

        protected void PublishExperienceAppliedStage()
        {
            StatGrowthStage.gameObject.SetActive(false);
            ExperienceGrowthStage.gameObject.SetActive(true);

            // Level
            if ((mPayload.LevelGrowth.Current + mPayload.LevelGrowth.Growth) >= mPayload.LevelGrowth.Max)
            {
                LevelTxt.Publish("Level Up!");
                ExpTxt.Publish(string.Format("{0}/{1}", mPayload.LevelGrowth.Max, mPayload.LevelGrowth.Max));
            }
            else
            {
                ExpTxt.Publish(string.Format("{0}/{1}", mPayload.LevelGrowth.Current + mPayload.LevelGrowth.Growth, mPayload.LevelGrowth.Max));
            }
            ExpGrowthTxt.Publish("");

            // Specialization
            if ((mPayload.SpecializationGrowth.Current + mPayload.SpecializationGrowth.Growth) >= mPayload.SpecializationGrowth.Max)
            {
                SpecializationLvlTxt.Publish("Level Up!");
                SpecializationExpTxt.Publish(string.Format("{0}/{1}", mPayload.SpecializationGrowth.Max, mPayload.SpecializationGrowth.Max));
            }
            else
            {
                SpecializationExpTxt.Publish(string.Format("{0}/{1}", mPayload.SpecializationGrowth.Current + mPayload.SpecializationGrowth.Growth, mPayload.SpecializationGrowth.Max));
            }
            SpecializationExpGrowthTxt.Publish("");
        }

        protected void PublishStatGrowthStage()
        {
            ExperienceGrowthStage.gameObject.SetActive(false);
            StatGrowthStage.gameObject.SetActive(true);

            // Might
            MightTxt.Publish(mPayload.MightGrowth.Current.ToString());
            MightGrowthTxt.Publish(string.Format("+{0}", mPayload.MightGrowth.Growth.ToString()));

            // Finesse
            FinesseTxt.Publish(mPayload.FineseGrowth.Current.ToString());
            FinesseGrowthTxt.Publish(string.Format("+{0}", mPayload.FineseGrowth.Growth.ToString()));

            // Magic
            MagicTxt.Publish(mPayload.MagicGrowth.Current.ToString());
            MagicGrowthTxt.Publish(string.Format("+{0}", mPayload.MagicGrowth.Growth.ToString()));

            // Fort
            FortitudeTxt.Publish(mPayload.FortitudeGrowth.Current.ToString());
            FortitudeGrowthTxt.Publish(string.Format("+{0}", mPayload.FortitudeGrowth.Growth.ToString()));

            // Attunement
            AttunementTxt.Publish(mPayload.AttunementGrowth.Current.ToString());
            AttunementGrowthTxt.Publish(string.Format("+{0}", mPayload.AttunementGrowth.Growth.ToString()));
        }
    }
}


