using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounter;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class EncounterFlowControl : FlowControlBase
    {
        private string TAG = "EncounterFlowControl";

        public bool RunDebug = false;

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
        public static AITurnControl AITurnControl;

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

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Encounter;
        }

        protected override void Setup()
        {
            Model = new EncounterModel();

            Model.EncounterContext = new GameSystems.World.EncounterContext();

            CharacterDirector = gameObject.AddComponent<CharacterDirector>();
            AnimationDirector = gameObject.AddComponent<AnimationDirector>();
            ActionDirector = gameObject.AddComponent<ActionDirector>();
            MovementDirector = gameObject.AddComponent<MovementDirector>();
            AnimationDirector = gameObject.AddComponent<AnimationDirector>();
            AuraDirector = gameObject.AddComponent<AuraDirector>();
            ProjectileDirector = gameObject.AddComponent<ProjectileDirector>();
            EffectSpawner = gameObject.AddComponent<EffectSpawner>();

            IntroViewControl = gameObject.AddComponent<EncounterIntroViewControl>();
            UnitPlacementViewControl = new EncounterUnitPlacementViewControl();
            StatusViewControl = gameObject.AddComponent<EncounterStatusControl>();
            TurnFlowControl = gameObject.AddComponent<TurnFlowControl>();
            AITurnControl = gameObject.AddComponent<AITurnControl>();
            MasterFlowControl = gameObject.AddComponent<MasterFlowControl>();

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
                    foreach (EncounterCharacterControl encounterCharacter in mEncounterContainer.AlliesContainer.GetComponentsInChildren<EncounterCharacterControl>())
                    {
                        int characterId = encounterCharacter.GetCharacterId();
                        Character character = MAGE.GameSystems.CharacterService.Get().GetCharacter(characterId);
                        character.TeamSide = teamSide;

                        TileControl closestTile = MapControl.GetClosestTileTo(encounterCharacter.transform.position);
                        Orientation startingOrientation = OrientationUtil.FromVector(encounterCharacter.transform.forward);

                        CharacterDirector.AddCharacter(character, new CharacterPosition(closestTile.Idx, startingOrientation));
                        encounterCharacter.gameObject.SetActive(false);
                    }

                    Model.EncounterContext.MaxAllyUnits = mEncounterContainer.MaxUserPlayers;
                }

                { // Enemies
                    TeamSide teamSide = TeamSide.EnemyAI;
                    Model.Teams.Add(teamSide, new List<Character>());
                    foreach (EncounterCharacterControl encounterCharacter in mEncounterContainer.EnemiesContainer.GetComponentsInChildren<EncounterCharacterControl>())
                    {
                        int characterId = encounterCharacter.GetCharacterId();
                        Character character = MAGE.GameSystems.CharacterService.Get().GetCharacter(characterId);
                        character.TeamSide = teamSide;

                        TileControl closestTile = MapControl.GetClosestTileTo(encounterCharacter.transform.position);
                        Orientation startingOrientation = OrientationUtil.FromVector(encounterCharacter.transform.forward);

                        CharacterDirector.AddCharacter(character, new CharacterPosition(closestTile.Idx, startingOrientation));
                        encounterCharacter.gameObject.SetActive(false);
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
            AITurnControl.Init();
            StatusViewControl.Init();
            IntroViewControl.Init();
            UnitPlacementViewControl.Init();

            mAmbientSoundSource = gameObject.AddComponent<AudioSource>();
            mAmbientSoundSource.clip = AudioManager.Instance.GetTrack(TrackId.Encounter);
            mAmbientSoundSource.loop = true;
            mAmbientSoundSource.spatialBlend = 0; // global volume
            //mAmbientSoundSource.Play();
            //AudioManager.Instance.FadeInTrack(mAmbientSoundSource, 10, .5f);

            Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.EncounterBegun));
        }

        protected override void Cleanup()
        {
            UnitPlacementViewControl.Cleanup();

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

            Messaging.MessageRouter.Instance.NotifyMessage(new LevelManagement.LevelMessage(LevelManagement.MessageType.EncounterComplete, mEncounterContainer));
        }
    }
}


