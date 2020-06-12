using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class ConversationDBLoader
{
    public static void LoadDB()
    {
        List<int> defaultMembers = new List<int>() { ConversationConstants.CONVERSATION_OWNER_ID, ConversationConstants.PARTY_AVATAR_ID };
        int ownerIdx = 0;
        int partyAvatarIdx = 1;

        {  // Bandits in the hills
            ConversationId conversationId = ConversationId.BanditsInTheHills;

            DB.DBConversation conversation = new DB.DBConversation();
            conversation.Id = (int)conversationId;
            conversation.Name = conversationId.ToString();
            conversation.Members = defaultMembers;

            conversation.Header = "There's unrest in these hills...";
            conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "Ware these hills m'Lord, bandits have krept in since the army moved to the borders" });
            conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "Bandits? Are the rangers not patrolling this area anymore?" });
            conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "It's not for me to say sire, but best keep your eyes sharp." });

            DB.DBHelper.WriteConversation((int)conversationId, conversation);
        }

        {  // Lothar In Trouble
            ConversationId conversationId = ConversationId.LotharInTrouble;

            DB.DBConversation conversation = new DB.DBConversation();
            conversation.Id = (int)conversationId;
            conversation.Name = conversationId.ToString();
            conversation.Members = new List<int>() { ConversationConstants.PARTY_AVATAR_ID, (int)StoryCharacterId.Lothar, CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 0), CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 1) };
            int lotharIdx = 1;
            int banditLeader = 2;
            int banditLackey = 3;

            conversation.Header = "";
            conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = lotharIdx, Content = "Let's not make any hasty moves here gentlemen" });
            conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = banditLeader, Content = "How's about you make nice and hand us over your gold and we can all be on our way" });
            conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = lotharIdx, Content = "See now that would be a problem, because I worked hard taking this money from vagrands like you!" });
            conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = banditLeader, Content = "You little..." });
            conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = banditLackey, Content = "Boss! Sheriff lookin fella just walked up" });
            conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = banditLeader, Content = "Good, we can take his gold while we're at it. GET EM BOYS!" });
            conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = lotharIdx, Content = "Stranger! Lend me a hand why don't you? My employer will surely reward you" });

            DB.DBHelper.WriteConversation((int)conversationId, conversation);
        }

        DB.DBHelper.UpdateConversationDB();
    }
}

