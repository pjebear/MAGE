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
using UnityEngine.EventSystems;

namespace MAGE.GameModes.Encounter
{
    class EncounterModule : GameModeBase
    {
        private string TAG = "EncounterModule";

        public bool RunDebug = false;

        public GameObject MapPrefab;

        private GameObject View;
        private AudioSource mAmbientSoundSource;
        private EncounterContainer mEncounterContainer;

        public static EncounterModel Model;
        public static CharacterDirector CharacterDirector;
        public static AuraDirector AuraDirector;
        public static AnimationDirector AnimationDirector;
        public static ActionDirector ActionDirector;
        public static MovementDirector MovementDirector;
        public static CameraDirector CameraDirector;
        public static ProjectileDirector ProjectileDirector;
        public static EffectSpawner EffectSpawner;
        public static MapControl MapControl;

        public static EncounterIntroViewControl IntroViewControl;
        public static EncounterUnitPlacementViewControl UnitPlacementViewControl;
        public static EncounterStatusControl StatusViewControl;
        public static MasterFlowControl MasterFlowControl;
        public static TurnFlowControl TurnFlowControl;

        public override void Init()
        {
            Model = new EncounterModel();

            Model.EncounterContext = new GameSystems.World.EncounterContext();

            CharacterDirector = GetComponent<CharacterDirector>();
            AnimationDirector = GetComponent<AnimationDirector>();
            ActionDirector = GetComponent<ActionDirector>();
            MovementDirector = GetComponent<MovementDirector>();
            AnimationDirector = GetComponent<AnimationDirector>();
            AuraDirector = GetComponent<AuraDirector>();
            ProjectileDirector = GetComponent<ProjectileDirector>();
            EffectSpawner = GetComponentInChildren<EffectSpawner>();

            IntroViewControl = GetComponent<EncounterIntroViewControl>();
            UnitPlacementViewControl = new EncounterUnitPlacementViewControl();
            StatusViewControl = GetComponent<EncounterStatusControl>();
            TurnFlowControl = GetComponent<TurnFlowControl>();
            MasterFlowControl = GetComponent<MasterFlowControl>();
        }

        public override GameSystems.LevelId GetLevelId()
        {
            EncounterScenarioId encounterId = WorldService.Get().GetEncounterParams().ScenarioId;
            LevelId levelId = LevelManagementService.Get().GetEncounterInfo((int)encounterId).LevelId;

            return levelId;
        }

        protected override void SetupMode()
        {
            Logger.Log(LogTag.GameModes, TAG, "::SetupMode()");

            // Level Manager:
            //TODO: Load level if needed

            MAGE.GameModes.ILevelManagerService levelManagerService = MAGE.GameModes.LevelManagementService.Get();
            Level level = levelManagerService.GetLoadedLevel();
            level.ToggleTreeColliders(false);

            List<EncounterContainer> activeEncounters = level.EncounterContainer.GetComponentsInChildren<EncounterContainer>().Where(x => x.IsEncounterPending).ToList();
            if (activeEncounters.Count > 0)
            {
                mEncounterContainer = activeEncounters[0];
                mEncounterContainer.StartEncounter();
                MapControl = new MapControl();
                MapControl.Initialize(level.TileContainerGenerator.InitializeContainer(mEncounterContainer.Tiles));

                { // Allies
                    TeamSide teamSide = TeamSide.AllyHuman;
                    Model.Teams.Add(teamSide, new List<Character>());
                    foreach (ActorSpawner actorSpawner in mEncounterContainer.AlliesContainer.GetComponentsInChildren<ActorSpawner>())
                    {
                        int characterId = actorSpawner.CharacterPicker.GetActorId();
                        Character character = MAGE.GameSystems.CharacterService.Get().GetCharacter(characterId);
                        character.TeamSide = teamSide;

                        TileControl closestTile = MapControl.GetClosestTileTo(actorSpawner.transform.position);
                        Orientation startingOrientation = OrientationUtil.FromVector(actorSpawner.transform.forward);

                        CharacterDirector.AddCharacter(character, new CharacterPosition(closestTile.Idx, startingOrientation));
                        actorSpawner.gameObject.SetActive(false);
                    }

                    Model.EncounterContext.MaxAllyUnits = mEncounterContainer.MaxUserPlayers;
                }

                { // Enemies
                    TeamSide teamSide = TeamSide.EnemyAI;
                    Model.Teams.Add(teamSide, new List<Character>());
                    foreach (ActorSpawner actorSpawner in mEncounterContainer.EnemiesContainer.GetComponentsInChildren<ActorSpawner>())
                    {
                        int characterId = actorSpawner.CharacterPicker.GetActorId();
                        Character character = MAGE.GameSystems.CharacterService.Get().GetCharacter(characterId);
                        character.TeamSide = teamSide;

                        TileControl closestTile = MapControl.GetClosestTileTo(actorSpawner.transform.position);
                        Orientation startingOrientation = OrientationUtil.FromVector(actorSpawner.transform.forward);

                        CharacterDirector.AddCharacter(character, new CharacterPosition(closestTile.Idx, startingOrientation));
                        actorSpawner.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                Debug.Assert(false);
            }

            if (Camera.main.GetComponent<CameraDirector>() == null)
            {
                Camera.main.gameObject.AddComponent<CameraDirector>();
            }

            CameraDirector = Camera.main.GetComponent<CameraDirector>();
            Camera.main.gameObject.AddComponent<AudioListener>();

            AuraDirector.Init();
            ActionDirector.Init();
            MovementDirector.Init();
            ProjectileDirector.Init();
            MasterFlowControl.Init();
            TurnFlowControl.Init();
            StatusViewControl.Init();
            IntroViewControl.Init();
            UnitPlacementViewControl.Init();

            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeSetup_Complete));
        }

