using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.EquipmentEnums;
using Common.EquipmentTypes;
using Common.StatusTypes;
using Common.ActionEnums;
using WorldSystem.Character;
using Common.EquipmentUtil;
using Screens.Payloads;

namespace WorldSystem.Character
{
    class EquipmentManager
    {
        EquipmentBase[] mWornEquipment;
        protected EquipmentProficienciesContainer rEquipmentProficiencies;
        protected StatusManager rStatusManager;

        public EquipmentManager()
        {
            mWornEquipment = new EquipmentBase[(int)WornEquipmentSlot.NUM];
            mWornEquipment[(int)WornEquipmentSlot.LeftHeld] = new WeaponBase(WeaponType.Unarmed);
            mWornEquipment[(int)WornEquipmentSlot.RightHeld] = new WeaponBase(WeaponType.Unarmed);
            
            rEquipmentProficiencies = null;
        }

        public void Initialize(StatusManager statusManager)
        {
            rStatusManager = statusManager;
        }

        public List<EquipmentBase> SetProficiencies(EquipmentProficienciesContainer proficiencies)
        {
            List<EquipmentBase> equipmentNeedingRemoval = new List<EquipmentBase>();
            rEquipmentProficiencies = proficiencies;
            // unequip anything that is no longer in proficiencies
            if (mWornEquipment[(int)WornEquipmentSlot.Armor] != null &&
                !rEquipmentProficiencies.CanEquip(mWornEquipment[(int)WornEquipmentSlot.Armor]))
            {
                equipmentNeedingRemoval.Add(UnequipSlot(WornEquipmentSlot.Armor));
            }
            if (mWornEquipment[(int)WornEquipmentSlot.LeftHeld] != null &&
                !rEquipmentProficiencies.CanEquip(mWornEquipment[(int)WornEquipmentSlot.LeftHeld]))
            {
                equipmentNeedingRemoval.Add(UnequipSlot(WornEquipmentSlot.LeftHeld));
            }
            if (mWornEquipment[(int)WornEquipmentSlot.RightHeld] != null &&
                !rEquipmentProficiencies.CanEquip(mWornEquipment[(int)WornEquipmentSlot.RightHeld]))
            {
                equipmentNeedingRemoval.Add(UnequipSlot(WornEquipmentSlot.RightHeld));
            }
            return equipmentNeedingRemoval;
        }

        public List<EquipmentBase> EquipInSlot(EquipmentBase equipment, WornEquipmentSlot slot)
        {
            List<EquipmentBase> toReturn = new List<EquipmentBase>();
            if (slot == WornEquipmentSlot.LeftHeld || slot == WornEquipmentSlot.RightHeld)
            {
                return EquipHeldEquipment(slot, equipment as HeldEquipment);
            }
            else if (slot == WornEquipmentSlot.Armor)
            {
                return EquipArmor(equipment as ArmorBase);
            }
            //else if (slot == WornEquipmentSlot.Accessory)
            //{
            //    UnityEngine.Debug.Log("Cannot equip Accessories yet!");
            //}
            return new List<EquipmentBase>();
        }

