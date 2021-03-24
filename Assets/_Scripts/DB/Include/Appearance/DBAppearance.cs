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
        public int OutfitType = -1;
        public int SkinToneType = -1;
        public int HairType = -1;
        public int FacialHairType = -1;
        public int HairColor = -1;
        public int LeftHeldAssetId = -1;
        public int RightHeldAssetId = -1;

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBAppearance from = _from as DBAppearance;
            DBAppearance to = _to as DBAppearance;

            to.Id = from.Id;
            to.PortraitSpriteId = from.PortraitSpriteId;
            to.BodyType = from.BodyType;
            to.OutfitType = from.OutfitType;
            to.SkinToneType = from.SkinToneType;
            to.HairType = from.HairType;
            to.FacialHairType = from.FacialHairType;
            to.HairColor = from.HairColor;
            to.LeftHeldAssetId = from.LeftHeldAssetId;
            to.RightHeldAssetId = from.RightHeldAssetId;
        }
    }
}
