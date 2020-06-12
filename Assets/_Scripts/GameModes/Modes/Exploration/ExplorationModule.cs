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

    GameObject mExplorationAvatar;
    ExplorationMenuViewControl MenuControl;
    InteractionFlowControl mInteractionFlowControl;
    ScenarioFlowControl mScenarioFlowControl;
    AudioSource mAmbientSoundSource;

    public MovementDirector MovementDirector;

    protected override void SetupMode()
    {
        Instance = this;

        MenuControl = new ExplorationMenuViewControl();
        mInteractionFlowControl = GetComponent<InteractionFlowControl>();
        mScenarioFlowControl = GetComponent<ScenarioFlowControl>();
        MovementDirector = gameObject.AddComponent<MovementDirector>();
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

        level.ScenarioContainer.gameObject.SetActive(true);
        level.NPCContainer.gameObject.SetActive(true);

        DB.DBCharacter avatar = DB.DBHelper.LoadCharacter(GameSystemModule.Instance.GetPartyAvatarId());

        mExplorationAvatar = GameModesModule.ActorLoader.CreateActor(DB.CharacterHelper.FromDB(avatar.Appearance), level.SpawnPoint).gameObject;
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

        GameModeEventRouter.Instance.NotifyEvent(new GameModeEvent(GameModeEvent.EventType.ModeSetup_Complete));
    }

    protected override void CleanUpMode()
    {
        mInteractionFlowControl.CleanUp();
        mScenarioFlowControl.CleanUp();

        Destroy(Camera.main.gameObject.GetComponent<ThirdPersonCamera>());
        Destroy(mExplorationAvatar);

        GameModesModule.LevelManager.GetLoadedLevel().ScenarioContainer.gameObject.SetActive(false);
        GameModesModule.LevelManager.GetLoadedLevel().NPCContainer.gameObject.SetActive(false);

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