        private List<EquipmentBase> EquipHeldEquipment(WornEquipmentSlot hand, HeldEquipment heldEquipment)
        {
            List<EquipmentBase> itemsUnequipped = new List<EquipmentBase>();
            WornEquipmentSlot otherHand = hand == WornEquipmentSlot.LeftHeld ? WornEquipmentSlot.RightHeld : WornEquipmentSlot.LeftHeld;
            
            // Unequip current hand if something already equiped
            if (mWornEquipment[(int)hand] != null)
            {
                itemsUnequipped.Add(UnEquipHeld(hand));
            }
            mWornEquipment[(int)hand] = heldEquipment;

            // Check off hand for compatability
            if (heldEquipment.NumHandsRequired == 2) // unequip other hand
            {
                if (mWornEquipment[(int)otherHand] != null)
                {
                    itemsUnequipped.Add(UnEquipHeld(otherHand));
                }
            }
            else if (heldEquipment.Index.Key == EquipmentCategory.Shield) 
            {
                // make sure other hand doesn't have a shield
                if (mWornEquipment[(int)otherHand] != null &&
                   mWornEquipment[(int)otherHand].Index.Key == EquipmentCategory.Shield)
                {
                    itemsUnequipped.Add(UnEquipHeld(otherHand));
                }
            } 
            else if (heldEquipment.Index.Key == EquipmentCategory.Weapon)
            {
                // if not dual wielding unequip other hand
                if (mWornEquipment[(int)otherHand] != null &&
                    mWornEquipment[(int)otherHand].Index.Key == EquipmentCategory.Weapon
                    && !rEquipmentProficiencies.HasDualWieldProficiency)
                {
                    itemsUnequipped.Add(UnEquipHeld(otherHand));
                }
            }

            // Reapply effects
            foreach (StatusEffect effect in heldEquipment.EquipmentEffects)
            {
                rStatusManager.ApplyStatusEffect(effect);
            }

            return itemsUnequipped;
        }

        private List<EquipmentBase> EquipArmor(ArmorBase armor)
        {
            List<EquipmentBase> unequippedArmor = new List<EquipmentBase>();
            if (mWornEquipment[(int)WornEquipmentSlot.Armor] != null)
            {
                // Add to inventory
                unequippedArmor.Add(UnEquipArmor());
            }
            mWornEquipment[(int)WornEquipmentSlot.Armor] = armor;
            foreach (StatusEffect effect in armor.EquipmentEffects)
            {
                rStatusManager.ApplyStatusEffect(effect);
            }
            return unequippedArmor;
        }

        public EquipmentBase UnequipSlot(WornEquipmentSlot slot)
        {
            if (slot == WornEquipmentSlot.Armor)
            {
                return UnEquipArmor();
            }
            else if (slot == WornEquipmentSlot.LeftHeld || slot == WornEquipmentSlot.RightHeld)
            {
                return UnEquipHeld(slot);
            }

            return null;
        }

        private EquipmentBase UnEquipArmor()
        {
            if (mWornEquipment[(int)WornEquipmentSlot.Armor] != null)
            {
                // Add to inventory
                ArmorBase armor = mWornEquipment[(int)WornEquipmentSlot.Armor] as ArmorBase;
                foreach (StatusEffect armorEffect in armor.EquipmentEffects)
                {
                    rStatusManager.RemoveStatusEffect(armorEffect.Index);
                }
                mWornEquipment[(int)WornEquipmentSlot.Armor] = null;
                return armor;
            }
            else
            {
                UnityEngine.Debug.Log("Attempting to unequip armor when nothing equipped!");
                return null;
            }
            
        }

        private EquipmentBase UnEquipHeld(WornEquipmentSlot hand)
        {
            if (hand == WornEquipmentSlot.LeftHeld || hand == WornEquipmentSlot.RightHeld)
            {
                if (mWornEquipment[(int)hand] != null)
                {
                    // Add to inventory
                    EquipmentBase held = mWornEquipment[(int)hand];
                    foreach (StatusEffect heldEffect in held.EquipmentEffects)
                    {
                        rStatusManager.RemoveStatusEffect(heldEffect.Index);
                    }
                    mWornEquipment[(int)hand] = new WeaponBase(WeaponType.Unarmed);
                    return held;
                }
                else
                {
                    UnityEngine.Debug.LogError("Attempting to UnEquip held equipment from empty slot");
                }
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("Attempting to Equip weapon into invalid slot. Got[{0}]", hand.ToString());
            }
            return null;
        }

