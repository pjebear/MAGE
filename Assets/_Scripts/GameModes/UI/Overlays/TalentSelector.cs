using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//class TalentSelector : UIContainer
//{
//    public class TalentData : UIButton.DataProvider
//    {
//        public int AssignedPoints;
//        public int MaxPoints;
//        public string Name;
//        public string Description;

//        public override string ToString()
//        {
//            return string.Format("{0}: [{1}/{2}]", Name, AssignedPoints, MaxPoints);
//        }
//    }

//    public class DataProvider : IDataProvider
//    {
//        public List<TalentData> TalentDataList;

//        public DataProvider(List<TalentData> talentDataList)
//        {
//            TalentDataList = talentDataList;
//        }

//        public override string ToString()
//        {
//            string toString = "Talents: ";
//            foreach (TalentData talentData in TalentDataList)
//            {
//                toString += " | " + talentData.ToString(); 
//            }

//            return toString;
//        }
//    }

//    public UIButtonList TalentList;

//    public override void Publish(IDataProvider dataProvider)
//    {
//        DataProvider dp = (DataProvider)dataProvider;

//        TalentList.Publish(dp.ButtonListDP);
//    }

//    protected override void InitComponents()
//    {
//        ButtonList.Init((int)ComponentId.ButtonList, this);
//    }

//    protected override void InitSelf()
//    {
//        mId = (int)UIContainerId.ActorActionsOverlay;
//        mContainerName = "ActorActions";
//    }

//    protected override IUIInteractionInfo ModifyInteractionInfo(IUIInteractionInfo interactionInfo)
//    {
//        return interactionInfo;
//    }
//}
