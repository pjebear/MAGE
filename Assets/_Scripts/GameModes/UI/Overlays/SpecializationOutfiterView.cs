using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using UnityEngine.Events;

class SpecializationOutfiterView
    : UIContainer
{
    public enum ComponentId
    {
        ResetTalentsBtn,
        TalentBtns
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
    }

    public UIButtonList TalentBtns;
    public UIText AvailablePointsTxt;
    public UIText SpecializationTxt;
    public UIButton ResetTalentsBtn;

    public override void Publish(IDataProvider dataProvider)
    {
        DataProvider dp = dataProvider as DataProvider;

        { // Talent btns
            List<UIButton.DataProvider> talentStrings = new List<UIButton.DataProvider>();
            foreach (TalentDP talentDP in dp.TalentDPs)
            {
                talentStrings.Add(new UIButton.DataProvider(string.Format("{0} [{1}/{2}]", talentDP.TalentName, talentDP.AssignedPoints, talentDP.MaxPoints), talentDP.IsSelectable));
            }

            UIButtonList.DataProvider buttonListDP = new UIButtonList.DataProvider(talentStrings);
            TalentBtns.Publish(buttonListDP);
        }

        AvailablePointsTxt.Publish(new UIText.DataProvider(dp.SpecializationName));
        AvailablePointsTxt.Publish(new UIText.DataProvider(dp.AvailableTalentPts.ToString()));
        SpecializationTxt.Publish(new UIText.DataProvider(dp.SpecializationName));
    }

    protected override void InitComponents()
    {
        TalentBtns.Init((int)ComponentId.TalentBtns, this);
        ResetTalentsBtn.Init((int)ComponentId.ResetTalentsBtn, this);
    }

    protected override void InitSelf()
    {
        mId = (int)UIContainerId.SpecializationOutfiterView;
        mContainerName = UIContainerId.SpecializationOutfiterView.ToString();
    }

    protected override IUIInteractionInfo ModifyInteractionInfo(IUIInteractionInfo interactionInfo)
    {
        return interactionInfo;
    }
}

