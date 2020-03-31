using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Specialization
{
    public SpecializationType SpecializationType;
    public int Level;
    public int Experience;
    public List<ActionId> Actions = new List<ActionId>();
    public List<AuraType> Auras = new List<AuraType>();
    public List<ActionResponseId> ActionResponses = new List<ActionResponseId>();
    public List<ProficiencyType> Proficiencies = new List<ProficiencyType>();
    public List<AttributeModifier> LevelUpModifiers = new List<AttributeModifier>();
    public Dictionary<TalentId, Talent> Talents = new Dictionary<TalentId, Talent>();

    public int NumUnassignedTalentPoints()
    {
        return Level - NumTalentPointsAssigned();
    }

    public int NumTalentPointsAssigned()
    {
        int numAssigned = 0;
        foreach (Talent talent in Talents.Values)
        {
            numAssigned += talent.PointsAssigned;
        }
        return numAssigned;
    }
}

