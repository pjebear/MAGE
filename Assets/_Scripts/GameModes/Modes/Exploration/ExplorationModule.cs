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
    class ExplorationModule 
        : FlowControl.FlowControlBase
    {
        private AudioSource mAmbientSoundSource;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Exploration;
        }

        protected override void Setup()
        {
            ExplorationModel.Instance = new ExplorationModel();

            ExplorationModel.Instance.MovementDirector = gameObject.AddComponent<MovementDirector>();

            SceneElements.Level level = LevelManagementService.Get().GetLoadedLevel();

            int partyAvatarId = MAGE.GameSystems.WorldService.Get().GetPartyAvatarId();
            GameSystems.World.PartyLocation partyLocation = WorldService.Get().GetPartyLocation();

            // SSFTODO: Update this to go through levelmanagement
            Appearance appearance = CharacterService.Get().GetCharacter(partyAvatarId).GetAppearance();

            GameObject partyAvatar = ActorLoader.Instance.CreateActor(appearance, level.transform).gameObject;
            partyAvatar.AddComponent<AudioListener>();
            ExplorationModel.Instance.PartyAvatar = partyAvatar.AddComponent<ThirdPersonActorController>();

            if (partyLocation.PositionType == GameSystems.World.PartyPositionType.SpawnPoint)
            {
                Transform spawnPoint = level.GetSpawnPoint(partyLocation.SpawnPoint);
                partyAvatar.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            }
            else if (partyLocation.PositionType == GameSystems.World.PartyPositionType.Position)
            {
                partyAvatar.transform.position = partyLocation.Position;
            }
            
            Camera.main.gameObject.AddComponent<ThirdPersonCamera>().Follow(partyAvatar.transform);

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
            partyLocation.SetPosition(ExplorationModel.Instance.PartyAvatar.transform.position);
            GameSystems.WorldService.Get().UpdatePartyLocation(partyLocation);

            Destroy(Camera.main.gameObject.GetComponent<ThirdPersonCamera>());
            Destroy(ExplorationModel.Instance.PartyAvatar.gameObject);

            ExplorationModel.Instance = null;
        }

        public void TriggerRandomEncounter()
        {
            Vector3 avatarPosition = ExplorationModel.Instance.PartyAvatar.transform.position;

            LevelManagementService.Get().GetLoadedLevel().GenerateTilesAtPosition(ExplorationModel.Instance.PartyAvatar.transform);

            EncounterCreateParams randomParams = new EncounterCreateParams();
            randomParams.ScenarioId = EncounterScenarioId.Random;
            randomParams.LevelId = MAGE.GameModes.LevelManagementService.Get().GetLoadedLevel().LevelId;
            MAGE.GameSystems.WorldService.Get().PrepareEncounter(randomParams);
        }
    }
}


