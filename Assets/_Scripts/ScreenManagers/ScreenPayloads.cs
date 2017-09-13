using Common.ProfessionEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Talents;

namespace Screens.Payloads
{
    public struct UnitPanelPayload
    {
        public int UnitId;
        public string UnitName;
        public string ProfessionName;
        public int CharacterLevel;
        public int Experience;
        public int ExperienceMax;
        public int ProfessionLevel;
        public int ProfessionExperience;
        public int ProfessionExperienceMax;
        public int Might;
        public int Finese;
        public int Magic;
        public int Fortitude;
        public int Attunement;
        public float CurrentHealth;
        public float MaxHealth;
        public float CurrentMana;
        public float MaxMana;
        public float CurrentEndurance;
        public float MaxEndurance;
        public string ImageAssetPath;
        public string AllignmentAssetPath;
        public string MainAllignment;
        public string SecondaryAllignment;
        public string TertiaryAllignment;
        public float CurrentClock;
        public float MaxClock;
     

        public List<StatusEffectIconPayload> StatusPayloads;

        public UnitPanelPayload(int unitId, string unitName, string professionName, 
            int characterLevel, 
            int experience, int maxExperience,
            int professionLevel,
            int professionExperience, int maxProfessionExperience,
            int might, int finese, int magic, int fortitude, int attunement,
            float currentHealth, float maxHealth, float currentEndurance, float maxEndurance, float currentMana, float maxMana,
            float currentClock, float maxClock, 
            string imageAssetPath, 
            string allignmentAssetPath, string primaryAllignment, string secondaryAllignment, string tertiaryAllignment,
            List<StatusEffectIconPayload> statusPayloads)
        {
            UnitId = unitId;
            UnitName = unitName;
            ProfessionName = professionName;
            CharacterLevel = characterLevel;
            Experience = experience;
            ExperienceMax = maxExperience;
            ProfessionLevel = professionLevel;
            ProfessionExperience = professionExperience;
            ProfessionExperienceMax = maxProfessionExperience;
            Might = might;
            Finese = finese;
            Magic = magic;
            Attunement = attunement;
            Fortitude = fortitude;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
            CurrentEndurance = currentEndurance;
            MaxEndurance = maxEndurance;
            CurrentMana = currentMana;
            MaxMana = maxMana;
            CurrentClock = currentClock;
            MaxClock = maxClock;
            ImageAssetPath = imageAssetPath;
            AllignmentAssetPath = allignmentAssetPath;
            MainAllignment = primaryAllignment;
            SecondaryAllignment = secondaryAllignment;
            TertiaryAllignment = tertiaryAllignment;
            StatusPayloads = statusPayloads;
        }
    }

    public struct WornEquipmentPayload
    {
        public string ImageAssetPath;
        public bool Selectable;
    }

    public struct InventoryEquipmentPayload
    {
        public string IconAssetPath;
        public string EquipmentName;
        public int Count;
        public KeyValuePair<int, int> EquipmentIndex;
    }

    public struct TalentTreePayload
    {
        public ProfessionType ProfessionType;
        public Dictionary<TalentIndex, TalentPayload> TalentPayloads;
        public int AvailablePoints;

        public TalentTreePayload(Dictionary<TalentIndex, TalentPayload> talentPayloads, int availablePoints, ProfessionType type)
        {
            AvailablePoints = availablePoints;
            ProfessionType = type;
            TalentPayloads = talentPayloads;
        }
    }

    public struct TalentPayload
    {
        public TalentIndex TalentIndex;
        public int CurrentPoints;
        public int MaxPoints;
        public bool IsUnlocked;

        public TalentPayload(TalentIndex index, int currentPoints, int maxPoints, bool isUnlocked)
        {
            TalentIndex = index;
            CurrentPoints = currentPoints;
            MaxPoints = maxPoints;
            IsUnlocked = isUnlocked;
        }
    }
    
    public struct StatusEffectIconPayload
    {
        public int AssetId;
        public string ToolTip;
        public int NumStacks;
        public bool IsBeneficial;

        public StatusEffectIconPayload(int assetId, string toolTip, int numStacks, bool isBeneficial)
        {
            AssetId = assetId;
            ToolTip = toolTip;
            NumStacks = numStacks;
            IsBeneficial = isBeneficial;
        }
    }

}
