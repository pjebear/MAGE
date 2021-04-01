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

            // TODO
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


