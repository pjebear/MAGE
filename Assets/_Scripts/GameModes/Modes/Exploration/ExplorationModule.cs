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
    AudioSource mAmbientSoundSource;

    protected override void SetupMode()
    {
        Instance = this;

        MenuControl = new ExplorationMenuViewControl();

        LevelId levelToExplore = GameSystemModule.Instance.GetCurrentLevel();

        Level level = GameModesModule.LevelManager.GetLoadedLevel();
        if (level != null && level.LevelId != levelToExplore)
        {
            GameModesModule.LevelManager.UnloadLevel();
            level = null;
        }

        if (level == null)
        {
            GameModesModule.LevelManager.LoadLevel(levelToExplore);
            level = GameModesModule.LevelManager.GetLoadedLevel();
        }

        //DB.DBCharacter avatar = DB.DBHelper.LoadCharacter(GameSystemModule.Instance.GetPartyAvatarId());
        //GameObject go = GameModesModule.ActorLoader.CreateActor(CharacterUtil.ActorParamsForCharacter(avatar), level.SpawnPoint).gameObject;
        //go.AddComponent<vThirdPersonMotor>();
        //go.AddComponent<vThirdPersonController>();
        //go.AddComponent<vThirdPersonInput>();
        mExplorationAvatar = Instantiate(ExplorationAvatarPrefab, level.transform);
        mExplorationAvatar.transform.SetPositionAndRotation(level.SpawnPoint.position, level.SpawnPoint.rotation);

        Camera.main.gameObject.AddComponent<vThirdPersonCamera>().SetMainTarget(mExplorationAvatar.transform);

        mExplorationAvatar.AddComponent<AudioListener>();

        mAmbientSoundSource = gameObject.AddComponent<AudioSource>();
        mAmbientSoundSource.clip = GameModesModule.AudioManager.GetTrack(TrackId.Explore);
        mAmbientSoundSource.loop = true;
        mAmbientSoundSource.spatialBlend = 0; // global volume
        mAmbientSoundSource.Play();
        GameModesModule.AudioManager.FadeInTrack(mAmbientSoundSource, 5, .5f);

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
        randomParams.LevelId = GameModesModule.LevelManager.GetLoadedLevel().LevelId;
        randomParams.BottomLeft = new TileIdx((int)mExplorationAvatar.transform.position.x, (int)mExplorationAvatar.transform.position.z);
        randomParams.TopRight = new TileIdx(randomParams.BottomLeft.x + 5, randomParams.BottomLeft.y + 5);


        GameSystemModule.Instance.PrepareEncounter(randomParams);
        GameModesModule.Instance.Encounter();
    }
}

