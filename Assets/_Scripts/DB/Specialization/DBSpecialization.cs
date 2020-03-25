using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    [System.Serializable]
    class SpecializationInfo
    {
        public int Experience = 0;
        public int Level = 0;
        public int TalentPoints;
        public List<int> SpentTalentPoints = new List<int>();

        public override string ToString()
        {
            string toString = "Level: " + Level + " Exp:" + Experience + " TalentPoints: " + TalentPoints + " Points: ";

            foreach (int spentPoints in SpentTalentPoints)
            {
                toString += spentPoints + ",";
            }

            toString.Remove(toString.Length - 1);

            return toString;
        }
    }

    [System.Serializable]
    class DBSpecializations : DBEntryBase
    {
        public SpecializationInfo[] Specializations = new SpecializationInfo[(int)SpecializationType.NUM];

        public DBSpecializations()
        {
            for (int i = 0; i < (int)SpecializationType.NUM; ++i)
            {
                Specializations[i] = new SpecializationInfo();
            }
        }

        public override string ToString()
        {
            string toString = "\nSpecializations:";

            foreach (SpecializationInfo info in Specializations)
            {
                toString += "\n" + info.ToString();
            }

            return toString;
        }

        public override void Copy(DBEntryBase _from, DBEntryBase _to)
        {
            DBSpecializations from = _from as DBSpecializations;
            DBSpecializations to = _to as DBSpecializations;

            for (int i = 0; i < (int)SpecializationType.NUM; ++i)
            {
                to.Specializations[i].Level = from.Specializations[i].Level;
                to.Specializations[i].Experience = from.Specializations[i].Experience;
                to.Specializations[i].TalentPoints = from.Specializations[i].TalentPoints;
                to.Specializations[i].SpentTalentPoints.Clear();
                to.Specializations[i].SpentTalentPoints.AddRange(from.Specializations[i].SpentTalentPoints);
            }
        }
    }
}


