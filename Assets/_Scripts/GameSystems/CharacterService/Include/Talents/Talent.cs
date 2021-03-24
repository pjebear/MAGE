using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MAGE.GameSystems.Characters
{
    class TalentProgress
    {
        public TalentId TalentId;
        public int CurrentPoints;
        public int MaxPoints;
    }

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
        public virtual List<EquippableModifier> GetEquippableModifiers() { return new List<EquippableModifier>(); }
        public virtual List<ProficiencyType> GetProficiencies() { return new List<ProficiencyType>(); }
        public virtual List<AttributeModifier> GetAttributeModifiers() { return new List<AttributeModifier>(); }
        public virtual List<ActionId> GetActions() { return new List<ActionId>(); }
        public virtual List<AuraType> GetAuras() { return new List<AuraType>(); }
        public virtual List<ActionResponseId> GetActionResponses() { return new List<ActionResponseId>(); }
    }
}