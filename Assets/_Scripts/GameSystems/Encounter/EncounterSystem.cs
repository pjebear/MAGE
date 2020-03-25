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

        mPreparedContext = new EncounterContext();

        mPreparedContext.WinConditions = new List<EncounterCondition>()
        {
            new TeamDefeatedCondition(TeamSide.EnemyAI)
        };

        mPreparedContext.LoseConditions = new List<EncounterCondition>()
        {
            new TeamDefeatedCondition(TeamSide.AllyHuman)
        };

        int encounterCharacterId = CharacterConstants.TEMPORARY_CHARACTER_ID_OFFSET;
        // Create enemy team
        {
            DB.DBCharacter maric = CharacterUtil.CreateBaseCharacter(encounterCharacterId++, "Maric", SpecializationType.Footman,
                   new List<int>() { (int)EquippableId.ChainArmor_0, (int)EquippableId.Shield_0, (int)EquippableId.Sword_0, (int)EquippableId.INVALID });

            DB.DBHelper.WriteCharacter(maric, TeamSide.EnemyAI);

            //DB.DBHelper.WriteNewCharacter(
            //    CreateBaseCharacter(
            //        "Asmund",
            //        SpecializationType.Monk,
            //        new List<int>() { (int)EquippableId.ClothArmor_0, (int)EquippableId.Staff_0, (int)EquippableId.INVALID, (int)EquippableId.INVALID }),
            //    TeamSide.EnemyAI);
        }
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

