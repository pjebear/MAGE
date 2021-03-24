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

        public PortraitSpriteId PortraitSpriteId = PortraitSpriteId.INVALID;

        public BodyType BodyType = BodyType.HumanoidMale;

        public ApparelAssetId OutfitType = ApparelAssetId.NONE;
        public SkinToneType SkinToneType = SkinToneType.Base;
        public HairType HairType = HairType.MaleBuzz;
        public FacialHairType FacialHairType = FacialHairType.None;
        public HairColor HairColor = HairColor.Brunette;
        public ApparelAssetId LeftHeldAssetId = ApparelAssetId.NONE;
        public ApparelAssetId RightHeldAssetId = ApparelAssetId.NONE;
    }
}

