using MAGE.GameModes.SceneElements.Encounters;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Combat
{
    class Aura : MonoBehaviour
    {
        public CapsuleCollider TriggerVolume;
        public Transform AuraGraphic;

        private Dictionary<StatusEffectControl, StatusEffect> mInAura = new Dictionary<StatusEffectControl, StatusEffect>();
        private Vector3 UnitRangeAuraScale;

        private AuraInfo mAuraInfo;
        private CombatEntity mOwner;

        private void Awake()
        {
            UnitRangeAuraScale = Vector3.one;
        }

        public void Initialize(AuraInfo info, CombatEntity owner)
        {
            mAuraInfo = info;
            mOwner = owner;

            TriggerVolume.radius = info.Range / 2f;
            AuraGraphic.localScale = UnitRangeAuraScale * (1 + info.Range);
        }

        public void SetActive(bool active)
        {
            if (active == gameObject.activeSelf) return;

            if (!active)
            {
                foreach (var statusEffectControlPair in mInAura)
                {
                    statusEffectControlPair.Key.RemoveStatusEffects(new List<StatusEffect>() { statusEffectControlPair.Value }, false);
                }
                mInAura.Clear();
            }
            else
            {
                OnAuraEntered(mOwner.GetComponent<StatusEffectControl>());
            }

            gameObject.SetActive(active);
        }

        private void OnTriggerEnter(Collider other)
        {
            StatusEffectControl entered = other.gameObject.GetComponentInParent<StatusEffectControl>();
            if (entered != null && entered.gameObject != mOwner.gameObject)
            {
                OnAuraEntered(entered);
            }
        }

        private void OnAuraEntered(StatusEffectControl effectControl)
        {
            if (!mInAura.ContainsKey(effectControl))
            {
                if ((effectControl.GetComponent<CombatEntity>().TeamSide != mOwner.TeamSide) ^ mAuraInfo.IsBeneficial)
                {
                    StatusEffect auraEffect = StatusEffectFactory.CheckoutStatusEffect(mAuraInfo.AuraEffectType);
                    mInAura.Add(effectControl, auraEffect);
                    effectControl.ApplyStatusEffects(new List<StatusEffect>() { auraEffect }, false);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            StatusEffectControl exited = other.gameObject.GetComponentInParent<StatusEffectControl>();
            if (exited != null && exited.gameObject != mOwner.gameObject)
            {
                OnAuraExited(exited);
            }
        }

        private void OnAuraExited(StatusEffectControl effectControl)
        {
            if (mInAura.ContainsKey(effectControl))
            {
                effectControl.RemoveStatusEffects(new List<StatusEffect>() { mInAura[effectControl] });
                mInAura.Remove(effectControl);
            }
        }
    }

}

