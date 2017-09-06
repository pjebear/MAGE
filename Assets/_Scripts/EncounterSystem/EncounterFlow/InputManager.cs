using Common.ActionTypes;
using Common.CharacterEnums;
using Common.EncounterTypes;
using EncounterSystem.Action;
using EncounterSystem.Character;
using EncounterSystem.EncounterFlow.AI;
using EncounterSystem.EncounterFlow.Mediator;
using EncounterSystem.Map;
using EncounterSystem.MapTypes;
using EncounterSystem.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EncounterSystem.EncounterFlow
{

    class InputManager
    {
        private EncounterState rEncounterState = null;
        private AIManager mAIManager = null;

        private PlayerInputViewController mPlayerInputViewController = null;
        private CinematicViewController mCinematicViewController = null;
        private UnitPlacementViewController mUnitPlacementViewController = null;

        private EncounterFlowManager rFlowManager = null;
        private MapManager rMapManager = null;

        public InputManager()
        {
            mAIManager = new AIManager();
        }

        public void Initialize(EncounterState state, PlayerInputViewController inputViewController, CinematicViewController dialogueController, UnitPlacementViewController unitPlacementController,
            EncounterFlowManager flowManager, MapManager mapmanager, MovementMediator movementMediator)
        {
            rEncounterState = state;

            mPlayerInputViewController = inputViewController;
            mPlayerInputViewController.Initialize(this);
            mPlayerInputViewController.gameObject.SetActive(false);

            mCinematicViewController = dialogueController;
            mCinematicViewController.Initialize(this, movementMediator, state);
            mCinematicViewController.gameObject.SetActive(false);

            mUnitPlacementViewController = unitPlacementController;
            mUnitPlacementViewController.Initialize(this);
            mUnitPlacementViewController.gameObject.SetActive(false);

            rFlowManager = flowManager;
            rMapManager = mapmanager;
            mAIManager.Initialize(this, state, inputViewController);
        }

        //---------------------TurnFlow---------------------------------------------
        #region UnitTurnFlow
        public void GetInput(CharacterManager unit)
        {
            mPlayerInputViewController.gameObject.SetActive(true);
            Debug.Assert(unit.IsAlive);
            Debug.Assert(!unit.FinishedTurn);

            if (unit.IsPlayerControlled)
            {
                mPlayerInputViewController.GetInputForCharacter(unit);
            }
            else
            {
                mAIManager.GetInputForUnit();
            }
        }

        public void GetInput(CinematicBlueprint cinematic)
        {
            mCinematicViewController.gameObject.SetActive(true);
            mCinematicViewController.BeginCinematicEvent(cinematic);
        }

        public void GetInput(List<CharacterManager> unitsToPlace, int numUnits, int mapSide)
        {
            mUnitPlacementViewController.gameObject.SetActive(true);
            mUnitPlacementViewController.GetCharacterLineupForEncounter(unitsToPlace, numUnits, mapSide);
        }

        public void WaitSelected()
        {
            mPlayerInputViewController.gameObject.SetActive(false);
            rEncounterState.CurrentUnit.FinishTurn();
            rFlowManager.ReturnControlToFlow();
        }

        public void CinematicFinished()
        {
            mCinematicViewController.gameObject.SetActive(false);
            rFlowManager.ReturnControlToFlow();
        }

        public void PlacementFinished(List<CharacterManager> chosenUnits)
        {
            HashSet<CharacterManager> potentialSet = rEncounterState.PlayerUnits;
            foreach (CharacterManager character in potentialSet.Except(chosenUnits))
            {
                UnityEngine.Object.Destroy(character.gameObject);
            }
            rEncounterState.SetPlayers(UnitGroup.Player, chosenUnits);
            rEncounterState.SetPlayers(UnitGroup.Player, chosenUnits);
            mUnitPlacementViewController.gameObject.SetActive(false);
            rFlowManager.ReturnControlToFlow();
        }

        // Called from AIManager
        public void ActionChosen(ActionBase action)
        {
            MapTile tile = rMapManager.GetAOEOrigin();
            ActionTargetSelection targetSelection;
            //TODO: Poll user for tile/character being the target of the action
            if (tile.GetCharacterOnTile() != null)
            {
                targetSelection = new ActionTargetSelection(tile.GetCharacterOnTile());
            }
            else
            {
                targetSelection = new ActionTargetSelection(tile);
            }

            QueuedActionPayload actionPayload = new QueuedActionPayload(rEncounterState.CurrentUnit, action, targetSelection);
            int chargeSpeed = rEncounterState.CurrentUnit.GetChargeSpeedForAction(action);
            if (chargeSpeed > 0)
            {
                rEncounterState.CurrentUnit.IsChargingAction = true;
                // set charge animation
            }
            mPlayerInputViewController.gameObject.SetActive(false);
            rFlowManager.InputChosen(actionPayload);
        }

        // Called from ViewModel
        public void ActionChosen( CharacterActionIndex actionIndex)
        {
            ActionBase action = rEncounterState.CurrentUnit.GetCharacterAction(actionIndex);
            ActionChosen( action);
        }

        public void MovementChosen( MapTile to)
        {
            //Debug.Assert(MapManager.IsValidMoveTile(to), "Attempting to move to invalid tile, please enfore valid tile selection");
            mPlayerInputViewController.gameObject.SetActive(false);
            rFlowManager.InputChosen(to);
        }
        #endregion

        //---------------------TileSelectionFunction---------------------------------------------
        #region TileSelectionFunction

        public void CalculateActionTiles(CharacterManager toAct, CharacterActionIndex actionIndex)
        {
            rMapManager.CalculateActionTiles(toAct.GetCurrentTile(), toAct.GetCharacterAction(actionIndex).MapInteractionInfo);
        }

        public void CalculateActionTiles(MapTile castTile, ActionBase action)
        {
            rMapManager.CalculateActionTiles(castTile, action.MapInteractionInfo);
        }

        public void CalculateActionAOETiles(MapTile target, MapTile castTile, ActionBase action)
        {
            rMapManager.CalculateActionAOETiles(target, castTile, action.MapInteractionInfo);
        }

        public void CalculateActionAOETiles(MapTile target, CharacterManager toAct, CharacterActionIndex actionIndex)
        {
            rMapManager.CalculateActionAOETiles(target, toAct.GetCurrentTile(), toAct.GetCharacterAction(actionIndex).MapInteractionInfo);
        }

        public void DisplayActionTiles(bool display)
        {
            rMapManager.DisplayActionTiles(display);
        }

        public void DisplayActionAOETiles(bool display)
        {
            rMapManager.DisplayActionAOETiles(display);
        }

        public bool IsValidTargetTile(MapTile toTarget)
        {
            return rMapManager.IsValidActionTile(toTarget);
        }

        public void CalculateTilePaths(CharacterManager toMove, bool forCinematic = false)
        {
            rMapManager.CalculatePathTiles(toMove, forCinematic);
        }

        public void DisplayMovementTiles(bool display)
        {
            rMapManager.DisplayMovementTiles(display);
        }

        public bool IsValidMoveTile(MapTile toMoveTo)
        {
            return rMapManager.IsValidMoveTile(toMoveTo);
        }

        public List<MapTile> GetValidMovementTiles()
        {
            return rMapManager.GetValidMovementTiles();
        }

        public Dictionary<MapTile, TilePath> GetValidMapPaths()
        {
            return rMapManager.GetValidMapPaths();
        }

        public List<MapTile> GetValidActionTiles()
        {
            return rMapManager.GetActionTiles();
        }

        public List<MapTile> GetValidActionAOETiles()
        {
            return rMapManager.GetActionAOETiles();
        }

        #endregion
    }
}
