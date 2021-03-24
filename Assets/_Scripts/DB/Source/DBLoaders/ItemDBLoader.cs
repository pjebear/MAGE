using MAGE.GameSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class ItemDBLoader
    {
        public static void LoadDB()
        {
            // StoryItems
            {
                { // First Quest Item
                    DBItem dbItem = new DBItem();
                    StoryItemId storyItemId = StoryItemId.GateKey;
                    dbItem.Id = (int)storyItemId;
                    dbItem.Name = storyItemId.ToString();
                    dbItem.SpriteId = (int)UI.ItemIconSpriteId.QuestItem;

                    DBService.Get().WriteItem((int)storyItemId, dbItem);
                }

                { // Quest: Clearing out the bears
                    DBItem dbItem = new DBItem();
                    StoryItemId storyItemId = StoryItemId.DEMO_GoldenBearPelt;
                    dbItem.Id = (int)storyItemId;
                    dbItem.Name = storyItemId.ToString();
                    dbItem.SpriteId = (int)UI.ItemIconSpriteId.GoldenFur;

                    DBService.Get().WriteItem((int)storyItemId, dbItem);
                }
            }

            // Vendor Items
            {
                { // Bear Pelt
                    DBItem dbItem = new DBItem();
                    VendorItemId itemId = VendorItemId.DEMO_BearPelt;
                    dbItem.Id = (int)itemId;
                    dbItem.Name = itemId.ToString();
                    dbItem.SpriteId = (int)UI.ItemIconSpriteId.Fur;
                    dbItem.Value = 50;

                    DBService.Get().WriteItem((int)itemId, dbItem);
                }

                { // Bear Claws
                    DBItem dbItem = new DBItem();
                    VendorItemId itemId = VendorItemId.DEMO_BearClaw;
                    dbItem.Id = (int)itemId;
                    dbItem.Name = itemId.ToString();
                    dbItem.SpriteId = (int)UI.ItemIconSpriteId.Claws;
                    dbItem.Value = 5;

                    DBService.Get().WriteItem((int)itemId, dbItem);
                }
            }

            DBService.Get().UpdateItemDB();
        }
    }
}



