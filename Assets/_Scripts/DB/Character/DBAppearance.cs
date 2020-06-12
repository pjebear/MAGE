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
        class DBAppearance : DBEntryBase
        {
            public int OwnerId = -1;
            public string Owner = "NONE";
            public int PortraitSpriteId = -1;
            public int BodyType = -1;
            public int ArmorPrefabId = -1;
            public int HeldLeftPrefabId = -1;
            public int HeldRightPrefabId = -1;

            public override void Copy(DBEntryBase _from, DBEntryBase _to)
            {
                DBAppearance from = _from as DBAppearance;
                DBAppearance to = _to as DBAppearance;

                to.OwnerId = from.OwnerId;
                to.Owner = from.Owner;
                to.PortraitSpriteId = from.PortraitSpriteId;
                to.BodyType = from.BodyType;
                to.ArmorPrefabId = from.ArmorPrefabId;
                to.HeldLeftPrefabId = from.HeldLeftPrefabId;
                to.HeldRightPrefabId = from.HeldRightPrefabId;
            }
        }
    }
}
