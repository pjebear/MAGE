using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Screens.Payloads;
using Common.CharacterEnums;
using Common.AttributeEnums;
using Common.AttributeTypes;
using Common.StatusEnums;
using Common.StatusTypes;
using Common.ActionTypes;
using Common.CommonUtil.AttributeUtil;
using Common.ProfessionEnums;
using Common.EquipmentTypes;
using Common.EquipmentEnums;


namespace WorldSystem
{
    using EncounterSystem.EventSystem;
    using Talents;

    namespace Character
    {
        public class CharacterBase
        {
            private const int LEVELUP_EXPERIENCE_REQUIREMENT = 100;

            public int CharacterID { get; protected set; } //unique identifier assigned to each unit added to the party roster
            public string CharacterName { get; protected set; }
            public bool IsStoryCharacter { get; protected set; }
            public UnitGroup OwnedBy { get; protected set; }
            public CharacterGender CharacterGender { get; protected set; }
            private ProfessionManager mProfessionManager;
            private StatusManager mStatusManager;
            private EquipmentManager mEquipmentManager;
            private TalentManager mTalentManager;

            public int Level { get; private set; }
            public int CurrentExperience { get; private set; }
            private AttributeContainer mBaseAttributes;
            public AttributeContainer Attributes { get; private set; }
            public EquipmentProficienciesContainer Proficiencies { get { return mProfessionManager.Proficiencies; } }

            CharacterBase(UnitGroup ownedBy)
            {
                OwnedBy = ownedBy;
                mProfessionManager = new ProfessionManager();
                mEquipmentManager = new EquipmentManager();
                mStatusManager = new StatusManager();
                mTalentManager = new TalentManager();
            }

            public CharacterBase(UnitGroup ownedBy, int characterId, string characterName, CharacterGender gender, ProfessionType profession, int level)
                : this(ownedBy)
            {
                CharacterID = characterId;
                CharacterName = characterName;
                IsStoryCharacter = false;
                CharacterGender = gender;
                CreateRandomizedAttributes();

                Initialize();
                SetCurrentProfession(profession);

                while (Level < level)
                {
                    LevelUp();
                }
                RandomizeCharacter();
            }

            public CharacterBase (UnitGroup ownedBy, int characterId, string characterName, CharacterGender gender, ProfessionType profession, AttributeContainer attributes, int level)
                : this(ownedBy)
            {
                CharacterID = characterId;
                CharacterName = characterName;
                IsStoryCharacter = true;
                CharacterGender = gender;
                mBaseAttributes = new AttributeContainer(attributes);
                Attributes = new AttributeContainer(attributes);

                Initialize();
                SetCurrentProfession(profession);
                while (Level < level)
                {
                    LevelUp();
                }
            }

            private void Initialize()
            {
                mStatusManager.Initialize(mBaseAttributes, Attributes);
                mEquipmentManager.Initialize( mStatusManager);
                mTalentManager.Initialize(mProfessionManager.GetSkeletonTalentTrees(), this);
            }

            public bool AddCharacterExperience(int amount)
            {
                CurrentExperience += amount;
                if (CurrentExperience >= LEVELUP_EXPERIENCE_REQUIREMENT)
                {
                    CurrentExperience -= LEVELUP_EXPERIENCE_REQUIREMENT;
                    LevelUp();
                    return true;
                }
                return false;
            }

            private void LevelUp()
            {
                Level++;
                mProfessionManager.ApplyProfessionLevelUpAttributes(mBaseAttributes);
                mStatusManager.RebuildAttributes(true);
            }

            public bool AddProfessionExperience(int amount)
            {

                return mProfessionManager.AddExperience(amount);
            }

            public void PostEncounterReset()
            {
                mStatusManager.SetToDefaultStatusEffects();
                for (int resourceIndex = 0; resourceIndex < 3; resourceIndex++)
                {
                    mBaseAttributes[AttributeType.Resource][resourceIndex] = mBaseAttributes[AttributeType.Resource][resourceIndex + 3]; // current = max
                }
            }

            public WornEquipmentPayload[] GetWornEquipmentPayloads()
            {
                return mEquipmentManager.GetWornEquipmentPayloads();
            }

