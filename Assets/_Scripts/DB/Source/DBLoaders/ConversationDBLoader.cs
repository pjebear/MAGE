using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
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
                conversation.Members = new List<int>() { ConversationConstants.PARTY_AVATAR_ID, (int)MAGE.GameSystems.Characters.StoryCharacterId.Lothar,
                MAGE.GameSystems.Characters.CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 0),
                MAGE.GameSystems.Characters.CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 1) };
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

            { // Demo Story Line
                {  // Lay down the law
                    ConversationId conversationId = ConversationId.Demo_LayDownTheLaw;

                    DB.DBConversation conversation = new DB.DBConversation();
                    conversation.Id = (int)conversationId;
                    conversation.Name = conversationId.ToString();
                    conversation.Members = defaultMembers;

                    conversation.Header = "Trouble with the guilds";
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "Do you see what I have to deal with here? I can't be held responsible for this fiasco" });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "Everything that happens in this town is your reponsibility as a Magistrate of the curch" });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "Now see here..." });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "No you see, your people are your flock and your arrogance has lead them to suffer. By the Saint I should strip you of your title now." });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "You wouldn't..!" });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "Oh I would. The only thing staying my hand right now is the faith that as a man of the Saint you can come to reconciliation and make ammends to your flock." });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "I'll be leaving shortly to deal with this bandit issue, I expect a change of heart by my return" });

                    DBService.Get().WriteConversation((int)conversationId, conversation);
                }

                {  // Training Time
                    ConversationId conversationId = ConversationId.Demo_TrainingTime;

                    DB.DBConversation conversation = new DB.DBConversation();
                    conversation.Id = (int)conversationId;
                    conversation.Name = conversationId.ToString();
                    conversation.Members = defaultMembers;

                    conversation.Header = "Traing the recruits";
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "The look on your face says that went as well as I could hope." });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "If the Saint doesn't help him get head out of his ass, you can be sure I will." });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "Ever the diplomat" });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "One of my many charming qualities. How are the recruits looking?" });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "You can still see the chalk marks on their armor, but they seem sharp enough. Ready to give them hell?" });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "Nothing would please me more" });

                    DBService.Get().WriteConversation((int)conversationId, conversation);
                }

                {  // Training Complete
                    ConversationId conversationId = ConversationId.Demo_TrainingComplete;

                    DB.DBConversation conversation = new DB.DBConversation();
                    conversation.Id = (int)conversationId;
                    conversation.Name = conversationId.ToString();
                    conversation.Members = defaultMembers;

                    conversation.Header = "Better than I thought";
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "Not bad for the bottom of the barrel. They may prove to be worthy of the Saint yet" });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "Let's just hope they can march after the beating you put them through" });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = partyAvatarIdx, Content = "Get them packed up Balgrid, it's time we go test the enemies metal" });
                    conversation.Conversation.Add(new DB.DBDialogue() { SpeakerIdx = ownerIdx, Content = "Yes sir!" });

                    DBService.Get().WriteConversation((int)conversationId, conversation);
                }
            }

            DBService.Get().UpdateConversationDB();
        }
    }
}



