using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBCharacter : Internal.DBEntryBase
    {
        public int Id = -1;
        public int AppearanceId = -1;
        public DBCharacterInfo CharacterInfo = new DBCharacterInfo();
        public List<int> Equipment = new List<int>();
        public List<DBSpecializationProgress> Specializations = new List<DBSpecializationProgress>();

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBCharacter from = _from as DBCharacter;
            DBCharacter to = _to as DBCharacter;

            to.Id = from.Id;
            to.AppearanceId = from.AppearanceId;

            from.CharacterInfo.CopyTo(to.CharacterInfo);
            to.Equipment = new List<int>(from.Equipment);

            to.Specializations.Clear();
            foreach (DBSpecializationProgress DBSpecializationProgress in from.Specializations)
            {
                DBSpecializationProgress toAdd = new DBSpecializationProgress();
                DBSpecializationProgress.CopyTo(toAdd);
                to.Specializations.Add(toAdd);
            }
        }
    }
}
