using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class SpecializationFactory
{
    public static Specialization CheckoutSpecialization(SpecializationType type, DB.Character.DBSpecializationInfo specializationInfo = null)
    {
        DB.DBSpecialization dbSpecialization = DB.DBHelper.LoadSpecialization(type);

        Specialization specialization = new Specialization();
        specialization.SpecializationType = type;

        if (specializationInfo != null)
        {
            specialization.Level = specializationInfo.Level;
            specialization.Experience = specializationInfo.Experience;
        }

        foreach (int talentId in dbSpecialization.TalentIds)
        {
            int assignedPoints = 0;

            if (specializationInfo != null)
            {
                DB.Character.Talent talent = specializationInfo.Talents.Find(x => x.TalentId == talentId);
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

        foreach (DB.DBAttribute levelupModifier in dbSpecialization.LevelUpModifiers)
        {
            AttributeIndex attributeIndex = new AttributeIndex((AttributeCategory)levelupModifier.AttributeCategory, levelupModifier.AttributeId);
            specialization.LevelUpModifiers.Add(new AttributeModifier(attributeIndex, ModifierType.Increment, levelupModifier.Value));
        }

        return specialization;
    }
}

