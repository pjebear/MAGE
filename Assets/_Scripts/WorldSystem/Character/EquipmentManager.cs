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

namespace WorldSystem.Character
{
    class EquipmentManager
    {
        protected EquipmentProficienciesContainer rEquipmentProficiencies;
        protected ArmorBase mArmor;
        protected StatusManager rStatusManager;
        HeldEquipment[] mHeldEquipment;

        public EquipmentManager()
        {
            mHeldEquipment = new HeldEquipment[2];
            mHeldEquipment[0] = new WeaponBase(WeaponType.Unarmed);
            mHeldEquipment[1] = new WeaponBase(WeaponType.Unarmed);
            rEquipmentProficiencies = null;
        }

        public void Initialize(StatusManager statusManager)
        {
            rStatusManager = statusManager;
        }

        public void SetProficiencies(EquipmentProficienciesContainer proficiencies)
        {
            rEquipmentProficiencies = proficiencies;
            // unequip anything that is no longer in proficiencies
            if (mArmor != null &&
                !rEquipmentProficiencies.CanEquip(mArmor))
            {
                UnEquipArmor();
            }
            if (mHeldEquipment[0] != null &&
                !rEquipmentProficiencies.CanEquip(mHeldEquipment[0]))
            {
                UnEquipHeld(0);
            }
            if (mHeldEquipment[1] != null &&
                !rEquipmentProficiencies.CanEquip(mHeldEquipment[1]))
            {
                UnEquipHeld(1);
            }

        }

        // UNEQUIP MUST HAPPEN BEFORE EQUIP
        public void EquipHeldEquipment(int hand, HeldEquipment heldEquipment)
        {
            int otherHand = (hand + 1) % 2;
            if (hand >= 0 && hand < 2)
            {
                // Unequip current hand if something already equiped
                if (mHeldEquipment[hand] != null)
                {
                    UnEquipHeld(hand);
                }
                mHeldEquipment[hand] = heldEquipment;

                // Check off hand for compatability
                if (heldEquipment.NumHandsRequired == 2) // unequip other hand
                {
                    UnityEngine.Debug.Assert(hand == 0, "Attempting to equip two handed weapon in off hand");
                    if (mHeldEquipment[1] != null)
                    {
                        UnEquipHeld(1);
                    }
                }
                else if (heldEquipment.EquipmentType == EquipmentType.Shield) 
                {
                    // make sure other hand doesn't have a shield
                    if (mHeldEquipment[otherHand] != null &&
                        mHeldEquipment[otherHand].EquipmentType == EquipmentType.Shield)
                    {
                        UnEquipHeld(otherHand);
                    }
                } 
                else if (heldEquipment.EquipmentType == EquipmentType.Weapon)
                {
                    // if not dual wielding unequip other hand
                    if (mHeldEquipment[otherHand] != null &&
                        mHeldEquipment[otherHand].EquipmentType == EquipmentType.Weapon
                        && !rEquipmentProficiencies.HasDualWieldProficiency)
                    {
                        UnEquipHeld(otherHand);
                    }
                }

                // Reapply effects
                foreach (StatusEffect effect in heldEquipment.EquipmentEffects)
                {
                    rStatusManager.ApplyStatusEffect(effect);
                }
                
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("Attempting to Equip weapon into invalid slot. Got[{0}]", hand);
            }
        }
        // UNEQUIP MUST HAPPEN BEFORE EQUIP
        public void EquipArmor(ArmorBase armor)
        {
            if (mArmor != null)
            {
                // Add to inventory
                UnEquipArmor();
            }
            mArmor = armor;
            foreach (StatusEffect effect in armor.EquipmentEffects)
            {
                rStatusManager.ApplyStatusEffect(effect);
            }

        }

        public ArmorBase UnEquipArmor()
        {
            if (mArmor != null)
            {
                // Add to inventory
                ArmorBase armor = mArmor;
                foreach (StatusEffect armorEffect in armor.EquipmentEffects)
                {
                    rStatusManager.RemoveStatusEffect(armorEffect.Index);
                }
                mArmor = null;
                return armor;
            }
            else
            {
                UnityEngine.Debug.Log("Attempting to unequip armor when nothing equipped!");
                return null;
            }
            
        }

        public HeldEquipment UnEquipHeld(int hand)
        {
            if (hand >= 0 && hand < 2)
            {
                if (mHeldEquipment[hand] != null)
                {
                    // Add to inventory
                    HeldEquipment held = mHeldEquipment[hand];
                    foreach (StatusEffect heldEffect in held.EquipmentEffects)
                    {
                        rStatusManager.RemoveStatusEffect(heldEffect.Index);
                    }
                    mHeldEquipment[hand] = new WeaponBase(WeaponType.Unarmed);
                    return held;
                }
                else
                {
                    UnityEngine.Debug.LogError("Attempting to UnEquip held equipment from empty slot");
                }
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("Attempting to Equip weapon into invalid slot. Got[{0}]", hand);
            }
            return null;
        }

        public List<string> GetHeldEquipmentModels()
        {
            List<string> heldIds = new List<string>();
            for (int i = 0; i < 2; i++)
            {
                if (mHeldEquipment[i] != null)
                {
                    heldIds.Add(mHeldEquipment[i].ModelId);
                }
            }
            return heldIds;
        }

        public List<WeaponBase> GetHeldWeapons()
        {
            List<WeaponBase> heldIds = new List<WeaponBase>();
            for (int i = 0; i < 2; i++)
            {
                if (mHeldEquipment[i] != null && mHeldEquipment[i].EquipmentType == EquipmentType.Weapon)
                {
                    heldIds.Add(mHeldEquipment[i] as WeaponBase);
                }
                else
                {
                    heldIds.Add(null);
                }
            }
            return heldIds;
        }

        public bool IsEquipped(EquipmentType type, int equipmentIndex)
        {
            bool isEquipped = false;
            switch (type)
            {
                case (EquipmentType.Armor):
                    isEquipped = (int)mArmor.ArmorType == equipmentIndex;
                    break;
                case (EquipmentType.Weapon):
                    for (int i = 0; i < 2; i++)
                    {
                        if (mHeldEquipment[i] != null && mHeldEquipment[i].EquipmentType == type)
                        {
                            if (equipmentIndex == -1)
                            {
                                isEquipped |= true;
                            }
                            else if (equipmentIndex == (int)WeaponType.MeleeWeapon)
                            {
                                isEquipped |= EquipmentUtil.IsMeleeWeapon((mHeldEquipment[i] as WeaponBase).WeaponType);
                            }
                            else if (equipmentIndex == (int)WeaponType.RangedWeapon)
                            {
                                isEquipped |= EquipmentUtil.IsRangedWeapon((mHeldEquipment[i] as WeaponBase).WeaponType);
                            }
                            else
                            {
                                isEquipped |= (int)(mHeldEquipment[i] as WeaponBase).WeaponType == equipmentIndex;
                            }
                        } 
                    }
                    break;
                case (EquipmentType.Shield):
                    for (int i = 0; i < 2; i++)
                    {
                        isEquipped |= mHeldEquipment[i] != null
                            && mHeldEquipment[i].EquipmentType == type
                            && (equipmentIndex == -1 || (int)(mHeldEquipment[i] as ShieldBase).ShieldType == equipmentIndex);
                    }
                    break;
            }
            return isEquipped;
        }
    }
}
