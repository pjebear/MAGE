using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.ProfessionEnums;
using Common.AttributeEnums;
using Common.AttributeTypes;
using Common.ActionEnums;
using Common.ActionTypes;
using Common.StatusEnums;
using Common.EquipmentTypes;
using Common.EquipmentEnums;
using EncounterSystem.EventSystem;
using WorldSystem.Talents;
using Common.CharacterEnums;

namespace WorldSystem
{
    namespace Profession
    {
        class ProfessionBase
        {

            protected const int MAX_PROFESSION_LEVEL = 8;
            protected static readonly float[] PROFESSION_LEVELUP_EXPERIENCE_REQUIREMENTS = { 200f, 300f, 500f, 800f, 1200f, 1700f, 2300f };
            public ProfessionType Type { get; protected set; }
            public ProfessionType Base { get; protected set; }
            public int CurrentLevel { get; private set; }
            public float CurrentExperience { get; private set; }

            public List<AttributeModifier> Passives { get; private set; }
            public List<ActionIndex> Actions { get; private set; }
            public Dictionary<ActionIndex, List<ActionModifier>> ActionModifiers { get; private set; }
            public Dictionary<ActionIndex, List<StatusEffectIndex>> ActionStatuses { get; private set; }
            public List<AuraIndex> Auras { get; private set; }
            public List<EventListenerIndex> Listeners { get; private set; }
            public EquipmentProficienciesContainer EquipmentProficiencies { get; protected set; }
            public Dictionary<TalentIndex, List<TalentIndex>> TalentTree { get; protected set; }

            protected ProfessionBase(ProfessionType type)
            {
                Type = type;
                Base = GetBaseProfession(type);
                CurrentLevel = 1;
                CurrentExperience = 0f;
                Passives = new List<AttributeModifier>();
                Actions = new List<ActionIndex>();
                ActionModifiers = new Dictionary<ActionIndex, List<ActionModifier>>();
                ActionStatuses = new Dictionary<ActionIndex, List<StatusEffectIndex>>();
                Auras = new List<AuraIndex>();
                Listeners = new List<EventListenerIndex>();
                EquipmentProficiencies = new EquipmentProficienciesContainer();
                TalentTree = new Dictionary<TalentIndex, List<TalentIndex>>();
            }

            public bool AddExperience(float amount)
            {
                CurrentExperience += amount;
                if (CurrentLevel < MAX_PROFESSION_LEVEL)
                {
                    if (CurrentExperience >= PROFESSION_LEVELUP_EXPERIENCE_REQUIREMENTS[CurrentLevel])
                    {
                        CurrentLevel++;
                        CurrentExperience -= PROFESSION_LEVELUP_EXPERIENCE_REQUIREMENTS[CurrentLevel];
                        return true;
                    }
                }
                return false;
            }
            public void ApplyProfessionLevelUpAttributes(AttributeContainer baseCharacterAttributes)
            {
                ApplyProfessionLevelupAttributes(Type, baseCharacterAttributes);
            }

            public string GetProfessionImageAsset(CharacterGender gender)
            {
                return GetProfessionImageAsset(Type, gender);
            }

