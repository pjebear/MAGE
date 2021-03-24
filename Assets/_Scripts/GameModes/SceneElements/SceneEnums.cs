using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.SceneElements
{
    enum Layer
    {
        Default,
        Tile = 8,
        Interactible,
        Terrain,
        Actor,
        Scenario,
        PostProcessing,
        Trees,
        Mob,
        Navigation,

        NUM

    }
}
