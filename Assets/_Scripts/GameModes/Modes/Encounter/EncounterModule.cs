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
    public static CameraDirector CameraDirector;
    public static EffectSpawner EffectSpawner;
    public static Map Map;

    public static EncounterIntroViewControl IntroViewControl;
    public static EncounterUnitPlacementViewControl UnitPlacementViewControl;
    public static EncounterStatusControl StatusViewControl;
    public static MasterFlowControl MasterFlowControl;
    public static TurnFlowControl TurnFlowControl;

    private void Awake()
    {
        Model = new EncounterModel();

        CharacterDirector = GetComponent<CharacterDirector>();
        AnimationDirector = GetComponent<AnimationDirector>();
        ActionDirector = GetComponent<ActionDirector>();
        AnimationDirector = GetComponent<AnimationDirector>();
        AuraDirector = GetComponent<AuraDirector>();
        EffectSpawner = GetComponentInChildren<EffectSpawner>();

        IntroViewControl = GetComponent<EncounterIntroViewControl>();
        UnitPlacementViewControl = new EncounterUnitPlacementViewControl();
        StatusViewControl = GetComponent<EncounterStatusControl>();
        TurnFlowControl = GetComponent<TurnFlowControl>();
        MasterFlowControl = GetComponent<MasterFlowControl>();
    }

    protected override void SetupMode()
    {
        Logger.Log(LogTag.GameModes, TAG, "::SetupMode()");

        Model.EncounterContext = GameSystemModule.Instance.GetEncounterContext();

        AuraDirector.Init();
        ActionDirector.Init();
        MasterFlowControl.Init();
        TurnFlowControl.Init();
        StatusViewControl.Init();
        IntroViewControl.Init();
        UnitPlacementViewControl.Init();


        // Level Manager:
        //TODO: Load level if needed
        Level level = GameModesModule.LevelManager.GetLoadedLevel();

        Map = new Map();
        Map.Initialize(level.Tiles, Model.EncounterContext.BottomLeft, Model.EncounterContext.TopRight);

        CalculateSpawnPoints();

        if (Camera.main.GetComponent<CameraDirector>() == null)
        {
            Camera.main.gameObject.AddComponent<CameraDirector>();
        }

        CameraDirector = Camera.main.GetComponent<CameraDirector>();

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

        count = 0;
        team = TeamSide.EnemyAI;
        Model.Teams.Add(team, new List<EncounterCharacter>());
        foreach (DB.DBCharacter dBCharacter in DB.DBHelper.LoadCharactersOnTeam(team))
        {
            CharacterDirector.AddCharacter(dBCharacter, team, Model.EnemySpawnPoints[count]);
            count++;
        }

        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeSetup_Complete));
    }

    protected override void CleanUpMode()
    {
        Map.Cleanup();
        Destroy(CameraDirector);
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