            // ------------------------ STATIC HELPERS---------------------------------------------------
            private static ProfessionType GetBaseProfession(ProfessionType profession)
            {
                if (profession == ProfessionType.Adept
                    || profession == ProfessionType.Footman
                    || profession == ProfessionType.Monk
                    || profession == ProfessionType.Archer)
                {
                    return ProfessionType.NONE;
                }
                else if (profession == ProfessionType.Elementalist
                    || profession == ProfessionType.Warlock
                    || profession == ProfessionType.SpellSword)
                {
                    return ProfessionType.Adept;
                }
                else if (profession == ProfessionType.ShieldWall
                    || profession == ProfessionType.Duelist
                    || profession == ProfessionType.Berserker)
                {
                    return ProfessionType.Footman;
                }
                else if (profession == ProfessionType.LongBowman
                    || profession == ProfessionType.Assassin
                    || profession == ProfessionType.Ranger)
                {
                    return ProfessionType.Archer;
                }
                else if (profession == ProfessionType.Priest
                    || profession == ProfessionType.Sage
                    || profession == ProfessionType.Druid
                    || profession == ProfessionType.Shamman)
                {
                    return ProfessionType.Monk;
                }
                else
                {
                    Debug.LogErrorFormat("Error retrieving Base Profession for {0}", profession.ToString());
                    return ProfessionType.NONE;
                }
            }
            private static List<ProfessionType> GetSpecializations(ProfessionType profession)
            {
                switch (profession)
                {
                    case (ProfessionType.Footman):
                        return new List<ProfessionType>()
                        {
                            ProfessionType.ShieldWall,
                            ProfessionType.Duelist,
                            ProfessionType.Berserker
                        };
                    case (ProfessionType.Archer):
                        return new List<ProfessionType>()
                        {
                            ProfessionType.LongBowman,
                            ProfessionType.Assassin,
                            ProfessionType.Ranger
                        };
                    case (ProfessionType.Adept):
                        return new List<ProfessionType>()
                        {
                            ProfessionType.Elementalist,
                            ProfessionType.Warlock,
                            ProfessionType.SpellSword
                        };
                    case (ProfessionType.Monk):
                        return new List<ProfessionType>()
                        {
                            ProfessionType.Sage,
                            ProfessionType.Priest,
                            ProfessionType.Druid,
                            ProfessionType.Shamman
                        };
                    default:
                        Debug.LogErrorFormat("Error Retrieving Specializations for {0}", profession.ToString());
                        return new List<ProfessionType>();
                }
            }
            private static void ApplyProfessionLevelupAttributes(ProfessionType type, AttributeContainer baseAttributes)
            {
                float mightGrowth = 0f, finessGrowth = 0f, magicGrowth = 0f, attunementGrowth = 0f, fortitudeGrowth = 0f;
                switch (type)
                {
                    case (ProfessionType.Footman):
                        mightGrowth = 6f; finessGrowth = 3f; magicGrowth = 3f; attunementGrowth = 0f; fortitudeGrowth = 1f;
                        break;
                    case (ProfessionType.ShieldWall):
                        mightGrowth = 5.5f; finessGrowth = 2.25f; magicGrowth = 2.25f; attunementGrowth = 0f; fortitudeGrowth = 1f;
                        break;
                    case (ProfessionType.Duelist):
                        mightGrowth = 5f; finessGrowth = 4f; magicGrowth = 1f; attunementGrowth = 0f; fortitudeGrowth = 1f;
                        break;
                    case (ProfessionType.Berserker):
                        mightGrowth = 5f; finessGrowth = 1.5f; magicGrowth = 3.5f; attunementGrowth = 1f; fortitudeGrowth = 1f;
                        break;
                    case (ProfessionType.Archer):
                        mightGrowth = 4f; finessGrowth = 6f; magicGrowth = 2f; attunementGrowth = 0f; fortitudeGrowth = 1f;
                        break;
                    case (ProfessionType.Adept):
                        mightGrowth = 1.75f; finessGrowth = 1.75f; magicGrowth = 8.5f; attunementGrowth = 1f; fortitudeGrowth = 0f;
                        break;
                    case (ProfessionType.Monk):
                        mightGrowth = 2.5f; finessGrowth = 2.5f; magicGrowth = 7f; attunementGrowth = 1f; fortitudeGrowth = 0f;
                        break;
                    default:
                        Debug.LogError("No StatGrowth defined for " + type.ToString());
                        break;
                }
                baseAttributes[AttributeType.Stat][(int)PrimaryStat.Might] += mightGrowth;
                baseAttributes[AttributeType.Stat][(int)PrimaryStat.Finese] += finessGrowth;
                baseAttributes[AttributeType.Stat][(int)PrimaryStat.Magic] += magicGrowth;
                baseAttributes[AttributeType.Stat][(int)SecondaryStat.Attunement] += attunementGrowth;
                baseAttributes[AttributeType.Stat][(int)SecondaryStat.Fortitude] += fortitudeGrowth;
            }
            private static string GetProfessionImageAsset(ProfessionType profession, CharacterGender gender)
            {
                string toReturn = "CharacterImages/" + profession.ToString() + "_{0}_Full"; 
                //switch (profession)
                //{
                //    case (ProfessionType.Footman):
                //        toReturn = "CharacterImages/Footman_{0}_Full";
                //        break;
                //    case (ProfessionType.ShieldWall):
                //        toReturn = "CharacterImages/Shieldwall_{0}_Full";
                //        break;
                //    case (ProfessionType.Duelist):
                //        toReturn = "CharacterImages/Duelist_{0}_Full";
                //        break;
                //    case (ProfessionType.Berserker):
                //        toReturn = "CharacterImages/Berserker_{0}_Full";
                //        break;
                //    default:
                //        Debug.LogError("No ImageAsset defined for " + profession.ToString());
                //        return "";
                //}
                return string.Format(toReturn, gender.ToString());
            }
        }

    }

}

