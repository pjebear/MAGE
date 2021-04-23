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
            fromDB.PortraitSpriteId         = (PortraitSpriteId)dbAppearance.PortraitSpriteId;
            fromDB.BodyType                 = (BodyType)dbAppearance.BodyType;
            fromDB.OutfitType               = (ApparelAssetId)dbAppearance.OutfitType;
            fromDB.SkinToneType             = (SkinToneType)dbAppearance.SkinToneType;
            fromDB.HairType                 = (HairType)dbAppearance.HairType;
            fromDB.FacialHairType           = (FacialHairType)dbAppearance.FacialHairType;
            fromDB.HairColor                = (HairColor)dbAppearance.HairColor;
            fromDB.LeftHeldAssetId          = (ApparelAssetId)dbAppearance.LeftHeldAssetId;
            fromDB.RightHeldAssetId         = (ApparelAssetId)dbAppearance.RightHeldAssetId;

            return fromDB;
        }

        public static DB.DBAppearance ToDB(Appearance appearance)
        {
            DB.DBAppearance toDB = new DB.DBAppearance();

            toDB.Id = (int)appearance.AppearanceId;
            toDB.PortraitSpriteId = (int)appearance.PortraitSpriteId;
            toDB.BodyType = (int)appearance.BodyType;
            toDB.OutfitType = (int)appearance.OutfitType;
            toDB.SkinToneType = (int)appearance.SkinToneType;
            toDB.HairType = (int)appearance.HairType;
            toDB.FacialHairType = (int)appearance.FacialHairType;
            toDB.HairColor = (int)appearance.HairColor;
            toDB.LeftHeldAssetId = (int)appearance.LeftHeldAssetId;
            toDB.RightHeldAssetId = (int)appearance.RightHeldAssetId;

            return toDB;
        }
    }
}
