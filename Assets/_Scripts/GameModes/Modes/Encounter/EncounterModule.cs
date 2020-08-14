using MAGE.GameModes.FlowControl;
using MAGE.GameModes.LevelManagement;
using MAGE.GameModes.SceneElements;
using MAGE.GameServices;
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

        public static EncounterModel Model;
        public static CharacterDirector CharacterDirector;
        public static AuraDirector AuraDirector;
        public static AnimationDirector AnimationDirector;
        public static ActionDirector ActionDirector;
        public static MovementDirector MovementDirector;
        public static CameraDirector CameraDirector;
        public static ProjectileDirector ProjectileDirector;
        public static EffectSpawner EffectSpawner;
        public static Map Map;

        public static EncounterIntroViewControl IntroViewControl;
        public static EncounterUnitPlacementViewControl UnitPlacementViewControl;
        public static EncounterStatusControl StatusViewControl;
        public static MasterFlowControl MasterFlowControl;
        public static TurnFlowControl TurnFlowControl;

        public override void Init()
        {
            Model = new EncounterModel();

            Model.EncounterContext = MAGE.GameServices.WorldService.Get().GetEncounterContext();

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

        public override GameServices.LevelId GetLevelId()
        {
            return Model.EncounterContext.LevelId;
        }

        protected override void SetupMode()
        {
            Logger.Log(LogTag.GameModes, TAG, "::SetupMode()");

            // Level Manager:
            //TODO: Load level if needed

            MAGE.GameModes.ILevelManagerService levelManagerService = MAGE.GameModes.LevelManagementService.Get();
            Level level = levelManagerService.GetLoadedLevel();
            if (level == null || level.LevelId != Model.EncounterContext.LevelId)
            {
                levelManagerService.LoadLevel(Model.EncounterContext.LevelId);
                level = levelManagerService.GetLoadedLevel();
            }

            Map = new Map();
            Map.Initialize(level.Tiles, Model.EncounterContext.BottomLeft, Model.EncounterContext.TopRight);

            CalculateSpawnPoints();

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

            // Load players
            int count = 0;
            TeamSide team = TeamSide.AllyHuman;
            Model.Teams.Add(team, new List<EncounterCharacter>());

            //foreach (DB.DBCharacter dBCharacter in DBService.Get().LoadCharactersOnTeam(team))
            //{
            //    TileIdx atPostition = new TileIdx((int)spawnPoints[count].x + context.BottomLeft.x, (int)spawnPoints[count].y + context.BottomLeft.y);

            //    EncounterCharacter character = new EncounterCharacter(team, CharacterLoader.LoadCharacter(dBCharacter));
            //    Model.Characters.Add(character.Id, character);
            //    Model.Teams[team].Add(character);

            //    CharacterDirector.AddCharacter(character, CharacterUtil.ActorParamsForCharacter(dBCharacter), atPostition);

            //    count++;
            //}

            team = TeamSide.EnemyAI;
            Model.Teams.Add(team, new List<EncounterCharacter>());
            foreach (int characterId in MAGE.GameServices.DBService.Get().LoadTeam(team))
            {
                MAGE.GameServices.Character.CharacterInfo characterInfo = MAGE.GameServices.CharacterService.Get().GetCharacterInfo(characterId);
                EncounterCharacter encounterCharacter = new EncounterCharacter(team, characterInfo);

                TileIdx spawnPoint = Model.EnemySpawnPoints.Find((x) => Map[x].OnTile == null);
                if (Model.EncounterContext.CharacterPositions.ContainsKey(characterId))
                {
                    spawnPoint = Map[Model.EncounterContext.CharacterPositions[characterId]].Idx;
                }

                CharacterDirector.AddCharacter(encounterCharacter, spawnPoint);
            }

            Messaging.MessageRouter.Instance.NotifyMessage(new GameModeMessage(GameModeMessage.EventType.ModeSetup_Complete));
        }

        protected override void CleanUpMode()
        {
            Map.Cleanup();
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
            mAmbientSoundSource.Play();
            GameModesModule.AudioManager.FadeInTrack(mAmbientSoundSource, 10, .5f);

            Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.EncounterBegun));
        }

        protected override void EndMode()
        {
            EncounterResultInfo resultInfo = new EncounterResultInfo();
            resultInfo.DidUserWin = Model.EncounterState == EncounterState.Win;
            resultInfo.PlayersInEncounter = new Teams();
            foreach (var teamList in Model.Teams)
            {
                foreach (EncounterCharacter character in teamList.Value)
                {
                    resultInfo.PlayersInEncounter[teamList.Key].Add(character.Id);
                }
            }

            MAGE.GameServices.WorldService.Get().UpdateOnEncounterEnd(resultInfo);
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
                for (int x = 0; x < Map.Width; ++x)
                {
                    Model.AllySpawnPoints.Add(new TileIdx(x + Map.TileIdxOffset.x, y + Map.TileIdxOffset.y));
                }
            }

            Model.EnemySpawnPoints = new List<TileIdx>();
            for (int y = 0; y < 2; ++y)
            {
                for (int x = 0; x < Map.Width; ++x)
                {
                    Model.EnemySpawnPoints.Add(new TileIdx(x + Map.TileIdxOffset.x, Map.Length - 1 - y + Map.TileIdxOffset.y));
                }
            }
        }
    }
}


