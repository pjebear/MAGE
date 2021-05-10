using UnityEngine;
using System.Collections;

namespace MAGE.GameSystems.Appearances
{
    //enum OutfitArrangementType
    //{
    //    NONE = -1,

    //    Default,
    //    Cloth_0,
    //    Leather_0,
    //    Mail_0,
    //    Plate_0,

    //    NUM
    //}

    enum PortraitSpriteId
    {
        INVALID = -1,

        // Custom Heads
        Rheinhardt,
        Asmund,
        Balgrid,
        Lothar,
        Maric,

        // Professions
        Adept,
        Archer,
        Bear,
        Footman,
        Monk,

        // NPCs
        BanditLeader,
        Bandit_0,
        Guard_0,
        Guard_1,
        GuildLeader,
        Magistrate,
        Vendor,

        NUM
    }

    enum ApparelAssetId
    {
        NONE = -1,

        // Weapons
        Dagger_0,
        Sword_0,
        Axe_0,
        Mace_0,
        BattleAxe_0,
        BastardSword_0,
        Maul_0,
        Bow_0,
        LongBow_0,
        Crossbow_0,
        Staff_0,
        Rod_0,

        // Shields
        Shield_0,
        TowerShield_0,

        // Armor
        Default,
        Cloth_0,
        Leather_0,
        Mail_0,
        Plate_0,

        NUM
    }

    public enum OutfitColorization
    {
        Allied,
        Enemy,

        NUM
    }

    public enum HairType
    {
        MaleBuzz,
        MaleLong,
        MaleShort,

        NUM
    }

    public enum FacialHairType
    {
        None,
        ShortBeard,
        LongBeard,

        NUM
    }

    public enum HairColor
    {
        Brunette,
        Blonde,
        Dark,
        Red,
        Grey,
        White,

        NUM
    }

    enum SkinToneType
    {
        Base,
        Tan,
        Dark,
        Pale,
        Elf,
        Yellow,

        NUM
    }
}
