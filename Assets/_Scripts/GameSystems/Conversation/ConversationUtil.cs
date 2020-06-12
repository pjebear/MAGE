using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class ConversationUtil
{
    public static Conversation Load(ConversationId conversationId, int ownerId)
    {
        Conversation fromDB = new Conversation();

        DB.DBConversation dBConversation = DB.DBHelper.LoadConversation((int)conversationId);

        fromDB.ConversationId = conversationId;
        fromDB.Header = dBConversation.Header;

        foreach (int speakerId in dBConversation.Members)
        {
            int translatedId = speakerId;

            if (speakerId == ConversationConstants.CONVERSATION_OWNER_ID)
            {
                translatedId = ownerId;
            }
            else if (speakerId == ConversationConstants.PARTY_AVATAR_ID)
            {
                translatedId = (int)StoryCharacterId.Rheinhardt;
            }

            Logger.Assert(translatedId >= 0, LogTag.Assets, "ConversationUtil", string.Format("Invalid Speaker Id [{0}] found in conversation. [{1}]", translatedId, conversationId.ToString()));
            fromDB.SpeakerIds.Add(translatedId);
        }

        foreach (DB.DBDialogue dbDialogue in dBConversation.Conversation)
        {
            Dialogue dialogue = new Dialogue();

            dialogue.SpeakerIdx = dbDialogue.SpeakerIdx;
            dialogue.Content = dbDialogue.Content;

            fromDB.DialogueChain.Add(dialogue);
        }

        return fromDB;
    }
}

