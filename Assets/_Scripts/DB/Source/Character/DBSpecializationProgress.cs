using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBTalentProgress
    {
        public int TalentId;
        public int AssignedPoints;
    }

    [System.Serializable]
    class DBSpecializationProgress : Internal.DBEntryBase
    {
        public int SpecializationType = 0;
        public int Experience = 0;
        public int Level = 0;
        public List<DBTalentProgress> Talents = new List<DBTalentProgress>();

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBSpecializationProgress from = _from as DBSpecializationProgress;
            DBSpecializationProgress to = _to as DBSpecializationProgress;

            to.SpecializationType = from.SpecializationType;
            to.Level = from.Level;
            to.Experience = from.Experience;

            to.Talents.Clear();
            foreach (DBTalentProgress talent in from.Talents)
            {
                to.Talents.Add(new DBTalentProgress() { AssignedPoints = talent.AssignedPoints, TalentId = talent.TalentId });
            }
        }
    }
}


