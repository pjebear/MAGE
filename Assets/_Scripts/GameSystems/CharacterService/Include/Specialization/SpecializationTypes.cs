using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Character
{
    static class SpecializationConstants
    {
        public static readonly int LEVEL_UP_THRESHOLD = 100;
    }

    enum SpecializationType
    {
        NONE = -2,
        Base = -1,

        Archer,
        Footman,
        Monk,
        Paladin,
        //Adept,
        ////Footman Tree
        //ShieldWall,
        //Duelist,
        //Berserker,
        //// Archer Tree
        //LongBowman,
        //Assassin,
        //Ranger,
        //// Monk Tree
        //Priest,
        //Sage = Priest,
        //Shamman,
        //Druid,
        //// Adept tree
        //Elementalist,
        //Warlock,
        //SpellSword,

        ////Custom
        //Chaplain,

        NUM
    }
}
