using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.AI
{
    class TurnProposal
    {
        public ActionId ActionId = ActionId.INVALID;
        public Vector3 Position;
        public TargetSelection ActionTarget;
        public float Value;
    }
}
