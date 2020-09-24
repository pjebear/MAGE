using MAGE.GameModes.SceneElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Exploration
{
    class ExplorationModel
    {
        public static ExplorationModel Instance = null;

        public ThirdPersonActorController PartyAvatar;
        public PropBase InteractionTarget;
        public MovementDirector MovementDirector;
    }
}
