using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Appearances
{
    static class AppearanceUtil
    {
        // Appearance
        public static Appearance FromDB(DB.DBAppearance dbAppearance)
        {
            Appearance fromDB = new Appearance();

            fromDB.AppearanceId             = dbAppearance.Id;

            fromDB.BasePortraitSpriteId         = (PortraitSpriteId)dbAppearance.BasePortraitSpriteId;
            fromDB.OverridePortraitSpriteId         = (PortraitSpriteId)dbAppearance.OverridePortraitSpriteId;

            fromDB.BaseOutfitType = (ApparelAssetId)dbAppearance.BaseOutfitType;
            fromDB.OverrideOutfitType = (ApparelAssetId)dbAppearance.OverrideOutfitType;

            fromDB.BaseLeftHeldAssetId = (ApparelAssetId)dbAppearance.BaseLeftHeldAssetId;
            fromDB.OverrideLeftHeldAssetId = (ApparelAssetId)dbAppearance.OverrideLeftHeldAssetId;

            fromDB.BaseRightHeldAssetId = (ApparelAssetId)dbAppearance.BaseRightHeldAssetId;
            fromDB.OverrideRightHeldAssetId = (ApparelAssetId)dbAppearance.OverrideRightHeldAssetId;

            fromDB.BodyType                 = (BodyType)dbAppearance.BodyType;
            fromDB.SkinToneType             = (SkinToneType)dbAppearance.SkinToneType;
            fromDB.HairType                 = (HairType)dbAppearance.HairType;
            fromDB.FacialHairType           = (FacialHairType)dbAppearance.FacialHairType;
            fromDB.HairColor                = (HairColor)dbAppearance.HairColor;

            return fromDB;
        }

        public static DB.DBAppearance ToDB(Appearance appearance)
        {
            DB.DBAppearance toDB = new DB.DBAppearance();

            toDB.Id = (int)appearance.AppearanceId;

            toDB.BasePortraitSpriteId = (int)appearance.BasePortraitSpriteId;
            toDB.OverridePortraitSpriteId = (int)appearance.OverridePortraitSpriteId;

            toDB.BaseOutfitType = (int)appearance.BaseOutfitType;
            toDB.OverrideOutfitType = (int)appearance.OverrideOutfitType;

            toDB.BaseLeftHeldAssetId = (int)appearance.BaseLeftHeldAssetId;
            toDB.OverrideLeftHeldAssetId = (int)appearance.OverrideLeftHeldAssetId;

            toDB.BaseRightHeldAssetId = (int)appearance.BaseRightHeldAssetId;
            toDB.OverrideRightHeldAssetId = (int)appearance.OverrideRightHeldAssetId;

            toDB.BodyType = (int)appearance.BodyType;
            toDB.SkinToneType = (int)appearance.SkinToneType;
            toDB.HairType = (int)appearance.HairType;
            toDB.FacialHairType = (int)appearance.FacialHairType;
            toDB.HairColor = (int)appearance.HairColor;
            

            return toDB;
        }
    }
}
