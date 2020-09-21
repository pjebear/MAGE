using MAGE.GameModes.LevelManagement;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems.Characters;

using MAGE.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.World.Internal
{
    class EncounterSystem
    {
        private string TAG = "EncounterSystem";
        private EncounterContext mPreparedContext;
        private EncounterCreateParams mEncounterParams;

        public void PrepareEncounter(EncounterCreateParams encounterParams)
        {
            mEncounterParams = encounterParams;

            Logger.Assert(mPreparedContext == null, LogTag.GameSystems, TAG, "EncounterContext already prepared", LogLevel.Warning);

            if (encounterParams.ScenarioId == EncounterScenarioId.Random)
            {
                mPreparedContext = CreateRandomEncounter(encounterParams);
            }
            else if (encounterParams.ScenarioId == EncounterScenarioId.Scenario)
            {
                EncounterContext scenarioContext = new EncounterContext();

                scenarioContext.EncounterType = EncounterType.Scenario;

                scenarioContext.WinConditions = new List<EncounterCondition>()
            {
                new TeamDefeatedCondition(TeamSide.EnemyAI)
            };

                scenarioContext.LoseConditions = new List<EncounterCondition>()
            {
                new TeamDefeatedCondition(TeamSide.AllyHuman)
                ,new UnitHealthCondition((int)StoryCharacterId.Lothar, 0, Operator.LessEqual)
            };

                // Allys
                {
                    DBService.Get().AddToTeam((int)StoryCharacterId.Lothar, TeamSide.AllyHuman); scenarioContext.CharacterPositions.Add((int)StoryCharacterId.Lothar, new TileIdx(4, 3));
                }

                // Create enemy team
                {
                    int enemyId = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 0);
                    DBService.Get().AddToTeam(enemyId, TeamSide.EnemyAI); scenarioContext.CharacterPositions.Add(enemyId, new TileIdx(6, 5));
                    enemyId = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 1);
                    DBService.Get().AddToTeam(enemyId, TeamSide.EnemyAI); scenarioContext.CharacterPositions.Add(enemyId, new TileIdx(3, 6));
                    enemyId = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 2);
                    DBService.Get().AddToTeam(enemyId, TeamSide.EnemyAI); scenarioContext.CharacterPositions.Add(enemyId, new TileIdx(7, 0));
                }

                // Rewards
                scenarioContext.CurrencyReward = UnityEngine.Random.Range(0, 500);
                scenarioContext.ItemRewards.Add((ItemId)UnityEngine.Random.Range(0, (int)ItemId.NUM));

                scenarioContext.LevelId = LevelId.Forest;
                scenarioContext.BottomLeft = new TileIdx(20, 3);
                scenarioContext.TopRight = new TileIdx(29, 12);

                mPreparedContext = scenarioContext;
            }
        }

        private EncounterContext CreateRandomEncounter(EncounterCreateParams createParams)
        {
            EncounterContext randomContext = new EncounterContext();

            randomContext.EncounterType = EncounterType.Random;

            randomContext.WinConditions = new List<EncounterCondition>()
        {
            new TeamDefeatedCondition(TeamSide.EnemyAI)
        };

            randomContext.LoseConditions = new List<EncounterCondition>()
        {
            new TeamDefeatedCondition(TeamSide.AllyHuman)
        };

            int encounterCharacterId = CharacterConstants.TEMPORARY_CHARACTER_ID_OFFSET;
            // Create enemy team
            {
                DBService.Get().AddToTeam((int)StoryCharacterId.Maric, TeamSide.EnemyAI);
            }

            // Rewards
            randomContext.CurrencyReward = UnityEngine.Random.Range(0, 500);
            randomContext.ItemRewards.Add((ItemId)UnityEngine.Random.Range(0, (int)ItemId.NUM));

            randomContext.LevelId = createParams.LevelId;
            randomContext.BottomLeft = createParams.BottomLeft;
            randomContext.TopRight = createParams.TopRight;

            return randomContext;
        }

        public EncounterContext GetEncounterContext()
        {
            Logger.Assert(mPreparedContext != null, LogTag.GameSystems, TAG, "EncounterContext not prepared", LogLevel.Warning);
            if (mPreparedContext == null)
            {
                return new EncounterContext();
            }

            return mPreparedContext;
        }

        public EncounterCreateParams GetParams()
        {
            return mEncounterParams;
        }

        public void CleanupEncounter()
        {
            List<int> enemyCharacters = DBService.Get().LoadTeam(TeamSide.EnemyAI);

            foreach (int enemy in enemyCharacters)
            {
                if (CharacterUtil.GetCharacterTypeFromId(enemy) == CharacterType.Temporary)
                {
                    DBService.Get().RemoveCharacter(enemy);
                }
            }

            DBService.Get().ClearTeam(TeamSide.EnemyAI);

            mPreparedContext = null;
        }
    }
}


