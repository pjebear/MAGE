using Common.ActionEnums;
using Common.StatusEnums;
using System.Collections.Generic;

using WorldSystem.Character;

namespace WorldSystem.Talents
{
    class AuraTalentBase : TalentBase
    {
        private AuraIndex mAuraIndex;


        public AuraTalentBase(AuraIndex auraIndex, CharacterBase character, TalentIndex talentIndex)
            : base(talentIndex, TalentUnlockType.Action, 1, character)
        {
            mAuraIndex = auraIndex;
        }

        protected override void _ApplyTalentPoints(int numToApply)
        {
            rCharacterBase.GetAuras().Add(mAuraIndex);
        }

        protected override void _RemoveTalentPoints(int numToRemove)
        {
            rCharacterBase.GetAuras().Remove(mAuraIndex);
        }
    }
}
