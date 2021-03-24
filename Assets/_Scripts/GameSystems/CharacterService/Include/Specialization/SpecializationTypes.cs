using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Characters
{
    static class SpecializationConstants
    {
        public static readonly int LEVEL_UP_THRESHOLD = 100;
    }

    class SpecializationProgress
    {
        public SpecializationType SpecializationType;
        public int Level = 1;
        public int Experience = 0;
        public Dictionary<TalentId, TalentProgress> TalentProgress = new Dictionary<TalentId, TalentProgress>();
    }


    enum SpecializationType
    {
        INVALID = -1, 

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

        // Summons
        Bear,

        NUM,

        MULTI_SPECIALIZATION_FIRST = Archer,
        MULTI_SPECIALIZATION_LAST = Monk,
        MULTI_SEPCIALIZATION_NUM = MULTI_SPECIALIZATION_LAST - MULTI_SPECIALIZATION_FIRST + 1,
    }

    enum SpecializationRole
    {
        Tank,
        Support,
        Range,
        Specialist,

        NUM
    }
}
