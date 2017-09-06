using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EncounterSystem
{
    using Character;
    using Common.EncounterTypes;

    namespace EncounterFlow
    {
        namespace Queue
        {
            class TurnQueue
            {
                private EncounterState rEncounterState;
                private Queue<CharacterManager> mReadyCharacters;

                public TurnQueue()
                {
                    mReadyCharacters = new Queue<CharacterManager>();
                }

                public void Initialize(EncounterState state)
                {
                    rEncounterState = state;
                }

                public void IncrementClockCounts()
                {
                    foreach (var character in rEncounterState.GetAllEncounterUnits())
                    {
                        character.ProgressCharacterClock();
                        if (character.ClockGuageFull)
                        {
                            mReadyCharacters.Enqueue(character);
                        }
                    }
                }

                public bool HasReadyUnits()
                {
                    return mReadyCharacters.Count > 0;
                }

                public CharacterManager NextReadyUnit()
                {
                    return mReadyCharacters.Dequeue();
                }

                public CharacterManager CurrentUnit { get { if (mReadyCharacters.Count > 0) { return mReadyCharacters.Peek(); } return null; } }
            }
        }
    }
   
}