            public UnitPanelPayload GetUnitPanelPayload()
            {
                AllignmentType mainAllignment = AttributeUtil.GetMainAllignment(Attributes[AttributeType.Allignment]);
                AllignmentType[] secondaryAllignments = AttributeUtil.GetSubAllignments(mainAllignment);
                return new UnitPanelPayload(
                    CharacterID,
                    CharacterName, 
                    mProfessionManager.ProfessionName,
                    Level,
                    CurrentExperience,
                    LEVELUP_EXPERIENCE_REQUIREMENT,
                    mProfessionManager.ProfessionLevel,
                    mProfessionManager.ProfessionExperience,
                    mProfessionManager.ProfessionExperienceMax,
                    (int)Attributes[AttributeType.Stat][(int)PrimaryStat.Might],
                    (int)Attributes[AttributeType.Stat][(int)PrimaryStat.Finese],
                    (int)Attributes[AttributeType.Stat][(int)PrimaryStat.Magic],
                    (int)Attributes[AttributeType.Stat][(int)SecondaryStat.Fortitude],
                    (int)Attributes[AttributeType.Stat][(int)SecondaryStat.Attunement],
                    Attributes[AttributeType.Resource][(int)Resource.Health],
                    Attributes[AttributeType.Resource][(int)Resource.MaxHealth],
                    Attributes[AttributeType.Resource][(int)Resource.Endurance],
                    Attributes[AttributeType.Resource][(int)Resource.MaxEndurance],
                    Attributes[AttributeType.Resource][(int)Resource.Mana],
                    Attributes[AttributeType.Resource][(int)Resource.MaxMana],
                    -1,
                    -1,
                    mProfessionManager.GetProfessionImageAsset(CharacterGender),
                    "Allignments/" + mainAllignment.ToString(),
                    mainAllignment.ToString(),
                    secondaryAllignments[0].ToString(),
                    secondaryAllignments[1].ToString(),
                    mStatusManager.GetStatusEffectPayloads()
                    );
            }

            public void RandomizeCharacter()
            {
                mTalentManager.TEMP_UnlockAllTalents(mProfessionManager.CurrentProfessionType);

                // armor
                List<int> armorOptions = mProfessionManager.Proficiencies[EquipmentCategory.Armor];
                if (armorOptions.Count > 0)
                {
                    int armorChoice = UnityEngine.Random.Range(0, armorOptions.Count);
                    mEquipmentManager.EquipInSlot(new ArmorBase((ArmorType)armorOptions[armorChoice]), WornEquipmentSlot.Armor);
                }

                // Held Equipment
                List<int> weaponOptions = mProfessionManager.Proficiencies[EquipmentCategory.Weapon];

                if (mProfessionManager.Proficiencies.HasShieldProficiency)
                {
                    int shieldHand = UnityEngine.Random.Range((int)WornEquipmentSlot.LeftHeld, (int)WornEquipmentSlot.RightHeld + 1);
                    List<int> shieldOptions = mProfessionManager.Proficiencies[EquipmentCategory.Shield];
                    int shieldChoice = UnityEngine.Random.Range(0, shieldOptions.Count);
                    mEquipmentManager.EquipInSlot( new ShieldBase((ShieldType)shieldOptions[shieldChoice]), (WornEquipmentSlot)shieldHand);

                    WornEquipmentSlot otherHand = shieldHand == (int)WornEquipmentSlot.LeftHeld ? WornEquipmentSlot.RightHeld : WornEquipmentSlot.LeftHeld;
                    int weaponChoice = UnityEngine.Random.Range(0, weaponOptions.Count);
                    mEquipmentManager.EquipInSlot(new WeaponBase((WeaponType)weaponOptions[weaponChoice]), otherHand);
                }
                else if (mProfessionManager.Proficiencies.HasDualWieldProficiency)
                {
                    int weaponChoice = UnityEngine.Random.Range(0, weaponOptions.Count);
                    mEquipmentManager.EquipInSlot(new WeaponBase((WeaponType)weaponOptions[weaponChoice]), WornEquipmentSlot.LeftHeld);
                    weaponChoice = UnityEngine.Random.Range(0, weaponOptions.Count);
                    mEquipmentManager.EquipInSlot(new WeaponBase((WeaponType)weaponOptions[weaponChoice]), WornEquipmentSlot.RightHeld);
                }
                else
                {
                    int weaponChoice = UnityEngine.Random.Range(0, weaponOptions.Count);
                    mEquipmentManager.EquipInSlot(new WeaponBase((WeaponType)weaponOptions[weaponChoice]), WornEquipmentSlot.LeftHeld);
                }
            }

            //-----------------------Talent System-------------------------------
            #region TalentSystem

            public void ApplyTalentPoint(TalentIndex index)
            {
                mTalentManager.ApplyTalentPoint(mProfessionManager.CurrentProfessionType, index);
            }

            public TalentTreePayload GetTalentTreePayload()
            {
                return mTalentManager.GetTalentTreePayload(mProfessionManager.CurrentProfessionType);
            }

            #endregion

            //-----------------------StatusEffects-------------------------------
            #region Status Effect Functions
            public Dictionary<StatusEffectIndex, StatusEffect> GetStatusEffects()
            {
                return mStatusManager.StatusEffectMap;
            }
            //--------------------------------------------------------------------
            public List<AttributeModifier> GetStatusPhaseEffects()
            {
                return mStatusManager.StatusPhaseEffectMap.Values.ToList();
            }

            //--------------------------------------------------------------------
            public void ApplyStatusEffect(StatusEffect effect, int numStacks = 1)
            {
                mStatusManager.ApplyStatusEffect(effect, numStacks);
            }

