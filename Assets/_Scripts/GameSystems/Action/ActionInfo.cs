using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    abstract class ActionInfoBase
    {
        public float Effectiveness = 1f;
        public ActionId ActionId = ActionId.INVALID;
        public StateChange ActionCost = StateChange.Empty;
        public ActionRange ActionRange = ActionRange.NUM;
        public ActionSource ActionSource = ActionSource.NUM;
        public CastSpeed CastSpeed = CastSpeed.INVALID;
        public RangeInfo CastRange = RangeInfo.Unit;
        public RangeInfo EffectRange = RangeInfo.Unit;
        public ActionAnimationInfo AnimationInfo = new ActionAnimationInfo();
        public ActionProjectileInfo ProjectileInfo = new ActionProjectileInfo();
        public ActionEffectInfo EffectInfo = new ActionEffectInfo();
        public ActionChainInfo ChainInfo = new ActionChainInfo();
        public ActionSummonInfo SummonInfo = new ActionSummonInfo();
        public bool IsSelfCast = false;
        public bool CanGroundTarget = false;

        public abstract StateChange GetTargetStateChange(Character caster, Character target);

        public bool CanCast(Character caster)
        {
            bool canCast = caster.HasResourcesForAction(ActionCost);

            if (ActionSource == ActionSource.Weapon)
            {
                canCast &= caster.CurrentAttributes[StatusType.Disarmed] == 0;
            }
            else
            {
                canCast &= caster.CurrentAttributes[StatusType.Silenced] == 0;
            }

            if (SummonInfo.SummonType != SpecializationType.INVALID)
            {
                int summonCount = caster.Children.FindAll(x => x.CurrentSpecializationType == SummonInfo.SummonType).Count;
                canCast &= summonCount <= SummonInfo.MaxSummonCount;
            }

            return canCast;
        }
    }

    [System.Serializable]
    class ActionInfo : ActionInfoBase
    {
        public override StateChange GetTargetStateChange(Character caster, Character target)
        {
            throw new NotImplementedException();
        }
    }
}