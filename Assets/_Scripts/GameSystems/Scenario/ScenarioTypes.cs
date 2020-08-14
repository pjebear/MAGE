using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices
{
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
        public List<Character.CharacterInfo> ScenarioCharacters = new List<MAGE.GameServices.Character.CharacterInfo>();
    }
}




