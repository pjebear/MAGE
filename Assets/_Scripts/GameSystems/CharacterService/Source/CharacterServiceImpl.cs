using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using MAGE.Services;

namespace MAGE.GameSystems.Characters.Internal
{
    class CharacterServiceImpl : ICharacterService
    {
        private readonly string TAG = "CharacterServiceImpl";

        public void Init()
        {
            DBService.Get().RegisterForCharacterUpdates(this, OnCharacterDBUpdated);
        }

        public void Takedown()
        {
            DBService.Get().UnRegisterForCharacterUpdates(this);
        }

        // DB Updates
        public void OnCharacterDBUpdated(int characterId)
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new CharacterMessage(CharacterMessage.MessageType.CharacterUpdated, characterId));
        }

        public CharacterGrowthInfo AssignExperience(int characterId, int experience)
        {
            CharacterGrowthInfo characterGrowthInfo = new CharacterGrowthInfo();
            characterGrowthInfo.CharacterId = characterId;

            CharacterInfo characterInfo = GetCharacterInfo(characterId);

            // Character experience
            characterGrowthInfo.Xp = experience;
            characterGrowthInfo.AttributeModifiers = new List<AttributeModifier>();
            characterInfo.Experience += experience;
            while (characterInfo.Experience >= CharacterConstants.LEVEL_UP_THRESHOLD)
            {
                characterInfo.Experience -= CharacterConstants.LEVEL_UP_THRESHOLD;
                characterInfo.Level++;

                // Health Increase
                {
                    AttributeModifier healthIncrease = new AttributeModifier(ResourceType.Health, ModifierType.Increment, 5);
                    characterGrowthInfo.AttributeModifiers.Add(healthIncrease);
                    characterInfo.Attributes.Modify(healthIncrease);
                }

                // Mana Increase
                {
                    AttributeModifier manaIncrease = new AttributeModifier(ResourceType.Mana, ModifierType.Increment, 2);
                    characterGrowthInfo.AttributeModifiers.Add(manaIncrease);
                    characterInfo.Attributes.Modify(manaIncrease);
                }

                Specialization specialization = SpecializationFactory.CheckoutSpecialization(characterInfo.CurrentSpecializationType, characterInfo.CurrentSpecializationProgress);
                foreach (AttributeModifier modifier in specialization.LevelUpModifiers)
                {
                    Logger.Assert(modifier.ModifierType == ModifierType.Increment, LogTag.GameSystems, TAG,
                        string.Format("Invalid Levelup modifier for Specialization [{0}] - {1}", characterInfo.CurrentSpecializationType.ToString(), modifier.ToString()), LogLevel.Warning);

                    characterGrowthInfo.AttributeModifiers.Add(modifier);
                    characterInfo.Attributes.Modify(modifier);
                }
            } 
            characterGrowthInfo.CharacterLevel = characterInfo.Level;

            // Specialization experience
            // Update specialization
            characterGrowthInfo.SpecializationXp = SpecializationConstants.LEVEL_UP_THRESHOLD;
            characterInfo.CurrentSpecializationProgress.Experience += SpecializationConstants.LEVEL_UP_THRESHOLD;
            while (characterInfo.CurrentSpecializationProgress.Experience >= SpecializationConstants.LEVEL_UP_THRESHOLD)
            {
                characterInfo.CurrentSpecializationProgress.Level++;
                characterInfo.CurrentSpecializationProgress.Experience -= SpecializationConstants.LEVEL_UP_THRESHOLD;
            }
            characterGrowthInfo.SpecializationLvl = characterInfo.CurrentSpecializationProgress.Level++;

            WriteCharacter(characterInfo);

            return characterGrowthInfo;
        }

        public void AssignTalentPoint(int characterId, TalentId talentId)
        {
            CharacterInfo toAssign = GetCharacterInfo(characterId);

            CharacterModifier.AssignTalentPoint(toAssign, talentId);

            // Refresh DB
            WriteCharacter(toAssign);
        }

        public List<int> ChangeSpecialization(int characterId, SpecializationType specializationType)
        {
            CharacterInfo toSpecialize = GetCharacterInfo(characterId);

            //  TODO: Sanity check which specialization is being switched to
            toSpecialize.CurrentSpecializationType = specializationType;

            // Remove any items the character can no longer equip
            List<int> unequippedItems = CharacterModifier.RefreshCharactersEquipment(toSpecialize);

            // Refresh the characters appearance to reflect new specialization
            WriteCharacter(toSpecialize);

            Appearance appearance = GetCharacterAppearance(characterId);
            UpdateCharacterAppearance(toSpecialize, appearance);
            DBService.Get().WriteAppearance(characterId, AppearanceUtil.ToDB(appearance));

            return unequippedItems;
        }

        public int CreateCharacter(CharacterCreateParams createParams)
        {
            DB.DBCharacter dbCharacter;
            CharacterCreator.CreateCharacter(createParams, out dbCharacter);

            CharacterInfo characterInfo = CharacterDBUtil.FromDB(dbCharacter);
            WriteCharacter(characterInfo);

            Appearance appearance = createParams.appearanceOverrides;
            UpdateCharacterAppearance(characterInfo, appearance);

            DBService.Get().WriteAppearance(characterInfo.Id, AppearanceUtil.ToDB(appearance));

            if (createParams.level > 1)
            {
                int experience = createParams.level * CharacterConstants.LEVEL_UP_THRESHOLD;
                AssignExperience(characterInfo.Id, experience);
            }

            return characterInfo.Id;
        }

        public int CreateMob(Mobs.MobId mobId, int level)
        {
            return CreateCharacter(MobFactory.GetCreateParamsForMob(mobId, level));
        }

        public List<int> EquipCharacter(int characterId, EquippableId equippableId, Equipment.Slot inSlot)
        {
            Character toEquip = GetCharacter(characterId);

            Equippable equipable = ItemFactory.LoadEquipable(equippableId);

            List<int> unequippedItems = toEquip.Equip(equipable, inSlot);

            // Update Appearance after changing items
            CharacterInfo characterInfo = toEquip.GetInfo();
            WriteCharacter(characterInfo);

            Appearance appearance = GetCharacterAppearance(characterId);
            UpdateCharacterAppearance(characterInfo, appearance);
            DBService.Get().WriteAppearance(characterId, AppearanceUtil.ToDB(appearance));

            return unequippedItems;
        }

        public Character GetCharacter(int characterId)
        {
            CharacterInfo characterInfo = GetCharacterInfo(characterId);
            return CharacterCreator.FromInfo(characterInfo); 
        }

        public void DeleteCharacter(int characterId)
        {
            DBService.Get().ClearCharacter(characterId);
        }

        public List<int> GetCharactersOfType(CharacterType characterType)
        {
            return DBService.Get().GetAllCharacterIds().Where(x => CharacterUtil.GetCharacterTypeFromId(x) == characterType).ToList();
        }

        public List<int> ResetTalentPoints(int characterId)
        {
            CharacterInfo toReset = GetCharacterInfo(characterId);

            CharacterModifier.ResetTalentPoints(toReset);

            // Refresh Equipment in case proficiencies have changed
            List<int> unequippedItems = CharacterModifier.RefreshCharactersEquipment(toReset);

            // Refresh Appearance in case talents modified appearance, and to reflect any unequipped items
            WriteCharacter(toReset);

            // Update Appearance after changing items
            Appearance appearance = GetCharacterAppearance(characterId);
            UpdateCharacterAppearance(toReset, appearance);
            DBService.Get().WriteAppearance(characterId, AppearanceUtil.ToDB(appearance));

            return unequippedItems;
        }

        public List<int> UnEquipCharacter(int characterId, Equipment.Slot inSlot)
        {
            List<int> unequippedItems = new List<int>();

            Character toUnEquip = GetCharacter(characterId);

            Optional<int> unequipped = toUnEquip.UnEquip(inSlot);
            if (unequipped.HasValue)
            {
                unequippedItems.Add(unequipped.Value);
            }

            // Refresh DB
            CharacterInfo characterInfo = toUnEquip.GetInfo();
            WriteCharacter(characterInfo);

            Appearance appearance = GetCharacterAppearance(characterId);
            UpdateCharacterAppearance(characterInfo, appearance);
            DBService.Get().WriteAppearance(characterId, AppearanceUtil.ToDB(appearance));

            return unequippedItems;
        }

        // Debug
        public void Debug_MaxOutTalents(int characterId)
        {
            CharacterInfo toMax = GetCharacterInfo(characterId);

            CharacterModifier.Debug_MaxTalentPoints(toMax);

            // Refresh DB
            WriteCharacter(toMax);
        }

        // Private 
        private CharacterInfo GetCharacterInfo(int characterId)
        {
            DB.DBCharacter dbCharacter = DBService.Get().LoadCharacter(characterId);

            return CharacterDBUtil.FromDB(dbCharacter);
        }

        private Appearance GetCharacterAppearance(int characterId)
        {
            DB.DBAppearance dbAppearance = DBService.Get().LoadAppearance(characterId);

            return AppearanceUtil.FromDB(dbAppearance);
        }

        private void UpdateCharacterAppearance(CharacterInfo character, Appearance appearance)
        {
            // Portrait
            appearance.BasePortraitSpriteId = SpecializationUtil.GetPortraitSpriteIdForSpecialization(character.CurrentSpecializationType);
            
            // Update Equipment
            for (int equipmentSlotIdx = 0; equipmentSlotIdx < (int)Equipment.Slot.NUM; ++equipmentSlotIdx)
            {
                EquippableId equipmentId = character.EquippedItems[equipmentSlotIdx];
                ApparelAssetId prefabId = ApparelAssetId.NONE;
                if (equipmentId != EquippableId.INVALID)
                {
                    Equippable equippable = ItemFactory.LoadEquipable(equipmentId);
                    prefabId = SpecializationUtil.GetEquipmentApparelAssetIdForSpecialization(equippable, character.CurrentSpecializationType);
                }

                switch ((Equipment.Slot)equipmentSlotIdx)
                {
                    case Equipment.Slot.Accessory:      /* empty */ break;
                    case Equipment.Slot.Armor:          appearance.OverrideOutfitType       = prefabId; break;
                    case Equipment.Slot.LeftHand:       appearance.OverrideLeftHeldAssetId  = prefabId; break;
                    case Equipment.Slot.RightHand:      appearance.OverrideRightHeldAssetId = prefabId; break;
                    case Equipment.Slot.RangedWeapon:   appearance.OverrideRangedAssetId    = prefabId; break;
                }
            }
        }

        private void WriteCharacter(CharacterInfo character)
        {
            DBService.Get().WriteCharacter(CharacterDBUtil.ToDB(character));
        }
    }
}
