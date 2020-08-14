using MAGE.GameServices;
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
            }

            DBService.Get().UpdateItemDB();
        }
    }
}



