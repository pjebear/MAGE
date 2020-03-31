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
    public List<Vector2> AllySpawnPoints = new List<Vector2>() { new Vector2(0,0), new Vector2(0, 1) };
    public List<Vector2> EnemySpawnPoints = new List<Vector2>() { new Vector2(2, 0), new Vector2(2, 1) };

    public GameObject MapPrefab;
    
    private GameObject View;

    public static EncounterModel Model;
    public static CharacterDirector CharacterDirector;
    public static AuraDirector AuraDirector;
    public static AnimationDirector AnimationDirector;
    public static ActionDirector ActionDirector;
    public static CameraDirector CameraDirector;
    public static EffectSpawner EffectSpawner;
    public static Map Map;

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

        StatusViewControl = GetComponent<EncounterStatusControl>();
        TurnFlowControl = GetComponent<TurnFlowControl>();
        MasterFlowControl = GetComponent<MasterFlowControl>();
    }

    protected override void SetupMode()
    {
        Logger.Log(LogTag.GameModes, TAG, "::SetupMode()");

        AuraDirector.Init();
        MasterFlowControl.Init();
        TurnFlowControl.Init();
        StatusViewControl.Init();

        Map = GameObject.Find("Map").GetComponent<Map>();
        if (Map == null)
        {
            Map = Instantiate(MapPrefab).GetComponent<Map>();
        }

        if (Camera.main.GetComponent<CameraDirector>() == null)
        {
            Camera.main.gameObject.AddComponent<CameraDirector>();
        }
        CameraDirector = Camera.main.GetComponent<CameraDirector>();

        EncounterContext context = GameSystemModule.Instance.GetEncounterContext();

        int count = 0;
        TeamSide team = TeamSide.AllyHuman;

        Model.Teams.Add(team, new List<EncounterCharacter>());
        List<Vector2> spawnPoints = AllySpawnPoints;
        foreach (DB.DBCharacter dBCharacter in DB.DBHelper.LoadCharactersOnTeam(team))
        {
            TileIdx atPostition = new TileIdx((int)spawnPoints[count].x, (int)spawnPoints[count].y);

            EncounterCharacter character = new EncounterCharacter(team, CharacterLoader.LoadCharacter(dBCharacter));
            Model.Characters.Add(character.Id, character);
            Model.Teams[team].Add(character);

            CharacterDirector.AddCharacter(character, CharacterUtil.ActorParamsForCharacter(dBCharacter), atPostition);

            count++;
        }

        count = 0;
        team = TeamSide.EnemyAI;
        Model.Teams.Add(team, new List<EncounterCharacter>());
        spawnPoints = EnemySpawnPoints;
        foreach (DB.DBCharacter dBCharacter in DB.DBHelper.LoadCharactersOnTeam(team))
        {
            TileIdx atPostition = new TileIdx((int)spawnPoints[count].x, (int)spawnPoints[count].y);

            EncounterCharacter character = new EncounterCharacter(team, CharacterLoader.LoadCharacter(dBCharacter));
            Model.Characters.Add(character.Id, character);
            Model.Teams[team].Add(character);
            CharacterDirector.AddCharacter(character, CharacterUtil.ActorParamsForCharacter(dBCharacter), atPostition);

            count++;
        }

        Model.WinConditions = context.WinConditions;
        Model.LoseConditions = context.LoseConditions;

        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeSetup_Complete));
    }

    protected override void CleanUpMode()
    {
        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeTakedown_Complete));
    }

    protected override void StartMode()
    {
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
}

