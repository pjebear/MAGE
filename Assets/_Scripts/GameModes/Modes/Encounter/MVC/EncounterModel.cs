using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounters;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class EncounterModel
    { 
        public bool IsEncounterActive = false;

        public List<EncounterCondition> mWinConditions = new List<EncounterCondition>();
        public List<EncounterCondition> mLoseConditions = new List<EncounterCondition>();

        public Dictionary<CombatEntity, ActionProposal> mChargingActions = new Dictionary<CombatEntity, ActionProposal>();
        public Queue<ActionProposal> mActionQueue = new Queue<ActionProposal>();

        public HashSet<TemporaryEntity> mTemporaryEntities = new HashSet<TemporaryEntity>();

        public Dictionary<TeamSide, List<ControllableEntity>> Teams = new Dictionary<TeamSide, List<ControllableEntity>>();
        public Dictionary<int, ControllableEntity> Players = new Dictionary<int, ControllableEntity>();
        public IEnumerable<ControllableEntity> AlivePlayers { get { return Players.Values.Where(x => x.GetComponent<ResourcesControl>().IsAlive()); } }

        public List<ControllableEntity> TurnQueue = new List<ControllableEntity>();

        public ControllableEntity CurrentTurn = null;
        public bool TurnComplete = false;

        public bool IsEncounterLost()
        {
            bool isLost = true;

            isLost = mLoseConditions.Where(x => x.IsConditionMet(this)).Count() > 0;

            return isLost;
        }

        public bool IsEncounterWon()
        {
            bool isWon = mWinConditions.Where(x => x.IsConditionMet(this)).Count() > 0;

            return isWon;
        }

        public bool IsEncounterOver()
        {
            return IsEncounterLost() || IsEncounterWon();
        }

        public void AddTemporaryEntity(TemporaryEntity temporaryEntity)
        {
            mTemporaryEntities.Add(temporaryEntity);
        }

        public void RemoveTemporaryEntity(TemporaryEntity temporaryEntity)
        {
            mTemporaryEntities.Remove(temporaryEntity);
        }

        public void AddPlayer(ControllableEntity player)
        {
            Logger.Log(LogTag.Combat, "EncounterModel", string.Format("Add Player [{0}] Exists in Lineup [{1}]", player.Id, Players.ContainsKey(player.Id) ? "TRUE" : "FALSE"));

            if (!Players.ContainsKey(player.Id))
            {
                Players.Add(player.Id, player);

                if (!Teams.ContainsKey(player.TeamSide)) Teams.Add(player.TeamSide, new List<ControllableEntity>());
                Teams[player.TeamSide].Add(player);

                if (player.TeamSide == TeamSide.EnemyAI)
                {
                    player.GetComponentInChildren<ActorOutfitter>()?.SetOutfitColorization(GameSystems.Appearances.OutfitColorization.Enemy);
                }
            }
        }

        public void RemovePlayer(int playerId)
        {
            if (Players.ContainsKey(playerId))
            {
                ControllableEntity entity = Players[playerId];
                Players.Remove(playerId);

                Teams[entity.TeamSide].Remove(entity);

                mActionQueue.ToList().RemoveAll(x => x.Proposer == entity);
                mChargingActions.Remove(entity);
                TurnQueue.Remove(entity);

                if (CurrentTurn == entity)
                {
                    CurrentTurn = null;
                    TurnComplete = false;
                }

                GameObject.Destroy(entity.gameObject);
            }
        }
    }
}