        public List<string> GetHeldEquipmentModelIds()
        {
            List<string> heldIds = new List<string>();
            heldIds.Add(!EquipmentUtil.IsEmptySlot(mWornEquipment[(int)WornEquipmentSlot.LeftHeld]) ? "WeaponModels/" + ((HeldEquipment)mWornEquipment[(int)WornEquipmentSlot.LeftHeld]).ModelId : "");
            heldIds.Add(!EquipmentUtil.IsEmptySlot(mWornEquipment[(int)WornEquipmentSlot.RightHeld]) ? "WeaponModels/" + ((HeldEquipment)mWornEquipment[(int)WornEquipmentSlot.RightHeld]).ModelId : "");

            return heldIds;
        }

        public List<WeaponBase> GetHeldWeapons()
        {
            List<WeaponBase> heldIds = new List<WeaponBase>();
            heldIds.Add(mWornEquipment[(int)WornEquipmentSlot.LeftHeld] != null && mWornEquipment[(int)WornEquipmentSlot.LeftHeld].Index.Key == EquipmentCategory.Weapon ? 
                mWornEquipment[(int)WornEquipmentSlot.LeftHeld] as WeaponBase : null);
            heldIds.Add(mWornEquipment[(int)WornEquipmentSlot.RightHeld] != null && mWornEquipment[(int)WornEquipmentSlot.RightHeld].Index.Key == EquipmentCategory.Weapon ?
               mWornEquipment[(int)WornEquipmentSlot.RightHeld] as WeaponBase : null);
            return heldIds;
        }

        public bool IsEquipped(EquipmentCategory type, int equipmentIndex)
        {
            bool isEquipped = false;
            switch (type)
            {
                case (EquipmentCategory.Armor):
                    isEquipped = (int)(mWornEquipment[(int)WornEquipmentSlot.Armor] as ArmorBase).ArmorType == equipmentIndex;
                    break;
                case (EquipmentCategory.Weapon):
                    for (int hand = (int)WornEquipmentSlot.LeftHeld; hand <= (int)WornEquipmentSlot.RightHeld; hand++)
                    {
                        if (mWornEquipment[hand] != null && mWornEquipment[hand].Index.Key == type)
                        {
                            if (equipmentIndex == -1)
                            {
                                isEquipped |= true;
                            }
                            else if (equipmentIndex == (int)WeaponType.MeleeWeapon)
                            {
                                isEquipped |= EquipmentUtil.IsMeleeWeapon((mWornEquipment[hand] as WeaponBase).WeaponType);
                            }
                            else if (equipmentIndex == (int)WeaponType.RangedWeapon)
                            {
                                isEquipped |= EquipmentUtil.IsRangedWeapon((mWornEquipment[hand] as WeaponBase).WeaponType);
                            }
                            else
                            {
                                isEquipped |= (int)(mWornEquipment[hand] as WeaponBase).WeaponType == equipmentIndex;
                            }
                        } 
                    }
                    break;
                case (EquipmentCategory.Shield):
                    for (int hand = (int)WornEquipmentSlot.LeftHeld; hand <= (int)WornEquipmentSlot.RightHeld; hand++)
                    {
                        isEquipped |= mWornEquipment[hand] != null
                            && mWornEquipment[hand].Index.Key == type
                            && (equipmentIndex == -1 || (int)(mWornEquipment[hand] as ShieldBase).ShieldType == equipmentIndex);
                    }
                    break;
            }
            return isEquipped;
        }

        public WornEquipmentPayload[] GetWornEquipmentPayloads()
        {
            WornEquipmentPayload[] wornPayloads = new WornEquipmentPayload[(int)WornEquipmentSlot.NUM];

            for (int i = 0; i < mWornEquipment.Length; ++i)
            {
                string assetPath = "";
                if (mWornEquipment[i] != null && mWornEquipment[i].WornAssetId != "")
                {
                    assetPath = "EquipmentImages/" + mWornEquipment[i].WornAssetId;
                }
                else
                {
                    assetPath = "empty";
                }
                wornPayloads[i] = new WornEquipmentPayload()
                {
                    ImageAssetPath = assetPath
                };
            }

            return wornPayloads;
        }
    }
}
