using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldSystem
{
    using Character;
    using global::Common.UnitTypes;
    using Screens.Payloads;

    class RosterManager
    {
        private readonly string FILE_NAME = "PartyRoster.XML";
        private int mRosterCharacterIndexCounter;

        public UnitRoster CharacterRoster { get; private set; }

        public RosterManager()
        {
            CharacterRoster = new UnitRoster();
        }

        public void ResetPostEncounter(List<CharacterBase> fromBattle)
        {
            foreach (CharacterBase character in fromBattle)
            {
                character.PostEncounterReset();
            }
        }

        public void AddCharacter(CharacterBase toAdd)
        {
            CharacterRoster.AddCharacter( toAdd);
        }

        public void RemoveCharacter(int index)
        {
            CharacterRoster.RemoveCharacter(index);
        }

        public bool LoadFromFile(string rootPath)
        {
            return false;
        }

        public bool SaveToFile(string rootPath)
        {
            return false;
        }
    }
}


