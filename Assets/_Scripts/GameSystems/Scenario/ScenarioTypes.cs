using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

enum ScenarioId
{
    INVALID = -1, 

    TheGreatHoldUp,

    NUM
}

class ScenarioInfo
{
    public ScenarioId ScenarioId = ScenarioId.INVALID;
    public ConversationId ConversationId = ConversationId.INVALID;
    public List<CharacterInfo> ScenarioCharacters = new List<CharacterInfo>();
}

