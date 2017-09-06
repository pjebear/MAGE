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
        public int ProfessionLevel;
        public float CurrentHealth;
        public float MaxHealth;
        public float CurrentMana;
        public float MaxMana;
        public float CurrentEndurance;
        public float MaxEndurance;
        public float CurrentClock;
        public float MaxClock;
        public string ImageAssetPath;
        public List<StatusEffectIconPayload> StatusPayloads;

        public UnitPanelPayload(int unitId, string unitName, string professionName, int characterLevel, int professionLevel,
            float currentHealth, float maxHealth, float currentEndurance, float maxEndurance, float currentMana, float maxMana,
            float currentClock, float maxClock, string imageAssetPath, List<StatusEffectIconPayload> statusPayloads)
        {
            UnitId = unitId;
            UnitName = unitName;
            ProfessionName = professionName;
            CharacterLevel = characterLevel;
            ProfessionLevel = professionLevel;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
            CurrentEndurance = currentEndurance;
            MaxEndurance = maxEndurance;
            CurrentMana = currentMana;
            MaxMana = maxMana;
            CurrentClock = currentClock;
            MaxClock = maxClock;
            ImageAssetPath = imageAssetPath;
            StatusPayloads = statusPayloads;
        }
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
