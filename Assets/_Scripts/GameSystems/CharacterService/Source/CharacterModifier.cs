using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Characters.Internal
{
    static class CharacterModifier
    {
        private static readonly string TAG = "CharacterModifier";

        public static void AssignTalentPoint(CharacterInfo toAssign, TalentId toTalent)
        {
            Logger.Assert(toAssign.CurrentSpecializationProgress.TalentProgress.ContainsKey(toTalent), LogTag.Character, TAG,
                string.Format("::AssignTalentPoint() - [{0}]'s current specialization [{1}] doesn't contain talent [{2}]", toAssign.Name, toAssign.CurrentSpecializationType.ToString(), toTalent.ToString()));
            if (toAssign.CurrentSpecializationProgress.TalentProgress.ContainsKey(toTalent))
            {
                TalentProgress talentProgress = toAssign.CurrentSpecializationProgress.TalentProgress[toTalent];

                Logger.Assert(talentProgress.CurrentPoints < talentProgress.MaxPoints, LogTag.Character, TAG,
                string.Format("::AssignTalentPoint() - [{0}]'s current specialization [{1}]'s talent [{2}] is already at max points", toAssign.Name, toAssign.CurrentSpecializationType.ToString(), toTalent.ToString()));
                if (talentProgress.CurrentPoints < talentProgress.MaxPoints)
                {
                    talentProgress.CurrentPoints++;
                }
            }
        }


        //public static void RefreshCharacterAppearance(CharacterInfo characterInfo, Appearance toRefresh)
        //{
        //    toRefresh.AppearanceId = characterInfo.AppearanceId;

        //    // Update Portrait
        //    if (CharacterUtil.GetCharacterTypeFromId(characterInfo.Id) == CharacterType.Create 
        //        || toRefresh.PortraitSpriteId == PortraitSpriteId.INVALID)
        //    {
        //        toRefresh.PortraitSpriteId = SpecializationUtil.GetPortraitSpriteIdForSpecialization(characterInfo.CurrentSpecializationType);
        //    }

        //    // Update Body Type
        //    toRefresh.BodyType = BodyType.Body_0;

        //    // Update Equipment
        //    for (int equipmentSlotIdx = 0; equipmentSlotIdx < (int)Equipment.Slot.NUM; ++equipmentSlotIdx)
        //    {
        //        Equipment.Slot slot = (Equipment.Slot)equipmentSlotIdx;
        //        AppearancePrefabId newPrefabId = characterInfo.Equipment[slot] == Equipment.NO_EQUIPMENT ? AppearancePrefabId.prefab_none : characterInfo.Equipment[slot].PrefabId;

        //        switch (slot)
        //        {
        //            case Equipment.Slot.Accessory:      /* empty */ break;
        //            case Equipment.Slot.Armor:          toRefresh.ArmorId        = newPrefabId; break;
        //            case Equipment.Slot.LeftHand:       toRefresh.LeftHeldId     = newPrefabId; break;
        //            case Equipment.Slot.RightHand:      toRefresh.RightHeldId    = newPrefabId; break;
        //        }
        //    }
        //}

        public static List<int> RefreshCharactersEquipment(CharacterInfo info)
        {
            List<int> unequippedItems = new List<int>();

            Character character = new Character(info);
            Specialization specialization = character.CurrentSpecialization;

            for (int equipmentSlotIdx = 0; equipmentSlotIdx < (int)Equipment.Slot.NUM; ++equipmentSlotIdx)
            {
                Equipment.Slot slot = (Equipment.Slot)equipmentSlotIdx;
                if (info.EquippedItems[equipmentSlotIdx] != EquippableId.INVALID)
                {
                    Equippable equippable = ItemFactory.LoadEquipable(info.EquippedItems[equipmentSlotIdx]);
                    if (!EquipmentUtil.HasProficiencyFor(character.GetProficiencies(), equippable))
                    {
                        unequippedItems.Add((int)info.EquippedItems[equipmentSlotIdx]);
                        info.EquippedItems[equipmentSlotIdx] = EquippableId.INVALID;
                    }
                }
            }

            return unequippedItems;
        }

        public static void ResetTalentPoints(CharacterInfo toReset)
        {
            foreach (SpecializationProgress specializationProgress in toReset.SpecializationsProgress)
            {
                foreach (TalentProgress talent in specializationProgress.TalentProgress.Values)
                {
                    talent.CurrentPoints = 0;
                }

            }
        }

        public static void Debug_MaxTalentPoints(CharacterInfo toMax)
        {
            foreach (SpecializationProgress specializationProgress in toMax.SpecializationsProgress)
            {
                foreach (TalentProgress talent in specializationProgress.TalentProgress.Values)
                {
                    talent.CurrentPoints = talent.MaxPoints;
                }

            }
        }
    }
}
