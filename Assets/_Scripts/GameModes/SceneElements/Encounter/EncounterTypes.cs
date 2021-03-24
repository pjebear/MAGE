using MAGE.GameSystems;
using MAGE.GameSystems.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.SceneElements
{
    [System.Serializable]
    class PlacementInfo
    {
        public PlacementRegion PlacementRegion;
    }

    enum PlacementRegion
    {
        Left,
        Right,
        Top,
        Bottom,

        NUM
    }
}
