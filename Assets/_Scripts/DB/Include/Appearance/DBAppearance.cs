using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBAppearance : Internal.DBEntryBase
    {
        public int Id = -1;
        public int PortraitSpriteId = -1;
        public int BodyType = -1;
        public int ArmorPrefabId = -1;
        public int HeldLeftPrefabId = -1;
        public int HeldRightPrefabId = -1;

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBAppearance from = _from as DBAppearance;
            DBAppearance to = _to as DBAppearance;

            to.Id = from.Id;
            to.PortraitSpriteId = from.PortraitSpriteId;
            to.BodyType = from.BodyType;
            to.ArmorPrefabId = from.ArmorPrefabId;
            to.HeldLeftPrefabId = from.HeldLeftPrefabId;
            to.HeldRightPrefabId = from.HeldRightPrefabId;
        }
    }
}
