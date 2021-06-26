using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Characters.Internal
{
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

                case (TalentId.MagicIncrease):
                    talent = new AttributeModifierTalentBase(talentId, new AttributeModifier(PrimaryStat.Magic, ModifierType.Multiply, .1f), 3);
                    break;

                case (TalentId.MightyBlow):
                    talent = new MightyBlowTalent();
                    break;

                case (TalentId.MightIncrease):
                    talent = new AttributeModifierTalentBase(talentId, new AttributeModifier(PrimaryStat.Might, ModifierType.Multiply, .1f), 3);
                    break;

                case (TalentId.MoveIncrease):
                    talent = new AttributeModifierTalentBase(talentId, new AttributeModifier(TertiaryStat.Movement, ModifierType.Increment, 1), 2);
                    break;

                case (TalentId.SpeedIncrease):
                    talent = new AttributeModifierTalentBase(talentId, new AttributeModifier(TertiaryStat.Speed, ModifierType.Multiply, .1f), 3);
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
                        new AttributeIndex(AttributeCategory.TertiaryStat, (int)TertiaryStat.Block)
                        , ModifierType.Increment
                        , BlockIncreasePerPoint * PointsAssigned));
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

            public override List<IActionModifier> GetActionModifiers()
            {
                List<IActionModifier> modifiers = base.GetActionModifiers();

                if (PointsAssigned > 0)
                {
                    modifiers.Add(new HealModifier(1 + HealIncreasePerPoint * PointsAssigned));
                }

                return modifiers;
            }
        }

        class AttributeModifierTalentBase : Talent
        {
            private AttributeModifier ToModify;

            public AttributeModifierTalentBase(TalentId talentId, AttributeModifier toModify, int maxPoints)
                : base(talentId, maxPoints)
            {
                ToModify = toModify;
            }

            public override List<AttributeModifier> GetAttributeModifiers()
            {
                List<AttributeModifier> modifiers = base.GetAttributeModifiers();

                if (PointsAssigned > 0)
                {
                    AttributeModifier attributeModifier = ToModify.Copy();
                    attributeModifier.Delta *= PointsAssigned;
                    modifiers.Add(attributeModifier);
                }

                return modifiers;
            }
        }

        //class EquipmentModifierTalentBase : Talent
        //{
        //    private EquippableTag Tag;
        //    private AttributeScalar ToAdd;

        //    public EquipmentModifierTalentBase(EquippableTag tag, AttributeScalar toAdd, int blockIncrease, int parryIncrease, TalentId talentId, int maxPoints)
        //        : base(talentId, maxPoints)
        //    {
        //        Tag = tag;
        //        ToAdd = toAdd;
        //    }

        //    public override List<EquippableModifier> GetEquippableModifiers()
        //    {
        //        List<EquippableModifier> modifiers = base.GetEquippableModifiers();

        //        if (PointsAssigned > 0)
        //        {
        //            modifiers.Add(new EquippableModifier (ToModify, ModifierType, IncreasePerPoint * PointsAssigned));
        //        }

        //        return modifiers;
        //    }
        //}

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
    }
}


