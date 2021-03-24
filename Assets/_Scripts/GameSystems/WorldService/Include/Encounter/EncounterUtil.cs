using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.World
{ 
    static class EncounterUtil
    {
        public static DB.DBEncounterInfo ToDB(EncounterInfo info)
        {
            DB.DBEncounterInfo db = new DB.DBEncounterInfo();
            db.Id = (int)info.EncounterScenarioId;
            db.Name = info.EncounterScenarioId.ToString();
            db.IsActive = info.IsActive;
            db.LevelId = (int)info.LevelId;

            return db;
        }

        public static EncounterInfo FromDB( DB.DBEncounterInfo db)
        {
            EncounterInfo info = new EncounterInfo();
            info.EncounterScenarioId = (EncounterScenarioId)db.Id;
            info.IsActive = db.IsActive;
            info.LevelId = (LevelId)db.LevelId;

            return info;
        }

        public static TeamSide GetOpponentTeamSide(TeamSide teamSide)
        {
            TeamSide opponentTeamSide = TeamSide.INVALID;

            switch (teamSide)
            {
                case TeamSide.AllyHuman:    opponentTeamSide = TeamSide.EnemyAI; break;
                case TeamSide.EnemyAI:      opponentTeamSide = TeamSide.AllyHuman; break;
                default: Debug.Assert(false); break;
            }

            return opponentTeamSide;
        }
    }
}
