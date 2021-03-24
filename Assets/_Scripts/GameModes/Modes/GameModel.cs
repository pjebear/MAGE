using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes
{
    static class GameModel
    {
        public static Exploration.ExplorationModel Exploration = new Exploration.ExplorationModel();
        public static Encounter.EncounterModel Encounter = new Encounter.EncounterModel();
    }
}
