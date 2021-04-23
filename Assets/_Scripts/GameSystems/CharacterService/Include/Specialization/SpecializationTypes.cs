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

        public static readonly List<SpecializationType> MULTI_SPECIALIZATIONS = new List<SpecializationType>()
        {
            SpecializationType.Archer
            ,SpecializationType.Footman
            ,SpecializationType.Monk
        };
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

        NUM
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
