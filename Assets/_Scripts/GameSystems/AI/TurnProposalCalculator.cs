using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.AI
{
    static class TurnProposalCalculator
    {
        public static string TAG = "TurnProposalCalculator";
        public static TurnProposal CalculateTurnProposal(Character character, Map map)
        {
            TurnProposal turnProposal = new TurnProposal();

            MapPathFinder mapPathFinder = new MapPathFinder();

            CharacterPosition currentPosition = map.GetCharacterPosition(character);

            List<TileIdx> tilesAvailableToActFrom = new List<TileIdx>();
            if (character.CanMove)
            {
                mapPathFinder.CalculatePaths(
                    map, 
                    currentPosition.Location, 
                    character.TeamSide, 
                    character.CurrentAttributes[TertiaryStat.Movement],
                    character.CurrentAttributes[TertiaryStat.Jump]);
                tilesAvailableToActFrom = mapPathFinder.GetPossibleTiles();
            }
            // Can act from where character is standing
            tilesAvailableToActFrom.Add(currentPosition.Location);

            if (character.CanAct)
            {
                List<ActionId> possibleActions = character.GetActionIds();
                // TODO: Filter actions the user doesn't have resources for

                List<ActionInfo> actionInfos = new List<ActionInfo>();
                foreach (ActionId actionId in character.GetActionIds())
                {
                    actionInfos.Add(character.GetActionInfo(actionId));
                }

                ActionTileCalculator actionTileCalculator = new ActionTileCalculator(map);

                List<TurnProposal> turnProposals = new List<TurnProposal>();
                foreach (TileIdx tileIdx in tilesAvailableToActFrom)
                {
                    map.UpdatePosition(character, new CharacterPosition() { Location = tileIdx });

                    List<TileIdx> path = mapPathFinder.GetPathTo(tileIdx);

                    foreach (ActionInfo actionInfo in actionInfos)
                    {
                        List<TileIdx> possibleCastPoints = new List<TileIdx>(); 
                        if (actionInfo.IsSelfCast)
                        {
                            possibleCastPoints.Add(tileIdx);
                        }
                        else
                        {
                            possibleCastPoints = actionTileCalculator.CalculateTilesInRange(tileIdx, tileIdx, actionInfo.CastRange);
                        }

                        foreach (TileIdx castPoint in possibleCastPoints)
                        {
                            List<TileIdx> castArea = actionTileCalculator.CalculateTilesInRange(tileIdx, castPoint, actionInfo.EffectRange);
                            List<Character> targetedByCast = map.GetCharactersOnTiles(castArea);
                            if (targetedByCast.Count > 0)
                            {
                                Logger.Log(LogTag.AI, TAG, 
                                    string.Format("Calculating value of action {0} standing on tileIdx {1} and targeting tile {2}", 
                                    actionInfo.ActionId.ToString(), 
                                    tileIdx.ToString(),
                                    castPoint.ToString()));

                                CharacterPosition positionWhileCasting = new CharacterPosition(tileIdx, Orientation.Forward);
                                float value = 0;
                                

                                foreach (Character targeted in targetedByCast)
                                {
                                    Logger.Log(LogTag.AI, TAG,
                                    string.Format("\t Target: {0} Position: {1} Orientation: {2} RelativeOrientation: {3}",
                                    targeted.Name,
                                    map.GetCharacterPosition(targeted).Location.ToString(),
                                    map.GetCharacterPosition(targeted).Orientation.ToString(),
                                    InteractionUtil.GetRelativeOrientation(character, targeted, map).ToString()));
                                }

                                float healthChangePolicy = HealthChangePolicy.CalculateActionValue(character, positionWhileCasting, actionInfo, targetedByCast, map);
                                Logger.Log(LogTag.AI, TAG,
                                    string.Format("\t HealthChangePolicy: Value: {0} ",
                                    value));
                                float percentPolicy = HealthPercentPolicy.CalculateActionValue(character, positionWhileCasting, actionInfo, targetedByCast, map, false);
                                Logger.Log(LogTag.AI, TAG,
                                    string.Format("\t HealthPercentPolicy: Value:{0} ",
                                    value));
                                float orientationPolicy = AvoidFrontalOrientationPolicy.CalculateActionValue(character, positionWhileCasting, actionInfo, targetedByCast, map);
                                Logger.Log(LogTag.AI, TAG,
                                    string.Format("\t AvoidFrontalOrientationPolicy: Value: {0} ",
                                    value));
                                value += healthChangePolicy;
                                value += percentPolicy;
                                value += orientationPolicy;

                                Logger.Log(LogTag.AI, TAG,
                                    string.Format("\t \t Value: {0}",
                                    value));

                                if (value > 0)
                                {
                                    TurnProposal proposal = new TurnProposal();
                                    proposal.MovementProposal = path;
                                    proposal.ActionId = actionInfo.ActionId;
                                    proposal.CastTile = tileIdx;
                                    proposal.ActionTarget = new TargetSelection(new Target(castPoint), actionInfo.EffectRange);
                                    proposal.Value = value;
                                    turnProposals.Add(proposal);
                                }
                            }
                            
                        }
                    }
                }

                if (turnProposals.Count > 0)
                {
                    turnProposal = turnProposals[0];
                    foreach (TurnProposal proposal in turnProposals)
                    {
                        if (proposal.Value > turnProposal.Value)
                        {
                            turnProposal = proposal;
                        }
                    }
                }
            }

            map.UpdatePosition(character, currentPosition);
            // Couldn't find an action or can move after
            if (turnProposal.MovementProposal.Count == 0)
            {

            }

            return turnProposal;
        }
    }
}
