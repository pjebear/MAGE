using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Characters.Internal
{
    class SpecializationInfo
    {
        public SpecializationType SpecializationType;
        public List<AttributeModifier> BaseProficiencyModifiers = new List<AttributeModifier>();
        public List<AttributeModifier> LevelUpModifiers = new List<AttributeModifier>();
        public List<IActionModifier> ActionModifiers = new List<IActionModifier>();
        public List<ActionId> Actions = new List<ActionId>();
        public List<AuraType> Auras = new List<AuraType>();
        public List<ActionResponseId> Listeners = new List<ActionResponseId>();
        public List<TalentId> Talents = new List<TalentId>();
    }
}


