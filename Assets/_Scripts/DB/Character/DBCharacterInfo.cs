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
        class DBCharacterInfo : DBEntryBase
        {
            public string Name = "EMPTY";
            public int Level = 1;
            public int Experience = 0;
            public List<DBAttributes> Attributes = new List<DBAttributes>();
            public int CurrentSpecialization;

            public override string ToString()
            {
                return "Name: " + Name;
            }

            public override void Copy(DBEntryBase _from, DBEntryBase _to)
            {
                DBCharacterInfo from = _from as DBCharacterInfo;
                DBCharacterInfo to = _to as DBCharacterInfo;

                to.Name = from.Name;
                to.Level = from.Level;
                to.Experience = from.Experience;
                to.Attributes.Clear();
                foreach (DBAttributes attributes in from.Attributes)
                {
                    DBAttributes copy = new DBAttributes();
                    copy.AttributeCategory = attributes.AttributeCategory;
                    copy.Attributes = new List<float>(attributes.Attributes);

                    to.Attributes.Add(copy);
                }

                to.CurrentSpecialization = from.CurrentSpecialization;
            }
        }
    }
}


