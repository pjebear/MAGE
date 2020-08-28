using MAGE.GameSystems.Characters;
using MAGE.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.World
{
    abstract class EncounterCondition
    {
        public abstract bool IsConditionMet(GameModes.Encounter.EncounterModel model);
        public abstract override string ToString();
    }

    class UnitHealthCondition : EncounterCondition
    {
        int mUnitId;
        float mHealthPercent;
        Operator Operator;

        public UnitHealthCondition(int unitId, float healthPercent, Operator op)
        {
            mUnitId = unitId;
            mHealthPercent = healthPercent;
            Operator = op;
        }

        public override bool IsConditionMet(GameModes.Encounter.EncounterModel model)
        {
            Debug.Assert(model.Characters.ContainsKey(mUnitId));

            float healthPercent = model.Characters[mUnitId].CurrentResources[ResourceType.Health].Ratio;

            return Condition.Compare(healthPercent, mHealthPercent, Operator);
        }

        public override string ToString()
        {
            return string.Format("UnitHealthCondition: Unit[{0}] Health {1} {2}", mUnitId, Operator.ToString(), mHealthPercent);
        }
    }

    class TeamDefeatedCondition : EncounterCondition
    {
        TeamSide mTeam;

        public TeamDefeatedCondition(TeamSide team)
        {
            mTeam = team;
        }

        public override bool IsConditionMet(GameModes.Encounter.EncounterModel model)
        {
            Debug.Assert(model.Teams.ContainsKey(mTeam));

            bool conditionMet = true;
            if (model.Teams.ContainsKey(mTeam))
            {
                foreach (Character character in model.Teams[mTeam])
                {
                    float healthPercent = character.CurrentResources[ResourceType.Health].Ratio;

                    conditionMet &= healthPercent <= 0;
                }
            }

            return conditionMet;
        }

        public override string ToString()
        {
            return string.Format("TeamDefeatedCondition: Team[{0}] ", mTeam.ToString());
        }
    }
}
    
