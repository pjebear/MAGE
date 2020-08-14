using MAGE.GameModes.SceneElements;
using MAGE.GameServices;
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


                    DBService.Get().WritePropInfo(dBPropInfo.Id, dBPropInfo);

                    DBAppearance npcAppearance = new DBAppearance();
                    npcAppearance.Id = (int)propId;
                    npcAppearance.PortraitSpriteId = (int)PortraitSpriteId.Vendor;
                    npcAppearance.BodyType = (int)BodyType.Body_0;
                    npcAppearance.ArmorPrefabId = (int)AppearancePrefabId.Cloth_0;

                    DBService.Get().WriteAppearance(dBPropInfo.Id, npcAppearance);
                }
            }

            DBService.Get().UpdatePropDB();
            DBService.Get().UpdateAppearanceDB();
        }
    }
}