            //--------------------------------------------------------------------
            public bool RemoveStatusEffect(StatusEffectIndex toRemove, int numStacks = -1)
            {
                return mStatusManager.RemoveStatusEffect(toRemove, numStacks);
            }
            #endregion

            //-----------------------Profession--------------------------------
            #region Profession Functions
            public ActionContainer GetActionContainer()
            {
                ActionContainer containerCopy = mProfessionManager.GetActionContainer();

                return containerCopy;
            }

            public List<EventListenerIndex> GetEventListeners()
            {
                return mProfessionManager.GetListeners();
            }

            public List<AuraIndex> GetAuras()
            {
                return mProfessionManager.GetAuras();
            }

            public string GetCurrentProfessionName()
            {
                return mProfessionManager.ProfessionName;
            }

            public void SetCurrentProfession(ProfessionType professionType)
            {
                mTalentManager.DeactivateTree(professionType);
                mProfessionManager.SetCurrentProfession(professionType);
                mTalentManager.ActivateTree(professionType);
                mEquipmentManager.SetProficiencies(mProfessionManager.Proficiencies);
            }

            #endregion

            //-----------------------Equipment--------------------------------
            #region Equipment Functions

            public bool CanEquip(EquipmentBase equipment)
            {
                return mProfessionManager.Proficiencies.CanEquip(equipment);
            }

            public List<EquipmentBase> EquipInSlot(EquipmentBase equipment, WornEquipmentSlot slot)
            {
                return mEquipmentManager.EquipInSlot(equipment, slot);
            }

            public EquipmentBase UnequipSlot(WornEquipmentSlot slot)
            {
                return mEquipmentManager.UnequipSlot(slot);
            }

            public List<string> GetHeldEquipmentModles()
            {
                return mEquipmentManager.GetHeldEquipmentModelIds();
            }

            public List<WeaponBase> GetHeldWeapons()
            {
                return mEquipmentManager.GetHeldWeapons();
            }

            public bool IsEquipped(EquipmentCategory type, int equipment)
            {
                return mEquipmentManager.IsEquipped(type, equipment);
            }

            #endregion
            //------------------------------- TEMP CODE UNTIL CHARACTER LOADING OCCURS-------------------------------------
            private void CreateRandomizedAttributes()
            {
                Dictionary<AttributeType, float[]> attributes = new Dictionary<AttributeType, float[]>();

                float[] stats = new float[(int)CharacterStats.NUM];
                //primary
                float randomRange = 2f;
                stats[(int)PrimaryStat.Might] = AttributeConstants.DefaultMight + UnityEngine.Random.Range(-randomRange, randomRange);
                stats[(int)PrimaryStat.Finese] = AttributeConstants.DefaultFinese + UnityEngine.Random.Range(-randomRange, randomRange);
                stats[(int)PrimaryStat.Magic] = AttributeConstants.DefaultMagic + UnityEngine.Random.Range(-randomRange, randomRange);

                //secondary
                randomRange = 5f;
                stats[(int)SecondaryStat.Attunement] = AttributeConstants.DefaultAttunement + UnityEngine.Random.Range(-randomRange, randomRange);
                stats[(int)SecondaryStat.Fortitude] = AttributeConstants.DefaultFortitude + UnityEngine.Random.Range(-randomRange, randomRange);

                //tertiary
                stats[(int)TertiaryStat.Speed] = AttributeConstants.DefaultSpeed;
                stats[(int)TertiaryStat.Movement] = AttributeConstants.DefaultMovement;
                stats[(int)TertiaryStat.Jump] = AttributeConstants.DefaultJump;
                stats[(int)TertiaryStat.EnduranceRecovery] = AttributeConstants.DefaultEnduranceRecovery;
                attributes.Add(AttributeType.Stat, stats);

                float[] resources = new float[(int)Resource.NUM];
                AttributeUtil.CalculateResourcesFromStats(stats, resources, false);
                attributes.Add(AttributeType.Resource, resources);

                float[] allignments = new float[(int)AllignmentType.NUM];
                allignments[(int)AllignmentType.Light] = AttributeConstants.MainAllignment;
                allignments[(int)AllignmentType.Sky] = AttributeConstants.SubAllignment;
                allignments[(int)AllignmentType.Fire] = AttributeConstants.SubAllignment;

                attributes.Add(AttributeType.Allignment, allignments);
                attributes.Add(AttributeType.Status, new float[(int)StatusType.NUM]);

                mBaseAttributes = new AttributeContainer(attributes);
                Attributes = new AttributeContainer(mBaseAttributes);
            }

            public bool IsCurrentProfessionSpecialized()
            {
                return mProfessionManager.IsCurrentProfessionSpecialized();
            }

            public List<EventListenerIndex> GetListeners()
            {
                return mProfessionManager.GetListeners();
            }
        }
    }
}

