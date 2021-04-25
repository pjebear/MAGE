using MAGE.GameSystems;
using MAGE.Utility.Enums;
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
        [SerializeField] private string _Destination;
        public LevelId Destination { get { return EnumUtil.StringToEnum<LevelId>(_Destination); } }
        public int SpawnPoint = 0;

        protected override int GetLayer()
        {
            return (int)Layer.Default;
        }

        protected override void HandleTriggerEntered(ThirdPersonActorController entered)
        {
            LevelId destination = Destination;
            Debug.Assert(destination != LevelId.INVALID);
            if (destination != LevelId.INVALID)
            {
                GameSystems.World.PartyLocation newLocation = new GameSystems.World.PartyLocation();
                newLocation.SetLevel(destination);
                newLocation.SetPosition(SpawnPoint);

                Exploration.ExplorationMessage travelMessage = new Exploration.ExplorationMessage(Exploration.ExplorationMessage.EventType.TravelTriggered, newLocation);
                Messaging.MessageRouter.Instance.NotifyMessage(travelMessage);
            }
        }
    }
}
