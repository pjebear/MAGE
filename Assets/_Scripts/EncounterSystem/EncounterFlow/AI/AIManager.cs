using System;
using System.Collections.Generic;
using System.Collections;

using System.Linq;
using System.Text;

using Common.ActionEnums;
namespace EncounterSystem
{
    using Character;
    using Screen;
    using Map;
    using UnityEngine;
    using AITypes;
    using Common.AI;
    using Common.ActionTypes;
    using Common.AttributeEnums;

    using Action;
    using MapTypes;
    using Common.EncounterTypes;

    namespace EncounterFlow
    {
        namespace AI
        {
            class AIManager
            {
                Dictionary<CharacterManager, PotentialTurnAction> mPotentialTurnActions;
                InputManager rInputManager;
                PlayerInputViewController rInputViewController;
                EncounterState rEncounterState;

                CharacterManager rCurrentCharacter;


                public AIManager()
                {
                    mPotentialTurnActions = new Dictionary<CharacterManager, PotentialTurnAction>();
                }

                public void Initialize(InputManager inputManager, EncounterState currentState, PlayerInputViewController viewController)
                {
                    rInputManager = inputManager;
                    rInputViewController = viewController;
                    rEncounterState = currentState;
                }

                public void GetInputForUnit()
                {
                    if (rCurrentCharacter != rEncounterState.CurrentUnit)
                    {
                        rCurrentCharacter = rEncounterState.CurrentUnit;
                        mPotentialTurnActions[rEncounterState.CurrentUnit] = CalculateActionForTurn(rEncounterState.CurrentUnit);
                    }
                    ProgressTurn();
                }

                public void ProgressTurn()
                {
                    bool performedTurnAction = false;
                    rInputViewController.DisplayCharacter(rCurrentCharacter);
                    PotentialTurnAction turnAction = mPotentialTurnActions[rCurrentCharacter];
                    if (rCurrentCharacter.CanAct)
                    {
                        if (turnAction.Action != null)
                        {
                            if (turnAction.NeedsMovementForAction)
                            {
                                if (turnAction.MovementLocation != rCurrentCharacter.GetCurrentTile())
                                {
                                    rCurrentCharacter.StartCoroutine(_Move(turnAction.MovementLocation));
                                    performedTurnAction = true;
                                }
                                else
                                {
                                    rCurrentCharacter.StartCoroutine(_Act(turnAction.ActionTarget, turnAction.Action));
                                    performedTurnAction = true;
                                }
                            }
                            else
                            {
                                rCurrentCharacter.StartCoroutine(_Act(turnAction.ActionTarget, turnAction.Action));
                                performedTurnAction = true;
                            }
                        }
                    }
                    else if (rCurrentCharacter.CanMove)
                    {
                        if (turnAction.MovementLocation != null && turnAction.MovementLocation != rCurrentCharacter.GetCurrentTile())
                        {
                            rCurrentCharacter.StartCoroutine(_Move(turnAction.MovementLocation));
                            performedTurnAction = true;
                        }
                    }
                    
                    if (!performedTurnAction)
                    {
                        FinishTurn();
                    }

                }

                private void FinishTurn()
                {
                    rInputViewController.HideCharacterDisplay();
                    rCurrentCharacter = null;
                    rInputManager.WaitSelected();
                }

