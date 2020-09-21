using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems
{
    enum ScenarioId
    {
        INVALID = -1,

        TheGreatHoldUp,
        DemoScenario,

        NUM
    }

    class ScenarioInfo
    {
        public ScenarioId ScenarioId = ScenarioId.INVALID;
        public ConversationId ConversationId = ConversationId.INVALID;
        public List<Characters.CharacterInfo> ScenarioCharacters = new List<MAGE.GameSystems.Characters.CharacterInfo>();
    }
}




