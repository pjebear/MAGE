using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    namespace Character
    {
        [System.Serializable]
        class Talent
        {
            public int TalentId;
            public int AssignedPoints;
        }

        [System.Serializable]
        class DBSpecializationInfo : DBEntryBase
        {
            public int SpecializationType = 0;
            public int Experience = 0;
            public int Level = 0;
            public List<Talent> Talents = new List<Talent>();

            public override void Copy(DBEntryBase _from, DBEntryBase _to)
            {
                DBSpecializationInfo from = _from as DBSpecializationInfo;
                DBSpecializationInfo to = _to as DBSpecializationInfo;

                to.SpecializationType = from.SpecializationType;
                to.Level = from.Level;
                to.Experience = from.Experience;

                to.Talents.Clear();
                foreach (Talent talent in from.Talents)
                {
                    to.Talents.Add(new Talent() { AssignedPoints = talent.AssignedPoints, TalentId = talent.TalentId });
                }
            }
        }
    }
}


