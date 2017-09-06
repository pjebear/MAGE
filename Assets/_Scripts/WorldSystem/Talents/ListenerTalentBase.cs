using Common.ActionEnums;
using Common.StatusEnums;
using EncounterSystem.EventSystem;
using System.Collections.Generic;
using WorldSystem.Character;

namespace WorldSystem.Talents
{
    class ListenerTalentBase : TalentBase
    {
        private EventListenerIndex mListenerIndex;

        public ListenerTalentBase(EventListenerIndex listenerIndex, CharacterBase character, TalentIndex talentIndex)
            : base(talentIndex, TalentUnlockType.Action, 1, character)
        {
            mListenerIndex = listenerIndex;
        }

        protected override void _ApplyTalentPoints(int numToApply)
        {
            rCharacterBase.GetListeners().Add(mListenerIndex);
        }

        protected override void _RemoveTalentPoints(int numToRemove)
        {
            rCharacterBase.GetListeners().Remove(mListenerIndex);
        }
    }
}
