using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.Character.Internal
{
    static class CharacterModifier
    {
        private static readonly string TAG = "CharacterModifier";

        public static void AssignTalentPoint(CharacterInfo toAssign, TalentId toTalent)
        {
            Logger.Assert(toAssign.CurrentSpecialization.Talents.ContainsKey(toTalent), LogTag.Character, TAG,
                string.Format("::AssignTalentPoint() - [{0}]'s current specialization [{1}] doesn't contain talent [{2}]", toAssign.Name, toAssign.CurrentSpecializationType.ToString(), toTalent.ToString()));
            if (toAssign.CurrentSpecialization.Talents.ContainsKey(toTalent))
            {
                Talent talent = toAssign.CurrentSpecialization.Talents[toTalent];

                Logger.Assert(talent.PointsAssigned < talent.MaxPoints, LogTag.Character, TAG,
                string.Format("::AssignTalentPoint() - [{0}]'s current specialization [{1}]'s talent [{2}] is already at max points", toAssign.Name, toAssign.CurrentSpecializationType.ToString(), toTalent.ToString()));
                if (talent.PointsAssigned < talent.MaxPoints)
                {
                    talent.PointsAssigned++;
                }
            }
        }

        public static List<int> EquipCharacter(CharacterInfo toEquip, Equippable equippable, Equipment.Slot inSlot)
        {
            List<int> unequippedItems = new List<int>();

            bool fitsInSlot = EquipmentUtil.FitsInSlot(equippable.EquipmentTag.Category, inSlot);
            bool hasProficiency = EquipmentUtil.HasProficiencyFor(toEquip.CurrentSpecialization, equippable);
            Logger.Assert(fitsInSlot && hasProficiency, LogTag.GameSystems, TAG, string.Format("EquipCharacter() - Item [{0}] doesn't fit in slot [{1}].", equippable.EquipmentId.ToString(), inSlot.ToString()));
            if (fitsInSlot && hasProficiency)
            {
                // Apply modifiers to the equipment itself incase it changes how it is equipped
                foreach (EquippableModifier equippableModifier in toEquip.EquippableModifiers)
                {
                    equippableModifier.Modify(equippable);
                }

                // unequip slots that new item will now occupy
                if (!EquipmentUtil.IsSlotEmpty(toEquip.Equipment, inSlot))
                {
                    unequippedItems.Add(UnEquipCharacter(toEquip, inSlot).Value);
                }

                if (inSlot == Equipment.Slot.LeftHand || inSlot == Equipment.Slot.RightHand)
                {
                    int numHands = (equippable as HeldEquippable).NumHandsRequired;
                    if (numHands == 2)
                    {
                        Equipment.Slot otherSlot = inSlot == Equipment.Slot.LeftHand ? Equipment.Slot.RightHand : Equipment.Slot.LeftHand;
                        if (!EquipmentUtil.IsSlotEmpty(toEquip.Equipment, otherSlot))
                        {
                            unequippedItems.Add(UnEquipCharacter(toEquip, otherSlot).Value);
                        }
                    }
                }

                // Finall, equip the item
                toEquip.Equipment[inSlot] = equippable;

                // Update character with equipment modifiers
                foreach (AttributeModifier attributeModifier in equippable.EquipBonuses)
                {
                    toEquip.Attributes.Modify(attributeModifier);
                }
            }
            else
            {
                unequippedItems.Add((int)equippable.EquipmentId);
            }

            return unequippedItems;
        }

        public static void RefreshCharacterAppearance(CharacterInfo characterInfo, Appearance toRefresh)
        {
            toRefresh.AppearanceId = characterInfo.AppearanceId;

            // Update Portrait
            if (CharacterUtil.GetCharacterTypeFromId(characterInfo.Id) == CharacterType.Create 
                || toRefresh.PortraitSpriteId == PortraitSpriteId.INVALID)
            {
                toRefresh.PortraitSpriteId = SpecializationUtil.GetPortraitSpriteIdForSpecialization(characterInfo.CurrentSpecializationType);
            }

            // Update Body Type
            toRefresh.BodyType = BodyType.Body_0;

            // Update Equipment
            for (int equipmentSlotIdx = 0; equipmentSlotIdx < (int)Equipment.Slot.NUM; ++equipmentSlotIdx)
            {
                Equipment.Slot slot = (Equipment.Slot)equipmentSlotIdx;
                AppearancePrefabId newPrefabId = characterInfo.Equipment[slot] == Equipment.NO_EQUIPMENT ? AppearancePrefabId.prefab_none : characterInfo.Equipment[slot].PrefabId;

                switch (slot)
                {
                    case Equipment.Slot.Accessory:      /* empty */ break;
                    case Equipment.Slot.Armor:          toRefresh.ArmorId        = newPrefabId; break;
                    case Equipment.Slot.LeftHand:       toRefresh.LeftHeldId     = newPrefabId; break;
                    case Equipment.Slot.RightHand:      toRefresh.RightHeldId    = newPrefabId; break;
                }
            }
        }

        public static List<int> RefreshCharactersEquipment(CharacterInfo toRefresh)
        {
            List<int> unequippedItems = new List<int>();

            Specialization specialization = toRefresh.CurrentSpecialization;

            for (int equipmentSlotIdx = 0; equipmentSlotIdx < (int)Equipment.Slot.NUM; ++equipmentSlotIdx)
            {
                Equipment.Slot slot = (Equipment.Slot)equipmentSlotIdx;
                Equippable equippable = toRefresh.Equipment[slot];
                if (equippable != Equipment.NO_EQUIPMENT)
                {
                    if (!EquipmentUtil.HasProficiencyFor(specialization, equippable))
                    {
                        unequippedItems.Add(UnEquipCharacter(toRefresh, slot).Value);
                    }
                }
            }

            return unequippedItems;
        }

        public static void ResetTalentPoints(CharacterInfo toReset)
        {
            foreach (Talent talent in toReset.CurrentSpecialization.Talents.Values)
            {
                talent.PointsAssigned = 0;
            }
        }

        public static Optional<int> UnEquipCharacter(CharacterInfo toUnEquip, Equipment.Slot inSlot)
        {
            Optional<int> unequipped = Optional<int>.Empty;
            if (!EquipmentUtil.IsSlotEmpty(toUnEquip.Equipment, inSlot))
            {
                Equippable unEquiped = toUnEquip.Equipment[inSlot];

                unequipped = (int)unEquiped.EquipmentId;

                // Remove equipment modifiers
                foreach (AttributeModifier attributeModifier in unEquiped.EquipBonuses)
                {
                    toUnEquip.Attributes.Revert(attributeModifier);
                }

                if (EquipmentUtil.IsHeld(inSlot)) // replace with fists
                {
                    toUnEquip.Equipment[inSlot] = ItemFactory.LoadEquipable(EquippableId.Fists_0);
                    // Apply modifiers to the equipment itself incase it changes how it is equipped
                    foreach (EquippableModifier equippableModifier in toUnEquip.EquippableModifiers)
                    {
                        equippableModifier.Modify(toUnEquip.Equipment[inSlot]);
                    }
                }
                else
                {
                    toUnEquip.Equipment[inSlot] = Equipment.NO_EQUIPMENT;
                }
            }

            return unequipped;
        }

        public static void Debug_MaxTalentPoints(CharacterInfo toMax)
        {
            foreach (Talent talent in toMax.CurrentSpecialization.Talents.Values)
            {
                talent.PointsAssigned = talent.MaxPoints;
            }
        }
    }
}
