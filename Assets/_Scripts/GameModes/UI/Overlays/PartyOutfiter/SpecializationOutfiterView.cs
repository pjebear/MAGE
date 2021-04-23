using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.Events;

namespace MAGE.UI.Views
{
    class SpecializationOutfiterView
        : UIContainer
    {
        public enum ComponentId
        {
            ResetTalentsBtn,
            TalentBtns,
            SpecializationBtns,
        }

        public class TalentDP
        {
            public string TalentName;
            public int AssignedPoints;
            public int MaxPoints;
            public bool IsSelectable;
        }

        public class DataProvider : IDataProvider
        {
            public int AvailableTalentPts;
            public string SpecializationName;
            public List<TalentDP> TalentDPs = new List<TalentDP>();
            public List<UIButton.DataProvider> SpecializationDPS = new List<UIButton.DataProvider>();
        }

        public UIList SpecializationBtns;
        public UIList TalentBtns;
        public UIText AvailablePointsTxt;
        public UIText SpecializationTxt;
        public UIButton ResetTalentsBtn;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = dataProvider as DataProvider;

            { // Talent btns
                AvailablePointsTxt.Publish(new UIText.DataProvider(dp.AvailableTalentPts.ToString()));
                List<IDataProvider> talentStrings = new List<IDataProvider>();
                foreach (TalentDP talentDP in dp.TalentDPs)
                {
                    talentStrings.Add(new UIButton.DataProvider(string.Format("{0} [{1}/{2}]", talentDP.TalentName, talentDP.AssignedPoints, talentDP.MaxPoints), talentDP.IsSelectable));
                }

                TalentBtns.Publish(new UIList.DataProvider(talentStrings));
            }

            { // Specialization btns
                SpecializationTxt.Publish(new UIText.DataProvider(dp.SpecializationName));
                SpecializationBtns.Publish(new UIList.DataProvider(dp.SpecializationDPS.Select(x => x as IDataProvider).ToList()));
            }
        }

        protected override void InitChildren()
        {
            TalentBtns.Init((int)ComponentId.TalentBtns, this);
            SpecializationBtns.Init((int)ComponentId.SpecializationBtns, this);
            ResetTalentsBtn.Init((int)ComponentId.ResetTalentsBtn, this);
        }
    }


}
