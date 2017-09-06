using Common.UnitTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Character;
using WorldSystem.Items;

namespace WorldSystem.Managers
{
    class PartyManager
    {
        private UnitRoster mUnitRoster;
        private Inventory mInventory;
        private int mWealth;

        public PartyManager()
        {
            mUnitRoster = new UnitRoster();
            mInventory = new Inventory();
            mWealth = 0;
        }

        #region _Roster_
        public UnitRoster GetRoster()
        {
            return mUnitRoster;
        }

        public void ResetPostEncounter()
        {
            foreach (CharacterBase character in mUnitRoster.Roster.Values)
            {
                character.PostEncounterReset();
            }
        }

        public void AddCharacter(CharacterBase toAdd)
        {
            mUnitRoster.AddCharacter(toAdd);
        }

        public void RemoveCharacter(CharacterBase toRemove)
        {
            mUnitRoster.RemoveCharacter(toRemove.CharacterID);
        }
        #endregion
    }
}
