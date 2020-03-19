using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    class SpecializationInfo
    {
        public int Experience = 0;
        public int Level = 0;
        public List<int> SpentTalentPoints = new List<int>();

        public override string ToString()
        {
            string toString = "Level: " + Level + " Exp:" + Experience + " Spent Points: ";

            foreach (int spentPoints in SpentTalentPoints)
            {
                toString += spentPoints + ",";
            }

            toString.Remove(toString.Length - 1);

            return toString;
        }
    }

    class SpecializationsInfo
    {
        public SpecializationInfo[] Specializations = new SpecializationInfo[(int)SpecializationType.NUM];

        public SpecializationsInfo()
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
    }

    class DBSpecializations : DBEntryBase<SpecializationsInfo>
    {
        public override void Copy(SpecializationsInfo from, SpecializationsInfo to)
        {
            for (int i = 0; i < (int)SpecializationType.NUM; ++i)
            {
                to.Specializations[i].Level = from.Specializations[i].Level;
                to.Specializations[i].Experience = from.Specializations[i].Experience;
                to.Specializations[i].SpentTalentPoints.Clear();
                to.Specializations[i].SpentTalentPoints.AddRange(from.Specializations[i].SpentTalentPoints);
            }
        }
    }
}


