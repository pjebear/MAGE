using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class EncounterSystem
{
    private string TAG = "EncounterSystem";
    private EncounterContext mPreparedContext;

    public void PrepareEncounter(EncounterCreateParams encounterParams)
    {
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
                DB.DBHelper.AddToTeam((int)StoryCharacterId.Lothar, TeamSide.AllyHuman); scenarioContext.CharacterPositions.Add((int)StoryCharacterId.Lothar, new TileIdx(24, 6));
            }

            // Create enemy team
            {
                int enemyId = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 0);
                DB.DBHelper.AddToTeam(enemyId, TeamSide.EnemyAI); scenarioContext.CharacterPositions.Add(enemyId, new TileIdx(26, 8));
                enemyId = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 1);
                DB.DBHelper.AddToTeam(enemyId, TeamSide.EnemyAI); scenarioContext.CharacterPositions.Add(enemyId, new TileIdx(23, 9));
                enemyId = CharacterUtil.ScenarioIdToDBId(ScenarioId.TheGreatHoldUp, 2);
                DB.DBHelper.AddToTeam(enemyId, TeamSide.EnemyAI); scenarioContext.CharacterPositions.Add(enemyId, new TileIdx(27, 3));
            }

            // Rewards
            scenarioContext.CurrencyReward = UnityEngine.Random.Range(0, 500);
            scenarioContext.ItemRewards.Add((ItemId)UnityEngine.Random.Range(0, (int)ItemId.NUM));

            scenarioContext.LevelId = LevelId.Forest;
            scenarioContext.BottomLeft = new TileIdx(20,3);
            scenarioContext.TopRight = new TileIdx(29,12);

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
            DB.DBHelper.AddToTeam((int)StoryCharacterId.Maric, TeamSide.EnemyAI);
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

    public void CleanupEncounter()
    {
        List<int> enemyCharacters = DB.DBHelper.LoadTeam(TeamSide.EnemyAI);

        foreach (int enemy in enemyCharacters)
        {
            if (CharacterUtil.GetCharacterTypeFromId(enemy) == CharacterType.Temporary)
            {
                DB.DBHelper.RemoveCharacter(enemy);
            }
        }

        DB.DBHelper.ClearTeam(TeamSide.EnemyAI);

        mPreparedContext = null;
    }
}

