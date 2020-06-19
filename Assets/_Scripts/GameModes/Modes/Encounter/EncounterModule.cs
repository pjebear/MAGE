using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

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

        Model.EncounterContext = GameSystemModule.Instance.GetEncounterContext();

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

    public override LevelId GetLevelId()
    {
        return Model.EncounterContext.LevelId;
    }

    protected override void SetupMode()
    {
        Logger.Log(LogTag.GameModes, TAG, "::SetupMode()");

        // Level Manager:
        //TODO: Load level if needed

        Level level = GameModesModule.LevelManager.GetLoadedLevel();
        if (level == null || level.LevelId != Model.EncounterContext.LevelId)
        {
            GameModesModule.LevelManager.LoadLevel(Model.EncounterContext.LevelId);
            level = GameModesModule.LevelManager.GetLoadedLevel();
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

        //foreach (DB.DBCharacter dBCharacter in DB.DBHelper.LoadCharactersOnTeam(team))
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
        foreach (DB.DBCharacter dBCharacter in DB.DBHelper.LoadCharactersOnTeam(team))
        {
            CharacterInfo character = DB.CharacterHelper.FromDB(dBCharacter);
            EncounterCharacter encounterCharacter = new EncounterCharacter(team, character);

            TileIdx spawnPoint = Model.EnemySpawnPoints.Find((x)=>Map[x].OnTile == null);
            if (Model.EncounterContext.CharacterPositions.ContainsKey(dBCharacter.Id))
            {
                spawnPoint = Map[Model.EncounterContext.CharacterPositions[dBCharacter.Id]].Idx;
            }

            CharacterDirector.AddCharacter(encounterCharacter, spawnPoint);
        }

        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeSetup_Complete));
    }

    protected override void CleanUpMode()
    {
        Map.Cleanup();
        Destroy(CameraDirector);
        Destroy(Camera.main.GetComponent<AudioListener>());
        CharacterDirector.CleanupCharacters();

        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeTakedown_Complete));
    }

    protected override void StartMode()
    {
        mAmbientSoundSource = gameObject.AddComponent<AudioSource>();
        mAmbientSoundSource.clip = GameModesModule.AudioManager.GetTrack(TrackId.Encounter);
        mAmbientSoundSource.loop = true;
        mAmbientSoundSource.spatialBlend = 0; // global volume
        mAmbientSoundSource.Play();
        GameModesModule.AudioManager.FadeInTrack(mAmbientSoundSource, 10, .5f);

        EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.EncounterBegun));
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

        GameSystemModule.Instance.UpdateOnEncounterEnd(resultInfo);
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

