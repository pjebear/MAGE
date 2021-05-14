using MAGE.GameModes.Exploration;
using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.Messaging;
using UnityEngine;

namespace MAGE.GameModes
{
    class ExplorationFlowControl
        : FlowControl.FlowControlBase
    {
        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Exploration;
        }

        protected override void Setup()
        {
            GameModel.Exploration.MovementDirector = gameObject.AddComponent<MovementDirector>();

            Level level = LevelManagementService.Get().GetLoadedLevel();

            Actor player = level.Player.GetComponent<Actor>();

            // Update location
            GameSystems.World.PartyLocation partyLocation = WorldService.Get().GetPartyLocation();
            if (partyLocation.PositionType == GameSystems.World.PartyPositionType.SpawnPoint)
            {
                Transform spawnPoint = level.GetSpawnPoint(partyLocation.SpawnPoint);
                player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            }
            else if (partyLocation.PositionType == GameSystems.World.PartyPositionType.Position)
            {
                player.transform.position = partyLocation.Position;
            }

            player.gameObject.SetActive(true);

            player.GetComponent<CharacterPickerControl>().CharacterId = WorldService.Get().GetPartyAvatarId();
            player.GetComponent<ActorSpawner>().Refresh();
            GameModel.Exploration.PartyAvatar = player;

            Camera.main.GetComponent<Cameras.CameraController>().SetTarget(player.transform, Cameras.CameraType.ThirdPerson);
        }

        protected override void Cleanup()
        {
            GameModel.Exploration.PartyAvatar.gameObject.SetActive(false);
        }

        public override void HandleMessage(MessageInfoBase eventInfoBase)
        {
            switch (eventInfoBase.MessageId)
            {
                case ExplorationMessage.Id:
                {
                    ExplorationMessage message = eventInfoBase as ExplorationMessage;
                    if (message.Type == ExplorationMessage.EventType.TravelTriggered)
                    {
                        WorldService.Get().UpdatePartyLocation(message.Arg<GameSystems.World.PartyLocation>());
                        SendFlowMessage("travel");
                    }
                }
                break;
            }
        }
    }
}


