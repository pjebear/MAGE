using Common.StatusEnums;
using Common.StatusTypes;
using EncounterSystem.StatusEffects;
using System;
using WorldSystem.Character;

namespace WorldSystem.Talents
{
    class StatusTalentBase : TalentBase
    {
        private StatusEffect mStatusEffect;

        public StatusTalentBase(StatusEffectIndex statusIndex, CharacterBase characterBase, TalentIndex talentIndex, int maxPoints)
            : base(talentIndex, TalentUnlockType.Status, maxPoints, characterBase)
        {
            mStatusEffect = StatusEffectFactory.CheckoutStatusEffect(statusIndex);
            rCharacterBase = characterBase;
            UnityEngine.Debug.Assert(maxPoints == 1 || mStatusEffect.CanStack, String.Format("Status talent created with {0} max points initialized with non stacking talent {1}", maxPoints, statusIndex.ToString()));
        }

        protected override void _ApplyTalentPoints(int numPointsToApply)
        {
            rCharacterBase.ApplyStatusEffect(mStatusEffect, numPointsToApply);
        }

        protected override void _RemoveTalentPoints(int numPointsToRemove)
        {
            rCharacterBase.RemoveStatusEffect(mStatusEffect.Index, numPointsToRemove);
        }
    }
}
