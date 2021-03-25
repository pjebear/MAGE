using MAGE.GameModes.Combat;
using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounters;
using MAGE.GameModes.SceneElements.Navigation;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Loot;
using MAGE.GameSystems.World;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace MAGE.GameModes.Encounter
{
    class EncounterFlowControl : FlowControlBase
    {
        private string TAG = "EncounterFlowControl";

        enum FlowState
        {
            Intro,
            //UnitPlacement,
            //Init,

            CoreLoopBegin,
            StatusIncrement = CoreLoopBegin,
            StatusCheck,
            ActionIncrement,
            ActionResolution,
            TurnIncrement,
            TurnResolution,
            CoreLoopEnd,

            Outro,

            NUM
        }

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Encounter;
        }

        protected override void Setup()
        {
            Level level = LevelManagementService.Get().GetLoadedLevel();
            EncounterContainer activeEncounter = level.GetActiveEncounter();

            GameModel.Encounter = new EncounterModel();

            List<CombatCharacter> allies = activeEncounter.Allys.GetComponentsInChildren<CombatCharacter>().ToList();
            GameModel.Encounter.Players.AddRange(allies);
            foreach (CombatCharacter character in allies)
            {
                character.GetComponent<CombatEntity>().TeamSide = TeamSide.AllyHuman;
            }
            List<CombatCharacter> enemies = activeEncounter.Enemies.GetComponentsInChildren<CombatCharacter>().ToList();
            GameModel.Encounter.Players.AddRange(enemies);
            foreach (CombatCharacter character in enemies)
            {
                character.GetComponent<CombatEntity>().TeamSide = TeamSide.EnemyAI;
            }

            GameModel.Encounter.TurnQueue.AddRange(GameModel.Encounter.Players);

            foreach (CombatCharacter combatCharacter in GameModel.Encounter.Players)
            {
                combatCharacter.GetComponent<ActorMotor>().Enable(false);
            }
        }

        protected override void Cleanup()
        {
            Level level = LevelManagementService.Get().GetLoadedLevel();
            EncounterContainer activeEncounter = level.GetActiveEncounter();
            Destroy(activeEncounter.gameObject);
        }

        public override bool Notify(string notifyEvent)
        {
            bool handled = false;

            switch (notifyEvent)
            {
                case "progressTurnFlow":
                {
                    ProgressTurnFlow();
                    handled = true;
                }
                break;

            }

            return handled;
        }

        public override string Query(string queryParam)
        {
            string result = "";

            switch (queryParam)
            {
                case "encounterFlowState":
                {
                    if (IsEncounterOver())
                    {
                        result = "encounterComplete";
                    }
                    else if (AreActionsPending())
                    {
                        result = "actionResolution";
                    }
                    else
                    {
                        result = "playerTurnFlow";
                    }
                }
                break;
            }

            return result;
        }

        public override string Condition(string condition)
        {
            string result = FlowConstants.CONDITION_EMPTY;

            switch (condition)
            {
                case "havePendingActions":
                {
                    result = GameModel.Encounter.mActionQueue.Count > 0
                        ? FlowConstants.CONDITION_TRUE
                        : FlowConstants.CONDITION_FALSE;
                }
                break;
                case "isCurrentTurnPlayControlled":
                {
                    result = GameModel.Encounter.CurrentTurn.GetComponent<CombatEntity>().TeamSide == TeamSide.AllyHuman
                        ? FlowConstants.CONDITION_TRUE
                        : FlowConstants.CONDITION_FALSE;
                }
                break;
            }

            return result;
        }

        private bool IsEncounterOver()
        {
            //check for win conditions
            int aliveEnemies = 0;
            int aliveAllies = 0;
            foreach (CombatCharacter combatCharacter in GameModel.Encounter.Players)
            {
                if (combatCharacter.GetComponent<ResourcesControl>().IsAlive())
                {
                    if (combatCharacter.GetComponent<CombatEntity>().TeamSide == TeamSide.AllyHuman)
                    {
                        ++aliveAllies;
                    }
                    else
                    {
                        ++aliveEnemies;
                    }
                }
            }

            return aliveEnemies == 0 || aliveAllies == 0;
        }

        private bool AreActionsPending()
        {
            return GameModel.Encounter.mActionQueue.Count > 0;
        }

        private void ProgressTurnFlow()
        {
            if (GameModel.Encounter.CurrentTurn != null)
            {
                if (!GameModel.Encounter.CurrentTurn.GetComponent<ResourcesControl>().IsAlive() 
                    || (GameModel.Encounter.HasActed && GameModel.Encounter.HasMoved))
                {
                    GameModel.Encounter.CurrentTurn.GetComponent<ActorMotor>().Enable(false);
                    GameModel.Encounter.CurrentTurn = null;
                }
            }

            // Clean up existing turn queue
            if (GameModel.Encounter.CurrentTurn == null)
            {
                if (GameModel.Encounter.TurnQueue.Count > 0)
                {
                    GameModel.Encounter.TurnQueue = GameModel.Encounter.TurnQueue.Where(x => x.GetComponent<ResourcesControl>().IsAlive()).ToList();
                }

                if (GameModel.Encounter.TurnQueue.Count == 0)
                {
                    GameModel.Encounter.TurnQueue = GameModel.Encounter.Players.Where(x => x.GetComponent<ResourcesControl>().IsAlive()).ToList();
                }

                GameModel.Encounter.CurrentTurn = GameModel.Encounter.TurnQueue[0];
                GameModel.Encounter.CurrentTurn.OnTurnStart();
                GameModel.Encounter.CurrentTurn.GetComponent<ActorMotor>().Enable(true);
                GameModel.Encounter.TurnQueue.RemoveAt(0);
                GameModel.Encounter.HasMoved = false;
                GameModel.Encounter.HasActed = false;
                
            }
        }
    }
}


