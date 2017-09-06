using Screens.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Character;

namespace WorldSystem
{
    namespace Talents
    {
        public enum TalentUnlockType
        {
            Status,
            Action,
            ActionModifier,
            EquipmentProficiency,
            Aura,
            EventListener,
        }

        abstract class TalentBase
        {
            protected CharacterBase rCharacterBase;
            public TalentUnlockType UnlockType { get; private set; }
            public TalentIndex TalentIndex { get; private set; }
            public string TalentName { get; private set; }
            public int CurrentPoints;
            public int MaxPoints;

            public List<TalentBase> PrerequisiteTalents;

            public TalentBase(TalentIndex talentIndex, TalentUnlockType unlockType, int maxPoints, CharacterBase characterBase)
            {
                TalentIndex = talentIndex;
                UnlockType = unlockType;
                CurrentPoints = 0;
                MaxPoints = maxPoints;
                rCharacterBase = characterBase;
                PrerequisiteTalents = new List<TalentBase>();
            }

            public bool IsUnlocked()
            {
                bool unlocked = true;
                foreach(TalentBase talent in PrerequisiteTalents)
                {
                    unlocked &= talent.CurrentPoints > 0; // TODO: Maybe prerequisite needs Current points == max points?
                }

                return unlocked;
            }

            public void ApplyTalentPoints(int numPoints)
            {
                UnityEngine.Debug.Assert(IsUnlocked(), "Attempting to assign points to a talent not unlocked");
                UnityEngine.Debug.AssertFormat(CurrentPoints + numPoints <= MaxPoints, "Attempting to assign {0} points to a talent with room for {1} more points", numPoints, MaxPoints - CurrentPoints);
                CurrentPoints++;
                _ApplyTalentPoints(numPoints);
                
            }

            public void RemoveTalentPoints(int numPoints)
            {
                UnityEngine.Debug.Assert(IsUnlocked(), "Attempting to remove points from a talent not unlocked");
                UnityEngine.Debug.AssertFormat(CurrentPoints >= numPoints, "Attempting to remove {0} points from a talent only {1} points available", numPoints, CurrentPoints);
                CurrentPoints--;
                _RemoveTalentPoints(numPoints);
            }

            public int ResetTalentPoints()
            {
                int numPointsToResest = CurrentPoints;
                CurrentPoints = 0;
                _RemoveTalentPoints(numPointsToResest);
                return numPointsToResest;
            }

            public TalentPayload GetPayload()
            {
                return new TalentPayload(TalentIndex, CurrentPoints, MaxPoints, IsUnlocked());
            }

            protected abstract void _ApplyTalentPoints(int numPointsToApply);
            protected abstract void _RemoveTalentPoints(int numPointsToRemove);
           
        }

    }
    
}
