﻿using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
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
            int guardAppearanceId = 0;
            {
                DBAppearance dBAppearance = new DBAppearance();

                dBAppearance.Id = guardAppearanceId;
                dBAppearance.PortraitSpriteId = (int)PortraitSpriteId.Guard_0;
                dBAppearance.BodyType = (int)BodyType.Body_0;
                dBAppearance.ArmorPrefabId = (int)AppearancePrefabId.Chain_0;
                dBAppearance.HeldLeftPrefabId = (int)AppearancePrefabId.Shield_0;
                dBAppearance.HeldRightPrefabId = (int)AppearancePrefabId.Sword_0;

                DBService.Get().WriteAppearance(dBAppearance.Id, dBAppearance);
            }

            int vendorAppearanceId = 1;
            {
                DBAppearance dBAppearance = new DBAppearance();

                dBAppearance.Id = vendorAppearanceId;
                dBAppearance.PortraitSpriteId = (int)PortraitSpriteId.Vendor;
                dBAppearance.BodyType = (int)BodyType.Body_0;
                dBAppearance.ArmorPrefabId = (int)AppearancePrefabId.Cloth_1;

                DBService.Get().WriteAppearance(dBAppearance.Id, dBAppearance);
            }

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
                    dBPropInfo.AppearanceId = vendorAppearanceId;
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

                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);
                }

                // Demo Level
                { // Field Vendor
                    NPCPropId propId = NPCPropId.FieldVendor;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.AppearanceId = vendorAppearanceId;
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

                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);
                }

                { // Guard 0
                    NPCPropId propId = NPCPropId.DemoLevel_GateGuard0;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.AppearanceId = guardAppearanceId;
                    dBPropInfo.IsInteractable = false;
                    dBPropInfo.IsActive = true;

                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);
                }

                { // Guard 1
                    NPCPropId propId = NPCPropId.DemoLevel_GateGuard1;

                    DBPropInfo dBPropInfo = new DBPropInfo();
                    dBPropInfo.Id = (int)propId;
                    dBPropInfo.Name = propId.ToString();
                    dBPropInfo.AppearanceId = guardAppearanceId;
                    dBPropInfo.IsInteractable = false;
                    dBPropInfo.IsActive = true;

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

                    DBAppearance dBAppearance = new DBAppearance();

                    dBAppearance.Id = (int)propId;
                    dBAppearance.PortraitSpriteId = (int)PortraitSpriteId.GuildLeader;
                    dBAppearance.BodyType = (int)BodyType.Body_0;
                    dBAppearance.ArmorPrefabId = (int)AppearancePrefabId.Cloth_1;

                    DBService.Get().WriteAppearance(dBAppearance.Id, dBAppearance);
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

                    DBAppearance dBAppearance = new DBAppearance();

                    dBAppearance.Id = (int)propId;
                    dBAppearance.PortraitSpriteId = (int)PortraitSpriteId.Magistrate;
                    dBAppearance.BodyType = (int)BodyType.Body_0;
                    dBAppearance.ArmorPrefabId = (int)AppearancePrefabId.Cloth_0;

                    DBService.Get().WriteAppearance(dBAppearance.Id, dBAppearance);
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
            }

            DBService.Get().UpdatePropDB();
            DBService.Get().UpdateAppearanceDB();
        }
    }
}


