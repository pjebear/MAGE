using Invector.vCharacterController;
using MAGE.GameModes.FlowControl;
using MAGE.GameModes.LevelManagement;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

namespace MAGE.GameModes
{
    class ExplorationModule : GameModeBase
    {
        public static ExplorationModule Instance;

        GameObject mExplorationAvatar;
        ExplorationMenuViewControl MenuControl;
        InteractionFlowControl mInteractionFlowControl;
        ScenarioFlowControl mScenarioFlowControl;
        AudioSource mAmbientSoundSource;

        public MovementDirector MovementDirector;

        public override void Init()
        {
            Instance = this;

            MenuControl = new ExplorationMenuViewControl();
            mInteractionFlowControl = GetComponent<InteractionFlowControl>();
            mScenarioFlowControl = GetComponent<ScenarioFlowControl>();
            MovementDirector = gameObject.AddComponent<MovementDirector>();
        }

        public override GameSystems.LevelId GetLevelId()
        {
            return MAGE.GameSystems.WorldService.Get().GetCurrentLevel();
        }

        protected override void SetupMode()
        {
            SceneElements.Level level = LevelManagementService.Get().GetLoadedLevel();

            level.ScenarioContainer.gameObject.SetActive(true);
            level.NPCContainer.gameObject.SetActive(true);

            int partyAvatarId = MAGE.GameSystems.WorldService.Get().GetPartyAvatarId();

            // SSFTODO: Update this to go through levelmanagement
            Appearance appearance = CharacterService.Get().GetCharacter(partyAvatarId).GetAppearance();

            mExplorationAvatar = GameModesModule.ActorLoader.CreateActor(appearance, level.SpawnPoint).gameObject;
            //go.AddComponent<vThirdPersonMotor>();
            //go.AddComponent<vThirdPersonController>();
            //go.AddComponent<vThirdPersonInput>();
            //mExplorationAvatar = Instantiate(ExplorationAvatarPrefab, level.transform);
            //mExplorationAvatar.transform.SetPositionAndRotation(level.SpawnPoint.position, level.SpawnPoint.rotation);

            Camera.main.gameObject.AddComponent<ThirdPersonCamera>().Follow(mExplorationAvatar.transform);

            mExplorationAvatar.AddComponent<AudioListener>();
            mExplorationAvatar.AddComponent<ThirdPersonActorController>();

            mAmbientSoundSource = gameObject.AddComponent<AudioSource>();
            mAmbientSoundSource.clip = GameModesModule.AudioManager.GetTrack(TrackId.Explore);
            mAmbientSoundSource.loop = true;
            mAmbientSoundSource.spatialBlend = 0; // global volume
                                                  //mAmbientSoundSource.Play();
            GameModesModule.AudioManager.FadeInTrack(mAmbientSoundSource, 5, .5f);

            mInteractionFlowControl.Init(mExplorationAvatar.GetComponent<ThirdPersonActorController>());
            mScenarioFlowControl.Init(mExplorationAvatar.GetComponent<ThirdPersonActorController>());

            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeSetup_Complete));
        }

        protected override void CleanUpMode()
        {
            mInteractionFlowControl.CleanUp();
            mScenarioFlowControl.CleanUp();

            Destroy(Camera.main.gameObject.GetComponent<ThirdPersonCamera>());
            Destroy(mExplorationAvatar);

            MAGE.GameModes.LevelManagementService.Get().GetLoadedLevel().ScenarioContainer.gameObject.SetActive(false);
            MAGE.GameModes.LevelManagementService.Get().GetLoadedLevel().NPCContainer.gameObject.SetActive(false);

            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeTakedown_Complete));

            Instance = null;
        }

        protected override void StartMode()
        {
            MenuControl.Show();
        }

        protected override void EndMode()
        {
            MenuControl.Hide();
        }

        public override GameModeType GetGameModeType()
        {
            return GameModeType.Exploration;
        }

        public void TriggerRandomEncounter()
        {
            Vector3 avatarPosition = mExplorationAvatar.transform.position;

            LevelManagementService.Get().GetLoadedLevel().GenerateTilesAtPosition(mExplorationAvatar.transform);

            EncounterCreateParams randomParams = new EncounterCreateParams();
            randomParams.ScenarioId = EncounterScenarioId.Random;
            randomParams.LevelId = MAGE.GameModes.LevelManagementService.Get().GetLoadedLevel().LevelId;
            MAGE.GameSystems.WorldService.Get().PrepareEncounter(randomParams);

            GameModesModule.Instance.Encounter();
        }
    }
}


