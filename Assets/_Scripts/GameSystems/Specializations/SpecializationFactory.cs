using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class SpecializationFactory
{
    public static SpecializationInfo CheckoutSpecializationInfo(SpecializationType type)
    {
        SpecializationInfo info = new SpecializationInfo();
        info.SpecializationType = type;

        switch (type)
        {
            case (SpecializationType.Footman):
                {
                    // Proficiencies
                    info.BaseProficiencyModifiers.Add(new AttributeModifier(ProficiencyType.OneHands));
                    info.BaseProficiencyModifiers.Add(new AttributeModifier(ProficiencyType.Sheild));
                    info.BaseProficiencyModifiers.Add(new AttributeModifier(ProficiencyType.Chain));
                    info.BaseProficiencyModifiers.Add(new AttributeModifier(ProficiencyType.Leather));

                    // LevelupModifiers
                    info.LevelUpModifiers.Add(new AttributeModifier(PrimaryStat.Might, ModifierType.Increment, 4));
                    info.LevelUpModifiers.Add(new AttributeModifier(PrimaryStat.Finese, ModifierType.Increment, 3));
                    info.LevelUpModifiers.Add(new AttributeModifier(PrimaryStat.Magic, ModifierType.Increment, 2));

                    info.LevelUpModifiers.Add(new AttributeModifier(SecondaryStat.Fortitude, ModifierType.Increment, 5));
                    info.LevelUpModifiers.Add(new AttributeModifier(SecondaryStat.Attunement, ModifierType.Increment, 1));

                    // Action modifiers
                    // empty

                    // Actions
                    // empty

                    // Auras
                    // empty

                    // Listeners
                    // empty

                    // Talents
                    info.Talents.AddRange(new List<TalentId>() { TalentId.BlockIncrease, TalentId.MightyBlow });
                }
                break;

            case (SpecializationType.Monk):
                {
                    // Proficiencies
                    info.BaseProficiencyModifiers.Add(new AttributeModifier(ProficiencyType.Staff));
                    info.BaseProficiencyModifiers.Add(new AttributeModifier(ProficiencyType.Cloth));

                    // LevelupModifiers
                    info.LevelUpModifiers.Add(new AttributeModifier(PrimaryStat.Might, ModifierType.Increment, 2));
                    info.LevelUpModifiers.Add(new AttributeModifier(PrimaryStat.Finese, ModifierType.Increment, 1));
                    info.LevelUpModifiers.Add(new AttributeModifier(PrimaryStat.Magic, ModifierType.Increment, 4));

                    info.LevelUpModifiers.Add(new AttributeModifier(SecondaryStat.Fortitude, ModifierType.Increment, 1));
                    info.LevelUpModifiers.Add(new AttributeModifier(SecondaryStat.Attunement, ModifierType.Increment, 5));

                    // Action modifiers
                    // empty

                    // Actions
                    info.Actions.Add(ActionId.Heal);
                    info.Actions.Add(ActionId.Protection);


                    // Auras
                    // empty

                    // Listeners
                    // empty

                    // Talents
                    info.Talents.AddRange(new List<TalentId>() { TalentId.HealIncrease, TalentId.HealOnHurt});
                }
                break;
        }

        return info;
    }
}

