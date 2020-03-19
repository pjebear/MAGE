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
        public CharacterInfo CharacterInfo = new CharacterInfo();
        public EquipmentInfo EquipmentInfo = new EquipmentInfo();
        public SpecializationsInfo SpecializationsInfo = new SpecializationsInfo();
    }
}
