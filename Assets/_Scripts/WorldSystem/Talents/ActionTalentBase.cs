using Common.ActionEnums;
using System.Collections.Generic;
using WorldSystem.Character;

namespace WorldSystem.Talents
{
    class ActionTalentBase : TalentBase
    {
        private ActionIndex mActionIndex;

        public ActionTalentBase(ActionIndex actionIndex, CharacterBase character, TalentIndex talentIndex)
            : base(talentIndex, TalentUnlockType.Action, 1, character)
        {
            mActionIndex = actionIndex;
        }

        protected override void _ApplyTalentPoints(int numToApply)
        {
            ActionContainerCategory category = rCharacterBase.IsCurrentProfessionSpecialized() ? ActionContainerCategory.Primary : ActionContainerCategory.Secondary;
            rCharacterBase.GetActionContainer().ActionMap[category].Add(mActionIndex);
        }

        protected override void _RemoveTalentPoints(int numToRemove)
        {
            ActionContainerCategory category = rCharacterBase.IsCurrentProfessionSpecialized() ? ActionContainerCategory.Primary : ActionContainerCategory.Secondary;
            rCharacterBase.GetActionContainer().ActionMap[category].Remove(mActionIndex);
        }
    }
}
