using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

enum Operator
{
    Less,
    Equal,
    Greater,
    LessEqual,
    GreaterEqual,

    NUM
}

static class Condition
{
    public static bool Compare<T>(T lhs, T rhs, Operator op) where T : IComparable
    {
        bool retVal = false;

        int compareValue = lhs.CompareTo(rhs);
        switch (op)
        {
            case Operator.Less:
                retVal = compareValue < 0;
                break;

            case Operator.Greater:
                retVal = compareValue > 0;
                break;

            case Operator.Equal:
                retVal = compareValue == 0;
                break;

            case Operator.LessEqual:
                retVal = compareValue <= 0;
                break;

            case Operator.GreaterEqual:
                retVal = compareValue >= 0;
                break;
        }

        return retVal;
    }
}

abstract class EncounterCondition
{
    public abstract bool IsConditionMet(EncounterModel model);
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

    public override bool IsConditionMet(EncounterModel model)
    {
        Debug.Assert(model.Actors.ContainsKey(mUnitId));

        float healthPercent = model.Actors[mUnitId].Resources[ResourceType.Health].Current / (float)model.Actors[mUnitId].Resources[ResourceType.Health].Max;

        return Condition.Compare(healthPercent, mHealthPercent, Operator);
    }
}

class TeamDefeatedCondition : EncounterCondition
{
    TeamSide mTeam;

    public TeamDefeatedCondition(TeamSide team)
    {
        mTeam = team;
    }

    public override bool IsConditionMet(EncounterModel model)
    {
        Debug.Assert(model.Teams.ContainsKey(mTeam));

        bool conditionMet = true;
        if (model.Teams.ContainsKey(mTeam))
        {
            foreach (EncounterCharacter actor in model.Teams[mTeam])
            {
                float healthPercent = actor.Resources[ResourceType.Health].Ratio;

                conditionMet &= healthPercent <= 0;
            }
        }
        
        return conditionMet;
    }
}