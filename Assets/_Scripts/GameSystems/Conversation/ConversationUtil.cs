using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices
{
    static class ConversationUtil
    {
        public static Conversation FromDB(DB.DBConversation dbConversation)
        {
            Conversation fromDB = new Conversation();

            fromDB.ConversationId = (ConversationId)dbConversation.Id;
            fromDB.Header = dbConversation.Header;
            fromDB.SpeakerIds = new List<int>(dbConversation.Members);

            foreach (DB.DBDialogue dbDialogue in dbConversation.Conversation)
            {
                Dialogue dialogue = new Dialogue();

                dialogue.SpeakerIdx = dbDialogue.SpeakerIdx;
                dialogue.Content = dbDialogue.Content;

                fromDB.DialogueChain.Add(dialogue);
            }

            return fromDB;
        }
    }
}