        protected override void CleanUpMode()
        {
            MAGE.GameModes.ILevelManagerService levelManagerService = MAGE.GameModes.LevelManagementService.Get();
            if (mEncounterContainer.EncounterScenarioId == EncounterScenarioId.Random)
            {
                Destroy(mEncounterContainer.gameObject);
            }
            else
            {
                EncounterInfo info = levelManagerService.GetEncounterInfo((int)mEncounterContainer.EncounterScenarioId);
                info.IsActive = false;
                levelManagerService.UpdateEncounterInfo(info);
            }
            Level level = levelManagerService.GetLoadedLevel();
            level.ToggleTreeColliders(true);
            //MapControl.Cleanup();
            Destroy(CameraDirector);
            Destroy(Camera.main.GetComponent<AudioListener>());
            CharacterDirector.CleanupCharacters();

            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeTakedown_Complete));
        }

        protected override void StartMode()
        {
            mAmbientSoundSource = gameObject.AddComponent<AudioSource>();
            mAmbientSoundSource.clip = GameModesModule.AudioManager.GetTrack(TrackId.Encounter);
            mAmbientSoundSource.loop = true;
            mAmbientSoundSource.spatialBlend = 0; // global volume
            //mAmbientSoundSource.Play();
            //GameModesModule.AudioManager.FadeInTrack(mAmbientSoundSource, 10, .5f);

            Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.EncounterBegun));
        }

        protected override void EndMode()
        {
            EncounterResultInfo resultInfo = new EncounterResultInfo();
            resultInfo.DidUserWin = Model.EncounterState == EncounterState.Win;
            resultInfo.PlayersInEncounter = new Teams();
            foreach (var teamList in Model.Teams)
            {
                foreach (Character character in teamList.Value)
                {
                    resultInfo.PlayersInEncounter[teamList.Key].Add(character.Id);
                }
            }

            MAGE.GameSystems.WorldService.Get().UpdateOnEncounterEnd(resultInfo);
        }

        public override GameModeType GetGameModeType()
        {
            return GameModeType.Encounter;
        }

        protected void CalculateSpawnPoints()
        {
            Model.AllySpawnPoints = new List<TileIdx>();
            for (int y = 0; y < 2; ++y)
            {
                for (int x = 0; x < MapControl.Map.Width; ++x)
                {
                    Model.AllySpawnPoints.Add(new TileIdx(x, y));
                }
            }

            Model.EnemySpawnPoints = new List<TileIdx>();
            for (int y = 0; y < 2; ++y)
            {
                for (int x = 0; x < MapControl.Map.Width; ++x)
                {
                    Model.EnemySpawnPoints.Add(new TileIdx(x, MapControl.Map.Length - 1 - y));
                }
            }
        }
    }
}