                private PotentialTurnAction CalculateActionForTurn(CharacterManager currentCharacter)
                {
                    PotentialTurnAction actionForTurn = new PotentialTurnAction();

                    rInputManager.CalculateTilePaths(rCurrentCharacter);
                    Dictionary<MapTile, TilePath> mapPaths = rInputManager.GetValidMapPaths();
                    List<MapTile> potentialMovementTiles = rInputManager.GetValidMovementTiles();

                    List<CharacterManager> enemies = rEncounterState.GetUnitGroup(Common.CharacterEnums.UnitGroup.Player);
                    List<CharacterManager> allies = rEncounterState.GetUnitGroup(Common.CharacterEnums.UnitGroup.AI);
                    Vector3 generalAlliedPosition = GetGeneralPositionOfUnitGroup(allies);
                    Vector3 generalEnemyPosition = GetGeneralPositionOfUnitGroup(enemies);

                    float advanceModifier = CalculateDesireToMoveForward(generalAlliedPosition, generalEnemyPosition, rCurrentCharacter.GetCurrentTile().transform.localPosition,
                        rCurrentCharacter.Resources[(int)Resource.Health] / rCurrentCharacter.Resources[(int)Resource.MaxHealth],
                        PositioningType.FrontLine, AggressionType.Normal);

                    if (rCurrentCharacter.CanAct)
                    {
                        Dictionary<ActionRootEffect, List<ActionBase>> availableActions = rCurrentCharacter.GetAIActionPayload();
                        List<MapTile> potentialCastTiles = new List<MapTile>();
                        potentialCastTiles.Add(rCurrentCharacter.GetCurrentTile()); // character can always cast from its own tile

                        if (rCurrentCharacter.CanMove)
                        {
                            potentialCastTiles.AddRange(potentialMovementTiles);
                        }

                        List<PotentialTurnAction> potentialActions = CalculatePotentialActionsForTurn(rCurrentCharacter, allies, enemies, potentialCastTiles, availableActions);

                        if (potentialActions.Count > 0)
                        {
                            //if (rCurrentCharacter.CanMove) // modify priorities to coincide with @desireToMoveForward
                            //{
                            //    for (int potentialActionIdx = 0; potentialActionIdx < potentialActions.Count; ++potentialActionIdx)
                            //    {
                            //        PotentialTurnAction potentialAction = potentialActions[potentialActionIdx];
                            //        MapTile castTile = potentialAction.MovementLocation;
                            //        if (castTile != rCurrentCharacter.GetCurrentTile()) // Need to move to perform the action
                            //        {
                            //            //float forwardModifier = GetNormalizedDistanceToEnemyLine(generalAlliedPosition, generalEnemyPosition, castTile.transform.localPosition);
                            //            float theta = Vector3.Dot(generalEnemyPosition - rCurrentCharacter.GetCurrentTile().transform.localPosition,generalEnemyPosition - rCurrentCharacter.GetCurrentTile().transform.localPosition);

                            //            if ((Mathf.Cos(theta) > 0) ^ (advanceModifier >= 0))
                            //            {
                            //                potentialAction.Priority -= 0.5f;
                            //                potentialActions[potentialActionIdx] = potentialAction;
                            //            }
                            //        }
                            //    }
                            //} // too much theory crafting right now. Hone the 
                            potentialActions.Sort((x, y) => y.Priority.CompareTo(x.Priority));
                            PotentialTurnAction chosenAction = potentialActions[0];

                            if (!chosenAction.NeedsMovementForAction)
                            {
                                // chose a movement tile for after casting
                                chosenAction.MovementLocation = CalculateTileToMoveTo(potentialMovementTiles, mapPaths, rCurrentCharacter.GetCurrentTile(), generalEnemyPosition, generalAlliedPosition, 
                                    advanceModifier, rCurrentCharacter.Stats[(int)TertiaryStat.Movement]);
                            }

                            actionForTurn = chosenAction;
                        }
                        else if (rCurrentCharacter.CanMove)
                        {
                            // move forward
                            actionForTurn.MovementLocation = CalculateTileToMoveTo(potentialMovementTiles, mapPaths, rCurrentCharacter.GetCurrentTile(), generalEnemyPosition, generalAlliedPosition,
                                    advanceModifier, rCurrentCharacter.Stats[(int)TertiaryStat.Movement]);
                        }
                        else
                        {
                            // can't do anything
                        }
                    }
                    else if (rCurrentCharacter.CanMove)
                    {
                        // move forward
                        actionForTurn.MovementLocation = CalculateTileToMoveTo(potentialMovementTiles, mapPaths, rCurrentCharacter.GetCurrentTile(), generalEnemyPosition, generalAlliedPosition,
                                    advanceModifier, rCurrentCharacter.Stats[(int)TertiaryStat.Movement]);
                    }
                    else
                    {
                        // can't do anything
                    }
                    return actionForTurn; // null
                }

                private MapTile CalculateTileToMoveTo(List<MapTile> tilesToChooseFrom, Dictionary<MapTile, TilePath> mapPaths, MapTile currentTile, Vector3 generalEnemyLocation, Vector3 generalAllyLocation,  float desireToMoveForward, float maxMoveRange)
                {
                    MapTile destination = null;
                    maxMoveRange = Mathf.Clamp(Mathf.Ceil(Mathf.Abs(desireToMoveForward) * maxMoveRange), 0, maxMoveRange);
                    if (maxMoveRange > 0) 
                    {
                        if (desireToMoveForward > 0)
                        {
                            MapTile closestTile = null;
                            float closestDistance = float.MaxValue;
                            // pick a tile path that will move character closer to enemy location
                            foreach (MapTile tile in mapPaths.Keys)
                            {
                                float sqrDistance = (tile.transform.localPosition - generalEnemyLocation).sqrMagnitude;
                                if (sqrDistance < closestDistance)
                                {
                                    closestTile = tile;
                                    closestDistance = sqrDistance;
                                }
                            }

                            if (closestTile != null) // move along that path as far as the desire to move will allow
                            {
                                while (closestTile != null)
                                {
                                    TilePath path = mapPaths[closestTile];
                                    if (path.pathLength <= maxMoveRange && tilesToChooseFrom.Contains(closestTile))
                                    {
                                        destination = closestTile;
                                        break;
                                    }
                                    closestTile = path.previous;
                                }
                            }
                        }
                        else if (desireToMoveForward < 0)
                        {
                            // choose what ever tile behind the character is withing range
                            MapTile farthestTile = null;
                            float farthestDistance = float.MinValue;
                            // pick a tile path that will move character away from the enemy 
                            foreach (MapTile tile in tilesToChooseFrom)
                            {
                                float sqrDistance = (tile.transform.localPosition - generalEnemyLocation).sqrMagnitude;
                                if (sqrDistance > farthestDistance)
                                {
                                    farthestTile = tile;
                                    farthestDistance = sqrDistance;
                                }
                            }

                            if (farthestTile != null) // move along that path as far as the desire to move will allow
                            {
                                
                                while (farthestTile != null)
                                {
                                    TilePath path = mapPaths[farthestTile];
                                    if (path.pathLength <= maxMoveRange && tilesToChooseFrom.Contains(farthestTile))
                                    {
                                        destination = farthestTile;
                                        break;
                                    }
                                    farthestTile = path.previous;
                                }
                            }
                        }
                    }// else stay put
                    

                    return destination;
                }

