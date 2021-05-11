using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class PropDBLoader
    {
        public static void LoadDB()
        {
            Appearance guardAppearance = new Appearance()
            {
                BasePortraitSpriteId = PortraitSpriteId.Guard_0,
                BodyType = BodyType.HumanoidMale,
                FacialHairType = FacialHairType.ShortBeard,
                HairColor = HairColor.Brunette,
                HairType = HairType.MaleShort,
                SkinToneType = SkinToneType.Base,
                BaseOutfitType = ApparelAssetId.Mail_0,
                BaseLeftHeldAssetId = ApparelAssetId.Shield_0,
                BaseRightHeldAssetId = ApparelAssetId.Sword_0
            };

            Appearance vendorAppearance = new Appearance()
            {
                BasePortraitSpriteId = PortraitSpriteId.Vendor,
                BodyType = BodyType.HumanoidMale,
                FacialHairType = FacialHairType.LongBeard,
                HairColor = HairColor.Grey,
                HairType = HairType.MaleLong,
                SkinToneType = SkinToneType.Dark,
                BaseOutfitType = ApparelAssetId.Cloth_0
            };
            
            // Containers
            {
                { // Field Container
                    ContainerPropId propId = ContainerPropId.FieldVendorContainer;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.IsInteractable = false;
                    dBPropInfo.IsActive = true;
                    dBPropInfo.Inventory = new List<int>() {};

                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);
                }
            }

            // NPC's
            {
                { // Field Vendor
                    NPCPropId propId = NPCPropId.FieldVendor;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.AppearanceId = (int)propId;
                    dBPropInfo.IsInteractable = true;
                    dBPropInfo.IsActive = true;
                    dBPropInfo.Conversations = new List<int>() { };
                    dBPropInfo.Inventory = new List<int>()
                    {
                        (int)EquippableId.Axe_0
                        ,(int)EquippableId.Sword_0
                        ,(int)EquippableId.Bow_0
                        ,(int)EquippableId.Mace_0
                        ,(int)EquippableId.ClothArmor_0
                        ,(int)EquippableId.LeatherArmor_0
                        ,(int)EquippableId.ChainArmor_0
                        ,(int)EquippableId.PlateArmor_0
                    };

                    DBService.Get().WriteAppearance(dBPropInfo.AppearanceId, AppearanceUtil.ToDB(vendorAppearance));
                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);
                }

                // Demo Level
                { // Field Vendor
                    NPCPropId propId = NPCPropId.FieldVendor;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.AppearanceId = (int)propId;
                    dBPropInfo.IsInteractable = true;
                    dBPropInfo.IsActive = true;
                    dBPropInfo.Conversations = new List<int>() { };
                    dBPropInfo.Inventory = new List<int>()
                    {
                        (int)EquippableId.Axe_0
                        ,(int)EquippableId.Sword_0
                        ,(int)EquippableId.Bow_0
                        ,(int)EquippableId.Mace_0
                        ,(int)EquippableId.ClothArmor_0
                        ,(int)EquippableId.LeatherArmor_0
                        ,(int)EquippableId.ChainArmor_0
                        ,(int)EquippableId.PlateArmor_0
                    };

                    DBService.Get().WriteAppearance(dBPropInfo.AppearanceId, AppearanceUtil.ToDB(vendorAppearance));
                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);
                }

                { // Guard 0
                    NPCPropId propId = NPCPropId.DemoLevel_GateGuard0;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.AppearanceId = dBPropInfo.Id;
                    dBPropInfo.IsInteractable = false;
                    dBPropInfo.IsActive = true;

                    DBService.Get().WriteAppearance(dBPropInfo.AppearanceId, AppearanceUtil.ToDB(guardAppearance));
                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);
                }

                { // Guard 1
                    NPCPropId propId = NPCPropId.DemoLevel_GateGuard1;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.AppearanceId = (int)propId;
                    dBPropInfo.IsInteractable = false;
                    dBPropInfo.IsActive = true;

                    DBService.Get().WriteAppearance(dBPropInfo.AppearanceId, AppearanceUtil.ToDB(guardAppearance));
                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);
                }

                { // Guild Leader
                    NPCPropId propId = NPCPropId.DemoLevel_GuildLeader;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.AppearanceId = (int)propId;
                    dBPropInfo.IsInteractable = false;
                    dBPropInfo.IsActive = true;

                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);

                    Appearance appearance = new Appearance()
                    {
                        AppearanceId = (int)propId,
                        BasePortraitSpriteId = PortraitSpriteId.GuildLeader,
                        BodyType = BodyType.HumanoidMale,
                        FacialHairType = FacialHairType.None,
                        HairColor = HairColor.Dark,
                        HairType = HairType.MaleShort,
                        SkinToneType = SkinToneType.Tan,
                        BaseOutfitType = ApparelAssetId.Cloth_0
                    };

                    DBService.Get().WriteAppearance(appearance.AppearanceId, AppearanceUtil.ToDB(appearance));
                }

                { // Magistrate
                    NPCPropId propId = NPCPropId.DemoLevel_Magistrate;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.AppearanceId = (int)propId;
                    dBPropInfo.IsInteractable = false;
                    dBPropInfo.IsActive = true;

                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);

                    Appearance appearance = new Appearance()
                    {
                        AppearanceId = (int)propId,
                        BasePortraitSpriteId = PortraitSpriteId.Magistrate,
                        BodyType = BodyType.HumanoidMale,
                        FacialHairType = FacialHairType.None,
                        HairColor = HairColor.Blonde,
                        HairType = HairType.MaleBuzz,
                        SkinToneType = SkinToneType.Base,
                        BaseOutfitType = ApparelAssetId.Cloth_0
                    };

                    DBService.Get().WriteAppearance(appearance.AppearanceId, AppearanceUtil.ToDB(appearance));
                }

                { // Balgrid
                    NPCPropId propId = NPCPropId.DemoLevel_Captain;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.AppearanceId = (int)StoryCharacterId.Balgrid;
                    dBPropInfo.IsInteractable = false;
                    dBPropInfo.IsActive = true;

                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);
                }

                { // Front Gate
                    DoorPropId propId = DoorPropId.DemoLevel_TownGateFront;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.IsInteractable = true;
                    dBPropInfo.IsActive = true;
                    dBPropInfo.Inventory = new List<int>() { };
                    dBPropInfo.State = 0;

                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);
                }

                { // Black Smith
                    NPCPropId propId = NPCPropId.DemoLevel_BlackSmith;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.AppearanceId = (int)propId;
                    dBPropInfo.IsInteractable = true;
                    dBPropInfo.IsActive = true;
                    dBPropInfo.Conversations = new List<int>() { };
                    dBPropInfo.Inventory = new List<int>()
                    {
                        (int)EquippableId.Axe_0
                        ,(int)EquippableId.Sword_0
                        ,(int)EquippableId.Bow_0
                        ,(int)EquippableId.Mace_0
                        ,(int)EquippableId.ClothArmor_0
                        ,(int)EquippableId.LeatherArmor_0
                        ,(int)EquippableId.ChainArmor_0
                        ,(int)EquippableId.PlateArmor_0
                    };

                    DBService.Get().WriteAppearance(dBPropInfo.AppearanceId, AppearanceUtil.ToDB(vendorAppearance));
                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);
                }
            }

            DBService.Get().UpdatePropDB();
            DBService.Get().UpdateAppearanceDB();
        }
    }
}



