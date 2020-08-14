using MAGE.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Character.Internal
{
    static class SpecializationFactory
    {
        public static Specialization CheckoutSpecialization(SpecializationType type, DBSpecializationProgress specializationProgress = null)
        {
            DBSpecialization dbSpecialization = DBService.Get().LoadSpecialization(type);

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
                    DBTalentProgress talent = specializationProgress.Talents.Find(x => x.TalentId == talentId);
                    if (talent != null)
                    {
                        assignedPoints = talent.AssignedPoints;
                    }
                }

                specialization.Talents.Add((TalentId)talentId, TalentFactory.CheckoutTalent((TalentId)talentId, assignedPoints));
            }

            foreach (int actionId in dbSpecialization.ActionIds)
            {
                specialization.Actions.Add((ActionId)actionId);
            }

            foreach (int auraId in dbSpecialization.AuraIds)
            {
                specialization.Auras.Add((AuraType)auraId);
            }

            foreach (int listenerId in dbSpecialization.ResponseListenerIds)
            {
                specialization.ActionResponses.Add((ActionResponseId)listenerId);
            }

            foreach (int proficiency in dbSpecialization.Proficiencies)
            {
                specialization.Proficiencies.Add((ProficiencyType)proficiency);
            }

            foreach (DBAttribute levelupModifier in dbSpecialization.LevelUpModifiers)
            {
                AttributeIndex attributeIndex = new AttributeIndex((AttributeCategory)levelupModifier.AttributeCategory, levelupModifier.AttributeId);
                specialization.LevelUpModifiers.Add(new AttributeModifier(attributeIndex, ModifierType.Increment, levelupModifier.Value));
            }

            return specialization;
        }
    }
}