                private List<PotentialTurnAction> CalculatePotentialActionsForTurn(CharacterManager currentCharacter, List<CharacterManager> allies, List<CharacterManager> enemies, List<MapTile> potentialCastTiles, Dictionary<ActionRootEffect, List<ActionBase>> actions)
                {
                    Dictionary<ActionBase, PotentialTurnAction> turnActions = new Dictionary<ActionBase, PotentialTurnAction>();
                   
                    foreach (var actionGroup in actions)
                    {
                        if (actionGroup.Key == ActionRootEffect.Beneficial)
                        {
                            foreach (ActionBase beneficialAction in actionGroup.Value)
                            {
                                GetPotentialTargetsForAction(beneficialAction, currentCharacter, allies, potentialCastTiles, ref turnActions);
                            }
                        }
                        else if ((actionGroup.Key == ActionRootEffect.Harmful))
                        {
                            foreach (ActionBase harmfulAction in actionGroup.Value)
                            {
                                GetPotentialTargetsForAction(harmfulAction, currentCharacter, enemies, potentialCastTiles, ref turnActions);
                            }
                        }
                    }
                    return turnActions.Values.ToList();
                }

                // Assumes targets are relative to the type of action. 
                // Beneficial -> allies, Harmful -> enemies
                // Valid Cast tiles are all the tiles the character could potentially move to on this turn
                private void GetPotentialTargetsForAction(ActionBase action, CharacterManager actor, List<CharacterManager> targets, List<MapTile> validCastTiles, ref Dictionary<ActionBase, PotentialTurnAction> potentialActions)
                {
                    float maxRange = action.MapInteractionInfo.MaxRange + action.MapInteractionInfo.MaxAoe;
                    float minRange = action.MapInteractionInfo.MinRange + action.MapInteractionInfo.MinAoe;
                    float maxElevation = action.MapInteractionInfo.MaxAoeElevation + action.MapInteractionInfo.MaxRangeElevation;
                    // for each tile the caster could cast from 
                    foreach (MapTile castTile in validCastTiles)
                    {
                        // for each potential target for the action
                        foreach (CharacterManager target in targets)
                        {
                            // first gate to cull tiles, check if the target is within range of action + action aoe 
                            MapTile targetTile = target.GetCurrentTile();
                            Vector3 displacement = targetTile.transform.localPosition - castTile.transform.localPosition;
                            float elevationDifference = Mathf.Abs(displacement.y);
                            displacement.y = 0f;
                            float distanceToTarget = displacement.magnitude;
                            if (distanceToTarget <= maxRange && distanceToTarget >= minRange && elevationDifference <= maxElevation)
                            {
                                // first gate passed. 
                                // Actually calculate the range tiles when casting from @castTile, and for each 
                                rInputManager.CalculateActionTiles(castTile, action);
                                List<MapTile> potentialActionTargets = rInputManager.GetValidActionTiles();
                                foreach (MapTile actionTarget in potentialActionTargets)
                                {
                                    // do a prelim check against aoe
                                    displacement = actionTarget.transform.localPosition - targetTile.transform.localPosition;
                                    elevationDifference = displacement.y;
                                    displacement.y = 0f;
                                    distanceToTarget = displacement.magnitude;

                                    // second gate to cull tiles, check if the target is within range of the action target + aoe range 
                                    if (distanceToTarget <= action.MapInteractionInfo.MaxAoe && distanceToTarget >= action.MapInteractionInfo.MinAoe && elevationDifference <= action.MapInteractionInfo.MaxAoeElevation)
                                    {
                                        rInputManager.CalculateActionAOETiles(actionTarget, castTile, action);
                                        List<MapTile> aoeTiles = rInputManager.GetValidActionAOETiles();
                                        float priorityRating = 0f;
                                        List<CharacterManager> aoeTargets = new List<CharacterManager>();

                                        foreach (MapTile aoeTile in aoeTiles)
                                        {
                                            CharacterManager aoeTarget = aoeTile.GetCharacterOnTile();
                                            if (aoeTarget != null)
                                            {
                                                aoeTargets.Add(aoeTarget);
                                            }
                                        }
                                        priorityRating = action.GetActionPriority(actor, aoeTargets);
                                        if (priorityRating > 0)
                                        {
                                            if (potentialActions.ContainsKey(action))
                                            {
                                                if (potentialActions[action].Priority < priorityRating)
                                                {
                                                    potentialActions[action] = new PotentialTurnAction(castTile, action, actionTarget, priorityRating, actor.GetCurrentTile() != castTile);
                                                }
                                            }
                                            else
                                            {
                                                potentialActions.Add(action, new PotentialTurnAction(castTile, action, actionTarget, priorityRating, actor.GetCurrentTile() != castTile));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                private float GetNormalizedDistanceToEnemyLine(Vector3 generalAlliedPosition, Vector3 generalEnemyPosition, Vector3 currentPosition)
                {
                    Vector3 alliedDispositionFromEnemies = generalEnemyPosition - generalAlliedPosition;
                    Vector3 dispositionFromAllies = currentPosition - generalAlliedPosition;

                    float distanceBetweenLines = alliedDispositionFromEnemies.magnitude;
                    float dispositionInEncounter = Vector3.Dot(alliedDispositionFromEnemies, dispositionFromAllies);
                    float normalizedDistanceToEnemyLine = (distanceBetweenLines - dispositionInEncounter) / distanceBetweenLines;
                    return normalizedDistanceToEnemyLine;
                }

                private float CalculateDesireToMoveForward(Vector3 generalAlliedPosition, Vector3 generalEnemyPosition, Vector3 currentPosition, float percentHealth, PositioningType positioningPreference, AggressionType aggressionPreference)
                {
                    float desiredDirectionModifier = GetNormalizedDistanceToEnemyLine(generalAlliedPosition, generalEnemyPosition, currentPosition);
                    float positioningModifier = AIConstants.GetPositionModifier(positioningPreference, desiredDirectionModifier);
                    float healthModifier = AIConstants.GetHealthModifier(percentHealth);
                    float aggressionModifier = AIConstants.GetAggressionModifier(aggressionPreference);

                    float desireToMoveForward = positioningModifier + healthModifier + aggressionModifier;

                    Debug.LogFormat("DesiredDirectionModifier: {0}\n PositionModifier: {1}\n HealthModifier: {2}\n AggressionModifier: {3}\n Total: {4}\n",
                        desiredDirectionModifier, positioningModifier, healthModifier, aggressionModifier, desireToMoveForward);

                    return desireToMoveForward;
                }

                private IEnumerator _Act(MapTile targetTile, ActionBase action)
                {
                    rInputManager.CalculateActionTiles(rCurrentCharacter.GetCurrentTile(), action);
                    rInputManager.DisplayActionTiles(true);
                    yield return new WaitForSeconds(1);
                    rInputManager.DisplayActionTiles(false);

                    rInputManager.CalculateActionAOETiles(targetTile, rCurrentCharacter.GetCurrentTile(), action);
                    rInputManager.DisplayActionAOETiles(true);
                    rInputViewController.DisplayTarget(targetTile.GetCharacterOnTile());
                    yield return new WaitForSeconds(1);
                    rInputManager.DisplayActionAOETiles(false);
                    rInputViewController.HideTargetDisplay();
                    rInputViewController.HideCharacterDisplay();
                    rInputManager.ActionChosen(action);
                }

                private IEnumerator _Move(MapTile destination)
                {
                    rInputManager.DisplayMovementTiles(true);
                    yield return new WaitForSeconds(1);
                    rInputViewController.HideCharacterDisplay();
                    rInputManager.DisplayMovementTiles(false);
                    rInputManager.MovementChosen(destination);
                }

                private Vector3 GetGeneralPositionOfUnitGroup(List<CharacterManager> units)
                {
                    Vector3 generalPosition = Vector3.zero;
                    float counter = 0;
                    foreach (CharacterManager character in units)
                    {
                        generalPosition += character.GetCurrentTile().transform.localPosition;
                        counter++;
                    }
                    if (counter > 0)
                    {
                        generalPosition /= counter;
                    }
                    return generalPosition;
                }
            }
        }
    }
}
