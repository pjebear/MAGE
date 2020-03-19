using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    class DBCharacter
    {
        public int Id = -1;
        public DBCharacterInfo CharacterInfo = new DBCharacterInfo();
        public DBEquipment Equipment = new DBEquipment();
        public DBSpecializations Specializations = new DBSpecializations();
    }
}
