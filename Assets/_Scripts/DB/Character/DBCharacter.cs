using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    [System.Serializable]
    class DBCharacter : DBEntryBase
    {
        public int Id = -1;
        public Character.DBCharacterInfo CharacterInfo = new Character.DBCharacterInfo();
        public List<int> Equipment = new List<int>();
        public List<Character.DBSpecializationInfo> Specializations = new List<Character.DBSpecializationInfo>();

        public override void Copy(DBEntryBase _from, DBEntryBase _to)
        {
            DBCharacter from = _from as DBCharacter;
            DBCharacter to = _to as DBCharacter;

            to.Id = from.Id;

            from.CharacterInfo.CopyTo(to.CharacterInfo);

            to.Equipment = new List<int>(from.Equipment);

            to.Specializations.Clear();
            foreach (Character.DBSpecializationInfo dBSpecializationInfo in from.Specializations)
            {
                Character.DBSpecializationInfo toAdd = new Character.DBSpecializationInfo();
                dBSpecializationInfo.CopyTo(toAdd);
                to.Specializations.Add(toAdd);
            }
        }
    }
}
