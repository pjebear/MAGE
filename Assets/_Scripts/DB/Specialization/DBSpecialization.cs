using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    [System.Serializable]
    class DBSpecialization : DBEntryBase
    {
        public int SpecializationType;
        public List<int> TalentIds = new List<int>();
        public List<int> Proficiencies = new List<int>();
        public List<int> ActionIds = new List<int>();
        public List<DBAttribute> LevelUpModifiers = new List<DBAttribute>();

        public override void Copy(DBEntryBase _from, DBEntryBase _to)
        {
            DBSpecialization from = _from as DBSpecialization;
            DBSpecialization to = _to as DBSpecialization;

            to.SpecializationType = from.SpecializationType;
            to.TalentIds = new List<int>(from.TalentIds);
            to.Proficiencies = new List<int>(from.Proficiencies);
            to.ActionIds = new List<int>(from.ActionIds);

            to.LevelUpModifiers.Clear();
            foreach (DBAttribute attribute in from.LevelUpModifiers)
            {
                DBAttribute toAttribute = new DBAttribute();
                toAttribute.Set(attribute);
                to.LevelUpModifiers.Add(toAttribute);
            }
        }
    }   
}


