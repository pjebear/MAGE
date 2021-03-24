using Invector.vCharacterController;
using MAGE.GameModes.Exploration;
using MAGE.GameModes.FlowControl;
using MAGE.GameModes.LevelManagement;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes
{
    class ExplorationFlowControl
        : FlowControl.FlowControlBase
    {
        private AudioSource mAmbientSoundSource;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Exploration;
        }

        protected override void Setup()
        {
            GameModel.Exploration.MovementDirector = gameObject.AddComponent<MovementDirector>();

            Level level = LevelManagementService.Get().GetLoadedLevel();

            Actor player = level.Player.GetComponent<Actor>();
            player.gameObject.SetActive(true);
            player.GetComponent<CharacterPickerControl>().CharacterPicker.RootCharacterId = WorldService.Get().GetPartyAvatarId();
            player.GetComponent<ActorSpawner>().Refresh();
            GameModel.Exploration.PartyAvatar = player;

            Camera.main.GetComponent<Cameras.CameraController>().SetTarget(player.transform, Cameras.CameraType.ThirdPerson);

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
            
            mAmbientSoundSource = gameObject.AddComponent<AudioSource>();
            mAmbientSoundSource.clip = AudioManager.Instance.GetTrack(TrackId.Explore);
            mAmbientSoundSource.loop = true;
            mAmbientSoundSource.spatialBlend = 0; // global volume
                                                  //mAmbientSoundSource.Play();
            AudioManager.Instance.FadeInTrack(mAmbientSoundSource, 5, .5f);
        }

        protected override void Cleanup()
        {
            GameSystems.World.PartyLocation partyLocation = GameSystems.WorldService.Get().GetPartyLocation();
            partyLocation.SetPosition(GameModel.Exploration.PartyAvatar.transform.position);
            GameSystems.WorldService.Get().UpdatePartyLocation(partyLocation);

            GameModel.Exploration.PartyAvatar.gameObject.SetActive(false);
            //GameModel.Exploration.PartyAvatar = null;
        }

        public void TriggerRandomEncounter()
        {
            Vector3 avatarPosition = GameModel.Exploration.PartyAvatar.transform.position;

            LevelManagementService.Get().GetLoadedLevel().GenerateTilesAtPosition(GameModel.Exploration.PartyAvatar.transform);

            EncounterCreateParams randomParams = new EncounterCreateParams();
            randomParams.ScenarioId = EncounterScenarioId.Random;
            randomParams.LevelId = MAGE.GameModes.LevelManagementService.Get().GetLoadedLevel().LevelId;
            MAGE.GameSystems.WorldService.Get().PrepareEncounter(randomParams);
        }
    }
}


