using MAGE.GameSystems.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Characters.Internal
{
    static class SpecializationFactory
    {
        public static Specialization CheckoutSpecialization(SpecializationType type, SpecializationProgress specializationProgress = null)
        {
            DB.DBSpecialization dbSpecialization = DBService.Get().LoadSpecialization(type);

            Specialization specialization = new Specialization();
            specialization.SpecializationType = type;

            if (specializationProgress != null)
            {
                specialization.Level = specializationProgress.Level;
                specialization.Experience = specializationProgress.Experience;
            }

            foreach (int talentId in dbSpecialization.TalentIds)
            {
                int assignedPoints = 0;

                if (specializationProgress != null)
                {
                    if (specializationProgress.TalentProgress.ContainsKey((TalentId)talentId))
                    {
                        assignedPoints = specializationProgress.TalentProgress[(TalentId)talentId].CurrentPoints;
                    }
                }

                specialization.Talents.Add((TalentId)talentId, TalentFactory.CheckoutTalent((TalentId)talentId, assignedPoints));
            }

            foreach (int actionId in dbSpecialization.ActionIds)
            {
                specialization.mBaseActions.Add((ActionId)actionId);
            }

            foreach (int auraId in dbSpecialization.AuraIds)
            {
                specialization.mBaseAuras.Add((AuraType)auraId);
            }

            foreach (int listenerId in dbSpecialization.ResponseListenerIds)
            {
                specialization.mBaseResponses.Add((ActionResponseId)listenerId);
            }

            foreach (int proficiency in dbSpecialization.Proficiencies)
            {
                specialization.mBaseProficiencies.Add((ProficiencyType)proficiency);
            }

            foreach (DB.DBAttribute levelupModifier in dbSpecialization.LevelUpModifiers)
            {
                AttributeIndex attributeIndex = new AttributeIndex((AttributeCategory)levelupModifier.AttributeCategory, levelupModifier.AttributeId);
                specialization.LevelUpModifiers.Add(new AttributeModifier(attributeIndex, ModifierType.Increment, levelupModifier.Value));
            }

            return specialization;
        }
    }
}



