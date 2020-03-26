using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class TalentFactory
{
    private static string TAG = "TalentFactory";
    public static Talent CheckoutTalent(TalentId talentId, int pointsAssigned = 0)
    {
        Talent talent = null;
        switch (talentId)
        {
            case (TalentId.BlockIncrease):
                talent = new BlockIncreaseTalent();
                break;

            case (TalentId.HealIncrease):
                talent = new HealIncreaseTalent();
                break;

            case (TalentId.MightyBlow):
                talent = new MightyBlowTalent();
                break;

            case (TalentId.HealOnHurt):
                talent = new HealOnHurtTalent();
                break;
        }

        Logger.Assert(pointsAssigned <= talent.MaxPoints, LogTag.Character, TAG,
            string.Format("::CheckoutTalent({0}) assignedPoints [{1}] exceeds maxPoints[{2}]", talentId.ToString(), pointsAssigned, talent.MaxPoints));
        if (pointsAssigned > talent.MaxPoints)
        {
            pointsAssigned = talent.MaxPoints;
        }
        talent.PointsAssigned = pointsAssigned;

        return talent;
    }
}

