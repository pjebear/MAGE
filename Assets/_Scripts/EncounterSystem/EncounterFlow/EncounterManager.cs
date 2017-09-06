using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WorldSystem;
using WorldSystem.Character;
namespace EncounterSystem
{
    using Character;
    using Action;
    using Map;
    using Common.ActionTypes;

    namespace EncounterFlow
    {
        using Mediator;
        using Screen;
        using AI;
        using Common.CharacterEnums;
        using UnityEngine.SceneManagement;
        using MapTypes;
        using Common.EncounterTypes;
        using Common.EncounterEnums;
        using StorySystem.StoryCast;
        using Interface;

        class EncounterManager : MonoBehaviour
        {
            public bool FreezeAI = false;

            //View Controller
            //public PlayerInputViewController EncounterViewController { get; private set; }
            //public UnitPlacementViewController UnitPlacementViewController { get; private set; }
            //public DialogueViewController DialogueViewController { get; private set; }

            //prefabs
            public GameObject CharacterPrefab = null;
            public GameObject ActionFactoryPrefab = null;

            private EncounterFlowManager mFlowManager = null;

            //logic classes
            public MapManager MapManager { get; private set; }

            private EncounterState rCurrentState;
            public EncounterBluePrint rBluePrint;

            void Awake()
            {
                rBluePrint = EncounterSystemFacade.Instance().GetEncounterBluePrint();
                rCurrentState = EncounterSystemFacade.Instance().GetEncounterState();

                //create relevant stuff
                //----Map-----//
                MapManager = new MapManager("Maps/" + rBluePrint.MapId);

                //----ActionFactory----//
                Instantiate(ActionFactoryPrefab);

                //----Encounter Flow ----//
                mFlowManager = GetComponent<EncounterFlowManager>();
            }

            // Use this for initialization
            void Start()
            {
                MapManager.Initialize();
                //----------------------- View Models ---------------------------//
                PlayerInputViewController playerInputViewController = GameObject.Find("PlayerInputViewController").GetComponent<PlayerInputViewController>();
                CinematicViewController dialogueViewController = GameObject.Find("DialogueViewController").GetComponent<CinematicViewController>();
                UnitPlacementViewController unitPlacementViewController = GameObject.Find("UnitPlacementViewController").GetComponent<UnitPlacementViewController>();

                //--------------------------------------------------------------------------
                ActionFlowMediator actionMediator = GetComponent<ActionFlowMediator>();
                actionMediator.Initialize(playerInputViewController, Camera.main.GetComponent<CameraManager>(), MapManager, mFlowManager);
                MovementMediator movementMediator = GetComponent<MovementMediator>();
                movementMediator.Initialize(playerInputViewController, Camera.main.GetComponent<CameraManager>(), MapManager, mFlowManager);

                InputManager inputManager = new InputManager();
                inputManager.Initialize(rCurrentState, playerInputViewController, dialogueViewController, unitPlacementViewController, mFlowManager, MapManager, movementMediator);

                mFlowManager.Initialize(this, inputManager, 
                    rCurrentState, rBluePrint,
                    actionMediator, movementMediator, rBluePrint.WinConditions, rBluePrint.CinematicEvents);
                // TODO: remove adding to map from PopulateCharacterLineups
                //---------------------------------------------------------------------------
                PopulateCharacterLineups(rBluePrint);
                //---------------------------------------------------------------------------
            }


            public void EncounterOver()
            {
                EncounterSystemFacade.Instance().EncounterFinished();
            }

            //---------------------Character Lineup Query---------------------------------------------
            #region CharacterLineup

            public void PopulateCharacterLineups(EncounterBluePrint bluePrint)
            {
                // Create all cinematic Characters and hide from map
                
                foreach (var cinematicCharacter in bluePrint.CinematicCharacters)
                {
                    CharacterManager egc = Instantiate(CharacterPrefab).GetComponent<CharacterManager>();
                    egc.Initialize(cinematicCharacter.Value);
                    MapManager.AddCharacter(egc);
                    egc.gameObject.SetActive(false);
                    rCurrentState.CinematicUnits.Add(cinematicCharacter.Key, egc);
                }

                // create all potential player characters for unit placement
                List<CharacterManager> playerCharacters = new List<CharacterManager>();
                foreach (CharacterBase character in bluePrint.UnitPlacementCharacters)
                {
                    CharacterManager egc = Instantiate(CharacterPrefab).GetComponent<CharacterManager>();
                    egc.Initialize(character);
                    MapManager.AddCharacter(egc);
                    egc.gameObject.SetActive(false);
                    egc.transform.localRotation = Quaternion.Euler(0, 180 * bluePrint.UnitPlacementSide, 0);
                    playerCharacters.Add(egc);
                }
                rCurrentState.AddPlayers(UnitGroup.Player, playerCharacters);

                // create all statc enemy characters at battle start
                List<CharacterManager> enemyCharacters = new List<CharacterManager>();
                int enemyPlacementSide = (bluePrint.UnitPlacementSide + 1) % 2;
                foreach (UnitPlacement placement in bluePrint.StaticEnemyCharacters)
                {
                    CharacterManager egc = Instantiate(CharacterPrefab).GetComponent<CharacterManager>();
                    egc.Initialize(placement.Unit);
                    if (placement.Placement != TileIndex.Invalid)
                    {
                        MapManager.AddCharacterAt(egc, placement.Placement.x, placement.Placement.y);   
                    }
                    else
                    {
                        MapManager.PlaceCharacterAtRandomSpawnTile(egc, enemyPlacementSide);
                    }
                    egc.gameObject.SetActive(false);
                    egc.transform.localRotation = Quaternion.Euler(0, 180 * enemyPlacementSide, 0);
                    enemyCharacters.Add(egc);
                }
                rCurrentState.SetPlayers(UnitGroup.AI, enemyCharacters);

                mFlowManager.BeginEncounterFlow();
            }

            
            #endregion
        }
    }
}


