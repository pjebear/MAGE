using Common.ActionEnums;
using Common.ActionTypes;
using System.Collections.Generic;
using WorldSystem.Character;

namespace WorldSystem.Talents
{
    class ActionModifierTalentBase : TalentBase
    {
        private ActionIndex mActionIndex;
        private ActionModifier mActionModifier;

        public ActionModifierTalentBase(ActionIndex actionIndex, ActionModifier actionModifier, CharacterBase characterBase, TalentIndex talentIndex)
            : base(talentIndex, TalentUnlockType.Action, 1, characterBase)
        {
            mActionIndex = actionIndex;
        }

        protected override void _ApplyTalentPoints(int numToApply)
        {
            for (int i = 0; i < numToApply; i++)
            {
                rCharacterBase.GetActionContainer().ActionModifierMap[mActionIndex].Add(mActionModifier);
            }
        }

        protected override void _RemoveTalentPoints(int numToRemove)
        {
            for (int i = 0; i < numToRemove; i++)
            {
                rCharacterBase.GetActionContainer().ActionModifierMap[mActionIndex].Remove(mActionModifier);
            }
        }
    }
}
