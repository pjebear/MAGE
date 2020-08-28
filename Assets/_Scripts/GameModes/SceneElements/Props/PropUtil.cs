using MAGE.GameSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.SceneElements
{
    static class PropUtil
    {
        public static PropType PropTypeFromId(int id)
        {
            PropType propType = PropType.Environment;

            if (id >= PropConstants.NPC_ID_OFFSET)
            {
                propType = PropType.NPC;
            }
            else if (id >= PropConstants.CONTAINER_ID_OFFSET)
            {
                propType = PropType.Container;
            }
            else if (id >= PropConstants.ENVIRONMENT_ID_OFFSET)
            {
                propType = PropType.Environment;
            }

            return propType;
        }

        public static PropInfo FromDB(DB.DBPropInfo dbInfo)
        {
            PropInfo fromDB = new PropInfo();

            fromDB.Tag = new PropTag(dbInfo.Id);
            fromDB.Name = dbInfo.Name;
            fromDB.AppearanceId = dbInfo.AppearanceId;
            fromDB.IsActive = dbInfo.IsActive;
            fromDB.IsInteractible = dbInfo.IsInteractable;
            fromDB.Currency = dbInfo.Currency;
            
            foreach (int itemId in dbInfo.Inventory)
            {
                fromDB.Inventory.Add(GameSystems.ItemFactory.LoadItem(itemId));
            }

            foreach (int conversationId in dbInfo.Conversations)
            {
                fromDB.Conversations.Add(GameSystems.WorldService.Get().GetConversation((GameSystems.ConversationId)conversationId));
            }

            return fromDB;
        }

        public static DB.DBPropInfo ToDB(PropInfo propInfo)
        {
            DB.DBPropInfo toDB = new DB.DBPropInfo();

            toDB.Id = propInfo.Tag.Id;
            toDB.Name = propInfo.Name;
            toDB.AppearanceId = propInfo.AppearanceId;
            toDB.IsActive = propInfo.IsActive;
            toDB.IsInteractable = propInfo.IsInteractible;
            toDB.Currency = propInfo.Currency;

            foreach (Item item in propInfo.Inventory)
            {
                toDB.Inventory.Add(item.ItemTag.ItemId);
            }

            foreach (Conversation conversation in propInfo.Conversations)
            {
                toDB.Conversations.Add((int)conversation.ConversationId);
            }

            return toDB;
        }
    }
}
