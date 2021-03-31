using MAGE.GameSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class TravelTrigger : TriggerVolumeBase<ThirdPersonActorController>
    {
        public LevelId LevelId = LevelId.INVALID;
        public int SpawnPoint = 0;

        protected override int GetLayer()
        {
            return (int)Layer.Default;
        }

        protected override void HandleTriggerEntered(ThirdPersonActorController entered)
        {
            Debug.Assert(LevelId != LevelId.INVALID);
            if (LevelId != LevelId.INVALID)
            {
                GameSystems.World.PartyLocation newLocation = new GameSystems.World.PartyLocation();
                newLocation.SetLevel(LevelId);
                newLocation.SetPosition(SpawnPoint);

                Exploration.ExplorationMessage travelMessage = new Exploration.ExplorationMessage(Exploration.ExplorationMessage.EventType.TravelTriggered, newLocation);
                Messaging.MessageRouter.Instance.NotifyMessage(travelMessage);
            }
        }
    }
}
