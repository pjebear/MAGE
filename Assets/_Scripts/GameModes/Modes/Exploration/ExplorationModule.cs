using Invector.vCharacterController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ExplorationModule : GameModeBase
{
    public static ExplorationModule Instance;

    public GameObject ExplorationAvatarPrefab;

    GameObject mExplorationAvatar;
    ExplorationMenuViewControl MenuControl;

    protected override void SetupMode()
    {
        Instance = this;

        MenuControl = new ExplorationMenuViewControl();

        LevelId levelToExplore = GameSystemModule.Instance.GetCurrentLevel();

        Level level = LevelManager.Instance.GetLoadedLevel();
        if (level != null && level.LevelId != levelToExplore)
        {
            LevelManager.Instance.UnloadLevel();
            level = null;
        }

        if (level == null)
        {
            LevelManager.Instance.LoadLevel(levelToExplore);
            level = LevelManager.Instance.GetLoadedLevel();
        }

        //DB.DBCharacter avatar = DB.DBHelper.LoadCharacter(GameSystemModule.Instance.GetPartyAvatarId());
        //GameObject go = GameModesModule.ActorLoader.CreateActor(CharacterUtil.ActorParamsForCharacter(avatar), level.SpawnPoint).gameObject;
        //go.AddComponent<vThirdPersonMotor>();
        //go.AddComponent<vThirdPersonController>();
        //go.AddComponent<vThirdPersonInput>();
        mExplorationAvatar = Instantiate(ExplorationAvatarPrefab, level.transform);
        mExplorationAvatar.transform.SetPositionAndRotation(level.SpawnPoint.position, level.SpawnPoint.rotation);

        Camera.main.gameObject.AddComponent<vThirdPersonCamera>().SetMainTarget(mExplorationAvatar.transform);

        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeSetup_Complete));
    }

    protected override void CleanUpMode()
    {
        Destroy(Camera.main.gameObject.GetComponent<vThirdPersonCamera>());
        Destroy(mExplorationAvatar);
        
        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeTakedown_Complete));

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

        EncounterCreateParams randomParams = new EncounterCreateParams();
        randomParams.ScenarioId = EncounterScenarioId.Random;
        randomParams.LevelId = LevelManager.Instance.GetLoadedLevel().LevelId;
        randomParams.BottomLeft = new TileIdx((int)mExplorationAvatar.transform.position.x, (int)mExplorationAvatar.transform.position.z);
        randomParams.TopRight = new TileIdx(randomParams.BottomLeft.x + 5, randomParams.BottomLeft.y + 5);


        GameSystemModule.Instance.PrepareEncounter(randomParams);
        GameModesModule.Instance.Encounter();
    }
}

