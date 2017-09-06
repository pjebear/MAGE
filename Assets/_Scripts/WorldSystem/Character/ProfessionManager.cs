using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.ProfessionEnums;
using Common.AttributeTypes;
using Common.ActionTypes;
using Common.ActionEnums;
using Common.StatusEnums;
using Common.EquipmentTypes;
using Common.CharacterEnums;

namespace Common.ProfessionEnums
{
    //------------------------------------------------------------
    public enum ProfessionType
    {
        NONE = -1,
        Footman,
        Archer,
        Monk,
        Adept,
        //Footman Tree
        ShieldWall,
        Duelist,
        Berserker,
        // Archer Tree
        LongBowman,
        Assassin,
        Ranger,
        // Monk Tree
        Priest,
        Sage = Priest,
        Shamman,
        Druid,
        // Adept tree
        Elementalist,
        Warlock,
        SpellSword,
    }
}



namespace WorldSystem.Character
{

    using EncounterSystem.EventSystem;
    using Profession;
    using Talents;

    public class ProfessionManager
    {
        public EquipmentProficienciesContainer Proficiencies { get { return mCurrentProfession.EquipmentProficiencies; } } 
        public string ProfessionName { get { return mCurrentProfession.Type.ToString(); } }
        public int ProfessionLevel { get { return mCurrentProfession.CurrentLevel; } }
        private Dictionary<ProfessionType, ProfessionBase> mProfessions;

        private ProfessionBase mCurrentProfession;
        public ProfessionType CurrentProfessionType { get { return mCurrentProfession.Type; } }

        public ProfessionManager()
        {
            mProfessions = new Dictionary<ProfessionType, ProfessionBase>();
            // Ouch ...
            mProfessions.Add(ProfessionType.Footman, new Footman());
            mProfessions.Add(ProfessionType.Archer, new Archer());
            mProfessions.Add(ProfessionType.Adept, new Adept());
            mProfessions.Add(ProfessionType.Monk, new Monk());
            mProfessions.Add(ProfessionType.ShieldWall, new ShieldWall());
            mProfessions.Add(ProfessionType.Duelist, new Duelist());
            mProfessions.Add(ProfessionType.Berserker, new Berserker());
            //mProfessions.Add(ProfessionType.LongBowman, new ProfessionBase(ProfessionType.LongBowman));
            //mProfessions.Add(ProfessionType.Assassin, new ProfessionBase(ProfessionType.Assassin));
            //mProfessions.Add(ProfessionType.Ranger, new ProfessionBase(ProfessionType.Ranger));
            //mProfessions.Add(ProfessionType.Elementalist, new ProfessionBase(ProfessionType.Elementalist));
            //mProfessions.Add(ProfessionType.Warlock, new ProfessionBase(ProfessionType.Warlock));
            //mProfessions.Add(ProfessionType.SpellSword, new ProfessionBase(ProfessionType.SpellSword));
            //mProfessions.Add(ProfessionType.Priest, new ProfessionBase(ProfessionType.Priest)); //TODO: Make Sage if certain allignment
            //mProfessions.Add(ProfessionType.Shamman, new ProfessionBase(ProfessionType.Shamman));
            //mProfessions.Add(ProfessionType.Druid, new ProfessionBase(ProfessionType.Druid));
        }

        // Assumes IsUnlocked has been verified
        public void SetCurrentProfession(ProfessionType type)
        {
            mCurrentProfession = mProfessions[type];
        }

        public bool IsProfessionUnlocked(ProfessionType type)
        {
            ProfessionBase toUnlock = mProfessions[type];
            if (toUnlock.Base == ProfessionType.NONE)
            {
                return true;
            }
            else
            {
                ProfessionBase baseProfession = mProfessions[toUnlock.Base];
                if (baseProfession.CurrentLevel >= 4)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool AddExperience(float amount)
        {
            return mCurrentProfession.AddExperience(amount);
        }

        public void ApplyProfessionLevelUpAttributes(AttributeContainer baseAttribtues)
        {
            mCurrentProfession.ApplyProfessionLevelUpAttributes(baseAttribtues);
        }

        public List<AttributeModifier> GetPassives()
        {
            List<AttributeModifier> passives = new List<AttributeModifier>();
            passives.AddRange(mCurrentProfession.Passives);
            if (mCurrentProfession.Base != ProfessionType.NONE)
            {
                passives.AddRange(mProfessions[mCurrentProfession.Base].Passives);
            }
            return passives;
        }

        public ActionContainer GetActionContainer()
        {
            ActionContainer container = new ActionContainer();

            // Primary Actions
            container.ActionMap[ActionContainerCategory.Primary].AddRange(mCurrentProfession.Actions);
            foreach (var modifier in mCurrentProfession.ActionModifiers)
            {
                container.ActionModifierMap.Add(modifier.Key, new List<ActionModifier>());
                container.ActionModifierMap[modifier.Key].AddRange(modifier.Value);
            }
            foreach (var status in mCurrentProfession.ActionStatuses)
            {
                container.ActionStatusMap.Add(status.Key, new List<StatusEffectIndex>());
                container.ActionStatusMap[status.Key].AddRange(status.Value);
            }

            // Secondary Actions
            if (mCurrentProfession.Base != ProfessionType.NONE)
            {
                ProfessionBase baseProfession = mProfessions[mCurrentProfession.Base];
                container.ActionMap[ActionContainerCategory.Secondary].AddRange(baseProfession.Actions);

                foreach (var modifier in baseProfession.ActionModifiers)
                {
                    if (!container.ActionModifierMap.ContainsKey(modifier.Key))
                    {
                        container.ActionModifierMap.Add(modifier.Key, new List<ActionModifier>());
                    }

                    container.ActionModifierMap[modifier.Key].AddRange(modifier.Value);
                }
                foreach (var status in mCurrentProfession.ActionStatuses)
                {
                    if(!container.ActionStatusMap.ContainsKey(status.Key))
                    {
                        container.ActionStatusMap.Add(status.Key, new List<StatusEffectIndex>());
                    }
                    container.ActionStatusMap[status.Key].AddRange(status.Value);
                }
            }
            return container;
        }

        public List<EventListenerIndex> GetListeners()
        {
            return mCurrentProfession.Listeners;
        }

        public List<AuraIndex> GetAuras()
        {
            return mCurrentProfession.Auras;
        }

        public bool IsCurrentProfessionSpecialized()
        {
            return mCurrentProfession.Base != ProfessionType.NONE;
        }

        public string GetProfessionImageAsset(CharacterGender gender)
        {
            return mCurrentProfession.GetProfessionImageAsset(gender);
        }

        public Dictionary<ProfessionType, Dictionary<TalentIndex, List<TalentIndex>>> GetSkeletonTalentTrees()
        {
            Dictionary<ProfessionType, Dictionary<TalentIndex, List<TalentIndex>>> skeletonTrees = new Dictionary<ProfessionType, Dictionary<TalentIndex, List<TalentIndex>>>();
            foreach (var professionPair in mProfessions)
            {
                skeletonTrees.Add(professionPair.Key, professionPair.Value.TalentTree);
            }
            return skeletonTrees;
        }
    }
}
