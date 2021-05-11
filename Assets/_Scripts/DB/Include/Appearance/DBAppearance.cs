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

        public int BasePortraitSpriteId = -1;
        public int OverridePortraitSpriteId = -1;

        public int BaseOutfitType = -1;
        public int OverrideOutfitType = -1;

        public int BaseLeftHeldAssetId = -1;
        public int OverrideLeftHeldAssetId = -1;
        public int BaseRightHeldAssetId = -1;
        public int OverrideRightHeldAssetId = -1;

        public int BodyType = -1;
        public int SkinToneType = -1;
        public int HairType = -1;
        public int FacialHairType = -1;
        public int HairColor = -1;
       

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBAppearance from = _from as DBAppearance;
            DBAppearance to = _to as DBAppearance;

            to.Id = from.Id;
            to.BasePortraitSpriteId = from.BasePortraitSpriteId;
            to.OverridePortraitSpriteId = from.OverridePortraitSpriteId;

            to.BaseOutfitType = from.BaseOutfitType;
            to.OverrideOutfitType = from.OverrideOutfitType;

            to.BaseLeftHeldAssetId = from.BaseLeftHeldAssetId;
            to.OverrideLeftHeldAssetId = from.OverrideLeftHeldAssetId;

            to.BaseRightHeldAssetId = from.BaseRightHeldAssetId;
            to.OverrideRightHeldAssetId = from.OverrideRightHeldAssetId;

            to.BodyType = from.BodyType;
            
            to.SkinToneType = from.SkinToneType;
            to.HairType = from.HairType;
            to.FacialHairType = from.FacialHairType;
            to.HairColor = from.HairColor;

        }
    }
}
