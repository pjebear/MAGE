using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

