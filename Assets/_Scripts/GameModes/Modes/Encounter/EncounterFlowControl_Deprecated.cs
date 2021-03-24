using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounters;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Loot;
using MAGE.GameSystems.World;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class EncounterFlowControl_Deprecated //: FlowControlBase
    {
        private string TAG = "EncounterFlowControl";

        public bool RunDebug = false;

        private GameObject View;
        private AudioSource mAmbientSoundSource;
        private EncounterContainer_Deprecated mEncounterContainer;

        public static CharacterDirector CharacterDirector;
        public static AuraDirector AuraDirector;
        public static AnimationDirector AnimationDirector;
        public static ActionDirector_Deprecated ActionDirector;
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

        //protected void CalculateSpawnPoints(PlacementInfo placementInfo)
        //{
        //    GameModel.Encounter.AllySpawnPoints = new List<TileIdx>();

        //    int leftBound = 0, rightBound = 0, upperBound = 0, lowerBound = 0;
        //    switch (placementInfo.PlacementRegion)
        //    {
        //        case PlacementRegion.Bottom:
        //        {
        //            leftBound = 0;
        //            rightBound = MapControl.Map.Width;
        //            upperBound = 2;
        //            lowerBound = 0;
        //        }
        //        break;

        //        case PlacementRegion.Top:
        //        {
        //            leftBound = 0;
        //            rightBound = MapControl.Map.Width;
        //            upperBound = MapControl.Map.Length;
        //            lowerBound = MapControl.Map.Length - 2;
        //        }
        //        break;

        //        case PlacementRegion.Left:
        //        {
        //            leftBound = 0;
        //            rightBound = 2;
        //            upperBound = MapControl.Map.Length;
        //            lowerBound = 0;
        //        }
        //        break;

        //        case PlacementRegion.Right:
        //        {
        //            leftBound = MapControl.Map.Width - 2;
        //            rightBound = MapControl.Map.Width;
        //            upperBound = MapControl.Map.Length;
        //            lowerBound = 0;
        //        }
        //        break;
        //    }
            

        //    for (int y = lowerBound; y < upperBound; ++y)
        //    {
        //        for (int x = leftBound; x < rightBound; ++x)
        //        {
        //            GameModel.Encounter.AllySpawnPoints.Add(new TileIdx(x, y));
        //        }
        //    }
        //}

        //public override FlowControlId GetFlowControlId()
        //{
        //    return FlowControlId.Encounter;
        //}

        //protected override void Setup()
        //{
        //    GameModel.Encounter.EncounterContext = new GameSystems.World.EncounterContext();

        //    CharacterDirector = gameObject.AddComponent<CharacterDirector>();
        //    AnimationDirector = gameObject.AddComponent<AnimationDirector>();
        //    ActionDirector = gameObject.AddComponent<ActionDirector>();
        //    MovementDirector = gameObject.AddComponent<MovementDirector>();
        //    AnimationDirector = gameObject.AddComponent<AnimationDirector>();
        //    AuraDirector = gameObject.AddComponent<AuraDirector>();
        //    ProjectileDirector = gameObject.AddComponent<ProjectileDirector>();
        //    EffectSpawner = gameObject.AddComponent<EffectSpawner>();

        //    IntroViewControl = gameObject.AddComponent<EncounterIntroViewControl>();
        //    UnitPlacementViewControl = new EncounterUnitPlacementViewControl();
        //    StatusViewControl = gameObject.AddComponent<EncounterStatusControl>();
        //    TurnFlowControl = gameObject.AddComponent<TurnFlowControl>();
        //    AITurnControl = gameObject.AddComponent<AITurnControl>();
        //    MasterFlowControl = gameObject.AddComponent<MasterFlowControl>();

        //    MAGE.GameModes.ILevelManagerService levelManagerService = MAGE.GameModes.LevelManagementService.Get();
        //    Level level = levelManagerService.GetLoadedLevel();
        //    level.ToggleTreeColliders(false);

        //    List<EncounterContainer> activeEncounters = level.EncounterContainer.GetComponentsInChildren<EncounterContainer>().Where(x => x.IsEncounterPending).ToList();
        //    if (activeEncounters.Count > 0)
        //    {
        //        mEncounterContainer = activeEncounters[0];
        //        mEncounterContainer.StartEncounter();
        //        MapControl = new MapControl();
        //        if (mEncounterContainer.Tiles.GetComponent<TileContainer>() == null)
        //        {
        //            level.TileContainerGenerator.InitializeContainer(mEncounterContainer.Tiles);
        //        }

        //        MapControl.Initialize(mEncounterContainer.Tiles.GetComponent<TileContainer>());

        //        // Encounter Conditions
        //        foreach (EncounterConditionParams encounterCreateParams in mEncounterContainer.WinConditions)
        //        {
        //            GameModel.Encounter.EncounterContext.WinConditions.Add(EncounterConditionFactory.CheckoutCondition(encounterCreateParams));
        //        }
        //        Debug.Assert(GameModel.Encounter.EncounterContext.WinConditions.Count > 0);
        //        if (GameModel.Encounter.EncounterContext.WinConditions.Count == 0)
        //        {
        //            GameModel.Encounter.EncounterContext.WinConditions.Add(new TeamDefeatedCondition(TeamSide.EnemyAI));
        //        }

        //        foreach (EncounterConditionParams encounterCreateParams in mEncounterContainer.LoseConditions)
        //        {
        //            GameModel.Encounter.EncounterContext.LoseConditions.Add(EncounterConditionFactory.CheckoutCondition(encounterCreateParams));
        //        }

        //        { // Allies
        //            TeamSide teamSide = TeamSide.AllyHuman;
        //            GameModel.Encounter.Teams.Add(teamSide, new List<Character>());
        //            foreach (ActorSpawner spawner in mEncounterContainer.AlliesContainer.GetComponentsInChildren<ActorSpawner>())
        //            {
        //                int characterId = spawner.CharacterPicker.GetCharacterId();
        //                Character character = CharacterService.Get().GetCharacter(characterId);
        //                character.TeamSide = teamSide;

        //                TileControl closestTile = MapControl.GetClosestTileTo(spawner.transform.position);
        //                Orientation startingOrientation = OrientationUtil.FromVector(spawner.transform.forward);

        //                CharacterDirector.AddCharacter(character, spawner, new CharacterPosition(closestTile.Idx, startingOrientation));
        //            }

        //            GameModel.Encounter.EncounterContext.MaxAllyUnits = mEncounterContainer.MaxUserPlayers;
        //        }

        //        { // Enemies
        //            TeamSide teamSide = TeamSide.EnemyAI;
        //            GameModel.Encounter.Teams.Add(teamSide, new List<Character>());
        //            foreach (ActorSpawner spawner in mEncounterContainer.EnemiesContainer.GetComponentsInChildren<ActorSpawner>())
        //            {
        //                int characterId = spawner.CharacterPicker.GetCharacterId();
        //                Character character = CharacterService.Get().GetCharacter(characterId);
        //                character.TeamSide = teamSide;

        //                TileControl closestTile = MapControl.GetClosestTileTo(spawner.transform.position);
        //                Orientation startingOrientation = OrientationUtil.FromVector(spawner.transform.forward);

        //                CharacterDirector.AddCharacter(character, spawner, new CharacterPosition(closestTile.Idx, startingOrientation));
        //            }
        //        }

        //        CalculateSpawnPoints(mEncounterContainer.PlacementInfo);

        //        // Rewards
        //        {
        //            ClaimLootParams lootParams = new ClaimLootParams();
        //            lootParams.EncounterId = mEncounterContainer.EncounterScenarioId;
        //            lootParams.LevelId = LevelManagementService.Get().GetLoadedLevel().LevelId;
        //            lootParams.Mobs.AddRange(mEncounterContainer.MobsInEncounter);

        //            GameModel.Encounter.EncounterContext.Rewards = WorldService.Get().GetLoot(lootParams);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Assert(false);
        //    }

        //    if (Camera.main.GetComponent<CameraDirector>() == null)
        //    {
        //        Camera.main.gameObject.AddComponent<CameraDirector>();
        //    }

        //    CameraDirector = Camera.main.GetComponent<CameraDirector>();
        //    Camera.main.gameObject.AddComponent<AudioListener>();

        //    AuraDirector.Init();
        //    ActionDirector.Init();
        //    MovementDirector.Init();
        //    ProjectileDirector.Init();
        //    MasterFlowControl.Init();
        //    TurnFlowControl.Init();
        //    AITurnControl.Init();
        //    StatusViewControl.Init();
        //    IntroViewControl.Init();
        //    UnitPlacementViewControl.Init();

        //    mAmbientSoundSource = gameObject.AddComponent<AudioSource>();
        //    mAmbientSoundSource.clip = AudioManager.Instance.GetTrack(TrackId.Encounter);
        //    mAmbientSoundSource.loop = true;
        //    mAmbientSoundSource.spatialBlend = 0; // global volume
        //    //mAmbientSoundSource.Play();
        //    //AudioManager.Instance.FadeInTrack(mAmbientSoundSource, 10, .5f);

        //    Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.EncounterBegun));
        //}

        //protected override void Cleanup()
        //{
        //    UnitPlacementViewControl.Cleanup();

        //    EncounterResultInfo resultInfo = new EncounterResultInfo();
        //    resultInfo.DidUserWin = GameModel.Encounter.EncounterState == EncounterState.Win;
        //    resultInfo.Rewards = GameModel.Encounter.EncounterContext.Rewards;
        //    resultInfo.PlayersInEncounter = new Teams();
        //    foreach (var teamList in GameModel.Encounter.Teams)
        //    {
        //        foreach (Character character in teamList.Value)
        //        {
        //            resultInfo.PlayersInEncounter[teamList.Key].Add(character.Id);
        //        }
        //    }

        //    MAGE.GameSystems.WorldService.Get().UpdateOnEncounterEnd(resultInfo);

        //    MAGE.GameModes.ILevelManagerService levelManagerService = MAGE.GameModes.LevelManagementService.Get();
        //    if (mEncounterContainer.EncounterScenarioId == EncounterScenarioId.Random)
        //    {
        //        Destroy(mEncounterContainer.gameObject);
        //    }
        //    else
        //    {
        //        EncounterInfo info = levelManagerService.GetEncounterInfo((int)mEncounterContainer.EncounterScenarioId);
        //        info.IsActive = false;
        //        levelManagerService.UpdateEncounterInfo(info);
        //    }


        //    Level level = levelManagerService.GetLoadedLevel();
        //    level.ToggleTreeColliders(true);
        //    //MapControl.Cleanup();
        //    Destroy(CameraDirector);
        //    Destroy(Camera.main.GetComponent<AudioListener>());
        //    CharacterDirector.CleanupCharacters();

        //    Messaging.MessageRouter.Instance.NotifyMessage(new LevelManagement.LevelMessage(LevelManagement.MessageType.EncounterComplete, mEncounterContainer));
        //}
    }
}


