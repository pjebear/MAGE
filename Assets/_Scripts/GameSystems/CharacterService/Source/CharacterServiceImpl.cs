using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAGE.Services;

namespace MAGE.GameServices.Character.Internal
{
    class CharacterServiceImpl : ICharacterService
    {
        private readonly string TAG = "CharacterServiceImpl";
        private Dictionary<int, HashSet<ICharacterUpdateListener>> mCharacterUpdateListeners = new Dictionary<int, HashSet<ICharacterUpdateListener>>();

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
            if (mCharacterUpdateListeners.ContainsKey(characterId))
            {
                foreach (ICharacterUpdateListener listener in new HashSet<ICharacterUpdateListener>(mCharacterUpdateListeners[characterId]))
                {
                    listener.OnCharacterUpdated(characterId);
                }
            }
        }

        public void AssignExperience(int characterId, int experience)
        {
            CharacterInfo characterInfo = GetCharacterInfo(characterId);

            // Character experience
            characterInfo.Experience += experience;
            if (characterInfo.Experience > CharacterConstants.LEVEL_UP_THRESHOLD)
            {
                characterInfo.Experience -= CharacterConstants.LEVEL_UP_THRESHOLD;
                characterInfo.Level++;

                foreach (AttributeModifier modifier in characterInfo.CurrentSpecialization.LevelUpModifiers)
                {
                    Logger.Assert(modifier.ModifierType == ModifierType.Increment, LogTag.GameSystems, TAG,
                        string.Format("Invalid Levelup modifier for Specialization [{0}] - {1}", characterInfo.CurrentSpecialization.ToString(), modifier.ToString()), LogLevel.Warning);

                    characterInfo.Attributes.Modify(modifier);
                }
            }

            // Specialization experience
            // Update specialization
            characterInfo.CurrentSpecialization.Experience += SpecializationConstants.LEVEL_UP_THRESHOLD;
            if (characterInfo.CurrentSpecialization.Experience >= SpecializationConstants.LEVEL_UP_THRESHOLD)
            {
                characterInfo.CurrentSpecialization.Level++;
            }

            WriteCharacter(characterInfo, false);
        }

        public void AssignTalentPoint(int characterId, TalentId talentId)
        {
            CharacterInfo toAssign = GetCharacterInfo(characterId);

            CharacterModifier.AssignTalentPoint(toAssign, talentId);

            // Refresh DB
            WriteCharacter(toAssign, false);
        }

        public List<int> ChangeSpecialization(int characterId, SpecializationType specializationType)
        {
            CharacterInfo toSpecialize = GetCharacterInfo(characterId);

            //  TODO: Sanity check which specialization is being switched to
            toSpecialize.CurrentSpecializationType = specializationType;

            // Remove any items the character can no longer equip
            List<int> unequippedItems = CharacterModifier.RefreshCharactersEquipment(toSpecialize);

            // Refresh the characters appearance to reflect new specialization
            WriteCharacter(toSpecialize, true);

            return unequippedItems;
        }

        public int CreateCharacter(CharacterCreateParams createParams)
        {
            DB.DBCharacter dbCharacter;
            DB.DBAppearance dbAppearance;
            CharacterCreator.CreateCharacter(createParams, out dbCharacter, out dbAppearance);

            CharacterInfo characterInfo = CharacterDBUtil.FromDB(dbCharacter);

            Appearance appearance = AppearanceUtil.FromDB(dbAppearance);
            CharacterModifier.RefreshCharacterAppearance(characterInfo, appearance);
            DBService.Get().WriteAppearance(characterInfo.AppearanceId, AppearanceUtil.ToDB(appearance));

            // Appearance is manually set. doesn't need a refresh
            WriteCharacter(characterInfo, false);

            return characterInfo.Id;
        }

        public List<int> EquipCharacter(int characterId, EquippableId equippableId, Equipment.Slot inSlot)
        {
            CharacterInfo toEquip = GetCharacterInfo(characterId);

            Equippable equipable = ItemFactory.LoadEquipable(equippableId);

            List<int> unequippedItems = CharacterModifier.EquipCharacter(toEquip, equipable, inSlot);

            // Update Appearance after changing items
            WriteCharacter(toEquip, true);

            return unequippedItems;
        }

        public int GetCharacterAppearanceId(int characterId)
        {
            DB.DBCharacter dbCharacter = DBService.Get().LoadCharacter(characterId);

            return dbCharacter.AppearanceId;
        }

        public CharacterInfo GetCharacterInfo(int characterId)
        {
            DB.DBCharacter dbCharacter = DBService.Get().LoadCharacter(characterId);

            return CharacterDBUtil.FromDB(dbCharacter);
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
            WriteCharacter(toReset, true);

            return unequippedItems;
        }

        public void RegisterForCharacterChanges(int characterId, ICharacterUpdateListener characterUpdateListener)
        {
            if (!mCharacterUpdateListeners.ContainsKey(characterId))
            {
                mCharacterUpdateListeners.Add(characterId, new HashSet<ICharacterUpdateListener>());
            }
            Logger.Assert(!mCharacterUpdateListeners[characterId].Contains(characterUpdateListener), LogTag.Character, TAG, "::RegisterForCharacterChanges() - Duplicate Listener Registered");
            if (!mCharacterUpdateListeners[characterId].Contains(characterUpdateListener))
            {
                mCharacterUpdateListeners[characterId].Add(characterUpdateListener);
            }
        }

        public List<int> UnEquipCharacter(int characterId, Equipment.Slot inSlot)
        {
            List<int> unequippedItems = new List<int>();

            CharacterInfo toUnEquip = GetCharacterInfo(characterId);

            Optional<int> unequipped = CharacterModifier.UnEquipCharacter(toUnEquip, inSlot);
            if (unequipped.HasValue)
            {
                unequippedItems.Add(unequipped.Value);
            }

            // Refresh DB
            WriteCharacter(toUnEquip, true);

            return unequippedItems;
        }

        public void UnRegisterForCharacterChanges(int characterId, ICharacterUpdateListener characterUpdateListener)
        {
            if (mCharacterUpdateListeners.ContainsKey(characterId))
            {
                mCharacterUpdateListeners[characterId].Remove(characterUpdateListener);
            }
        }

        // Debug
        public void Debug_MaxOutTalents(int characterId)
        {
            CharacterInfo toMax = GetCharacterInfo(characterId);

            CharacterModifier.Debug_MaxTalentPoints(toMax);

            // Refresh DB
            WriteCharacter(toMax, false);
        }

        // Private 
        private void WriteCharacter(CharacterInfo character, bool refreshAppearance)
        {
            DBService.Get().WriteCharacter(CharacterDBUtil.ToDB(character));

            if (refreshAppearance)
            {
                Appearance appearance = AppearanceUtil.FromDB(DBService.Get().LoadAppearance(character.AppearanceId));
                CharacterModifier.RefreshCharacterAppearance(character, appearance);
                DBService.Get().WriteAppearance(character.AppearanceId, AppearanceUtil.ToDB(appearance));
            }
        }
    }
}
