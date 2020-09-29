using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.AI
{
    class TurnProposal
    {
        public List<TileIdx> MovementProposal = new List<TileIdx>();

        public ActionId ActionId = ActionId.INVALID;
        public TileIdx CastTile;
        public TargetSelection ActionTarget;
        public float Value;
    }
}
