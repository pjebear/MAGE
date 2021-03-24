using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    static class ConversationConstants
    {
        public const int CONVERSATION_OWNER_ID = -2;
        public const int PARTY_AVATAR_ID = -3;
    }

    enum ConversationId
    {
        INVALID = -1,

        // StoryArc Debug
        BanditsInTheHills,
        UnlockTheGate,
        LotharInTrouble,

        // Demo Level
        Demo_LayDownTheLaw,
        Demo_TrainingTime,
        Demo_BlackSmithTraining_Start,
        Demo_BlackSmithTraining_Return,
        Demo_TrainingComplete,

        NUM
    }

    class Dialogue
    {
        public int SpeakerIdx = -1;
        public string Content = "";
    }

    class Conversation
    {
        public ConversationId ConversationId;
        public string Header = "";
        public List<int> SpeakerIds = new List<int>();
        public List<Dialogue> DialogueChain = new List<Dialogue>();
    }
}



