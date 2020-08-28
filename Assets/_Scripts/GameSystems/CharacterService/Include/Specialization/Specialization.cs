using MAGE.GameSystems.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Characters
{
    class Specialization
    {
        public SpecializationType SpecializationType;
        public int Level;
        public int Experience;
        public List<ActionId> mBaseActions = new List<ActionId>();
        public List<AuraType> mBaseAuras = new List<AuraType>();
        public List<ActionResponseId> mBaseResponses = new List<ActionResponseId>();
        public List<ProficiencyType> mBaseProficiencies = new List<ProficiencyType>();

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

        public List<ProficiencyType> GetProficiencies()
        {
            List<ProficiencyType> proficiencies = new List<ProficiencyType>(mBaseProficiencies);
            foreach (Talent talent in Talents.Values)
            {
                proficiencies.AddRange(talent.GetProficiencies());
            }
            return proficiencies;
        }

        public List<EquippableModifier> GetEquippableModifiers()
        {
            List<EquippableModifier> equippableModifiers = new List<EquippableModifier>();
            foreach (Talent talent in Talents.Values)
            {
                equippableModifiers.AddRange(talent.GetEquippableModifiers());
            }
            return equippableModifiers;
        }

        public List<AttributeModifier> GetAttributeModifiers()
        {
            List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();
            foreach (Talent talent in Talents.Values)
            {
                attributeModifiers.AddRange(talent.GetAttributeModifiers());
            }
            return attributeModifiers;
        }

        public List<ActionResponseId> GetActionResponses()
        {
            List<ActionResponseId> actionResponseIds = new List<ActionResponseId>(mBaseResponses);
            foreach (Talent talent in Talents.Values)
            {
                actionResponseIds.AddRange(talent.GetActionResponses());
            }
            return actionResponseIds;
        }

        public List<AuraType> GetAuras()
        {
            List<AuraType> auras = new List<AuraType>(mBaseAuras);
            foreach (Talent talent in Talents.Values)
            {
                auras.AddRange(talent.GetAuras());
            }
            return auras;
        }

        public List<ActionId> GetActions()
        {
            List<ActionId> actions = new List<ActionId>(mBaseActions);
            foreach (Talent talent in Talents.Values)
            {
                actions.AddRange(talent.GetActions());
            }
            return actions;
        }

        public List<IActionModifier> GetActionModifiers()
        {
            List<IActionModifier> actionModifiers = new List<IActionModifier>();
            foreach (Talent talent in Talents.Values)
            {
                actionModifiers.AddRange(talent.GetActionModifiers());
            }
            return actionModifiers;
        }

        public SpecializationProgress GetProgress()
        {
            SpecializationProgress specializationProgress = new SpecializationProgress();
            specializationProgress.SpecializationType = SpecializationType;
            specializationProgress.Level = Level;
            specializationProgress.Experience = Experience;

            foreach (var pair in Talents)
            {
                specializationProgress.TalentProgress.Add(pair.Key,
                    new TalentProgress() { TalentId = pair.Key, CurrentPoints = pair.Value.PointsAssigned, MaxPoints = pair.Value.MaxPoints });
            }

            return specializationProgress;
        }
    }
}



