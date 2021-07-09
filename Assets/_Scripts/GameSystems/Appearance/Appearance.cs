using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace MAGE.GameSystems.Appearances
{
    [System.Serializable]
    class Appearance
    {
        public static int NO_ASSET = -1;

        public int AppearanceId = -1;
        public int LinkedAppearanceId = -1;

        public PortraitSpriteId BasePortraitSpriteId = PortraitSpriteId.INVALID;
        public PortraitSpriteId OverridePortraitSpriteId = PortraitSpriteId.INVALID;
        public PortraitSpriteId PortraitSpriteId { get { return OverridePortraitSpriteId != PortraitSpriteId.INVALID ? OverridePortraitSpriteId : BasePortraitSpriteId; } }

        public ApparelAssetId BaseOutfitType = ApparelAssetId.NONE;
        public ApparelAssetId OverrideOutfitType = ApparelAssetId.NONE;
        public ApparelAssetId OutfitType { get { return OverrideOutfitType != ApparelAssetId.NONE ? OverrideOutfitType : BaseOutfitType; } }

        public ApparelAssetId BaseLeftHeldAssetId = ApparelAssetId.NONE;
        public ApparelAssetId OverrideLeftHeldAssetId = ApparelAssetId.NONE;
        public ApparelAssetId LeftHeldAssetId { get { return OverrideLeftHeldAssetId != ApparelAssetId.NONE ? OverrideLeftHeldAssetId : BaseLeftHeldAssetId; } }

        public ApparelAssetId BaseRightHeldAssetId = ApparelAssetId.NONE;
        public ApparelAssetId OverrideRightHeldAssetId = ApparelAssetId.NONE;
        public ApparelAssetId RightHeldAssetId { get { return OverrideRightHeldAssetId != ApparelAssetId.NONE ? OverrideRightHeldAssetId : BaseRightHeldAssetId; } }

        public ApparelAssetId BaseRangedAssetId = ApparelAssetId.NONE;
        public ApparelAssetId OverrideRangedAssetId = ApparelAssetId.NONE;
        public ApparelAssetId RangedAssetId { get { return OverrideRangedAssetId != ApparelAssetId.NONE ? OverrideRangedAssetId : BaseRangedAssetId; } }

        public BodyType BodyType = BodyType.HumanoidMale;
        public SkinToneType SkinToneType = SkinToneType.Base;
        public HairType HairType = HairType.MaleBuzz;
        public FacialHairType FacialHairType = FacialHairType.None;
        public HairColor HairColor = HairColor.Brunette;
    }
}

