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
            DB.DBCharacter maric = CharacterUtil.CreateBaseCharacter(encounterCharacterId++, "Maric", SpecializationType.Footman,
                   new List<int>() { (int)EquippableId.ChainArmor_0, (int)EquippableId.Shield_0, (int)EquippableId.Sword_0, (int)EquippableId.INVALID });

            DB.DBHelper.WriteCharacter(maric, TeamSide.EnemyAI);
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
        DB.DBHelper.ClearTeam(TeamSide.EnemyAI, true);

        mPreparedContext = null;
    }
}

