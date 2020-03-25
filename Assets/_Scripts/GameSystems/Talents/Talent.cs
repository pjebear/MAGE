using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


abstract class Talent
{
    public TalentId TalentId { get; }
    public int PointsAssigned;
    public int MaxPoints { get; }

    protected Talent(TalentId talentId, int maxPoints)
    {
        TalentId = talentId;
        MaxPoints = maxPoints;
    }

    public virtual List<IActionModifier> GetActionModifiers() { return new List<IActionModifier>(); }
    public virtual List<AttributeModifier> GetAttributeModifiers() { return new List<AttributeModifier>(); }
    public virtual List<ActionId> GetActions() { return new List<ActionId>(); }
    public virtual List<AuraType> GetAuras() { return new List<AuraType>(); }
    public virtual List<ActionResponseId> GetActionResponses() { return new List<ActionResponseId>(); }
}

class BlockIncreaseTalent : Talent
{
    public float BlockIncreasePerPoint = 5;

    public BlockIncreaseTalent() 
        : base(TalentId.BlockIncrease, 3)
    {
        // empty;
    }

    public override List<AttributeModifier> GetAttributeModifiers()
    {
        List<AttributeModifier> modifiers = base.GetAttributeModifiers();

        if (PointsAssigned > 0)
        {
            modifiers.Add(
            new AttributeModifier(
                new AttributeIndex(AttributeCategory.Stat, (int)TertiaryStat.FrontalBlock)
                , ModifierType.Increment
                , BlockIncreasePerPoint * PointsAssigned));

            modifiers.Add(
               new AttributeModifier(
                   new AttributeIndex(AttributeCategory.Stat, (int)TertiaryStat.PeriferalBlock)
                   , ModifierType.Increment
                   , BlockIncreasePerPoint / 2 * PointsAssigned));
        }

        return modifiers;
    }
}

class HealOnHurtTalent : Talent
{
    public ActionResponseId HealOnHurtId = ActionResponseId.HealOnHurtListener;

    public HealOnHurtTalent()
        : base(TalentId.HealOnHurt, 1)
    {
        // empty;
    }

    public override List<ActionResponseId> GetActionResponses()
    {
        List<ActionResponseId> responseIds = base.GetActionResponses();

        if (PointsAssigned == MaxPoints)
        {
            responseIds.Add(HealOnHurtId);
        }

        return responseIds;
    }
}

class HealIncreaseTalent : Talent
{
    public float HealIncreasePerPoint = .25f;

    public HealIncreaseTalent()
        : base(TalentId.HealIncrease, 3)
    {
        // empty;
    }

    public virtual List<IActionModifier> GetActionModifiers()
    {
        List<IActionModifier> modifiers = base.GetActionModifiers();

        if (PointsAssigned > 0)
        {
            modifiers.Add(new HealModifier(HealIncreasePerPoint * PointsAssigned));
        }
        
        return modifiers;
    }
}

class MightyBlowTalent : Talent
{
    public MightyBlowTalent()
        : base(TalentId.MightyBlow, 1)
    {
        // empty;
    }

    public override List<ActionId> GetActions()
    {
        List<ActionId> actions = base.GetActions();

        if (PointsAssigned == MaxPoints)
        {
            actions.Add(ActionId.MightyBlow);
        }

        return actions;
    }
}

