using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBStoryArcInfo : Internal.DBEntryBase
    {
        public int Id = -1;
        public string Name = "INVALID";
        public DBStoryObjective ActivationCondition = new DBStoryObjective();
        public List<DBStoryNodeInfo> StoryArc = new List<DBStoryNodeInfo>();

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBStoryArcInfo from = _from as DBStoryArcInfo;
            DBStoryArcInfo to = _to as DBStoryArcInfo;

            to.Id = from.Id;
            to.Name = from.Name;

            from.ActivationCondition.CopyTo(to.ActivationCondition);

            to.StoryArc = new List<DBStoryNodeInfo>();
            foreach (DBStoryNodeInfo dBStoryNodeInfo in from.StoryArc)
            {
                DBStoryNodeInfo copy = new DBStoryNodeInfo();
                dBStoryNodeInfo.CopyTo(copy);
                to.StoryArc.Add(copy);
            }
        }
    }
}
