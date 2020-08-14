using MAGE.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
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
                conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "It's not for me to say sire. We've locked the gate until someone can deal with them" });
                conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "I cannot countenance filth like this running free under my watch, we'll root them out" });
                conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "Many thanks m'Lord. I've stashed the key just over there if you can grab it for me" });

                DBService.Get().WriteConversation((int)conversationId, conversation);
            }

            {  // Bandits in the hills
                ConversationId conversationId = ConversationId.UnlockTheGate;

                DB.DBConversation conversation = new DB.DBConversation();
                conversation.Id = (int)conversationId;
                conversation.Name = conversationId.ToString();
                conversation.Members = defaultMembers;

                conversation.Header = "Ah you have my key...";
                conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "Good you found my key. Can never trust my memory that I put it in the right place" });
                conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "Are you sure you want to venture out into such peril" });
                conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "It is my duty to safe guard the children of the profit. I have not choice in the matter" });
                conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "May the prophet keep you safe on your quest" });

                DBService.Get().WriteConversation((int)conversationId, conversation);
            }

            {  // Lothar In Trouble
                ConversationId conversationId = ConversationId.LotharInTrouble;

                DB.DBConversation conversation = new DB.DBConversation();
                conversation.Id = (int)conversationId;
                conversation.Name = conversationId.ToString();
                conversation.Members = new List<int>() { ConversationConstants.PARTY_AVATAR_ID, (int)MAGE.GameServices.Character.StoryCharacterId.Lothar,
                MAGE.GameServices.Character.CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 0),
                MAGE.GameServices.Character.CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 1) };
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

                DBService.Get().WriteConversation((int)conversationId, conversation);
            }

            DBService.Get().UpdateConversationDB();
        }
    }
}



