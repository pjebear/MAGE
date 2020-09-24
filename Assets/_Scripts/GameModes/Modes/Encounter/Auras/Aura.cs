using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class Aura : MonoBehaviour
    {
        public Collider TriggerVolume;

        private Dictionary<CharacterActorController, StatusEffect> mInAura = new Dictionary<CharacterActorController, StatusEffect>();
        private Vector3 UnitRangeAuraScale;

        private AuraInfo mAuraInfo;
        private CharacterActorController mOwner;

        private void Awake()
        {
            UnitRangeAuraScale = Vector3.one;

            SetActive(false);
        }

        public void Initialize(AuraInfo info, CharacterActorController owner)
        {
            mAuraInfo = info;
            mOwner = owner;

            TriggerVolume.transform.localScale = UnitRangeAuraScale * (1 + info.Range);

        }

        public void SetActive(bool active)
        {
            if (active == gameObject.activeSelf) return;


            if (!active)
            {
                foreach (var actorStatusPair in mInAura)
                {
                    EncounterFlowControl.CharacterDirector.RemoveAura(actorStatusPair.Key.Character, actorStatusPair.Value);
                }

                mInAura.Clear();
            }
            else
            {
                OnAuraEntered(mOwner);
            }

            gameObject.SetActive(active);
        }

        private void OnTriggerEnter(Collider other)
        {
            CharacterActorController entered = other.gameObject.GetComponentInParent<CharacterActorController>();
            if (entered != null && entered != mOwner)
            {
                OnAuraEntered(entered);
            }
        }

        private void OnAuraEntered(CharacterActorController controller)
        {
            if (!mInAura.ContainsKey(controller))
            {
                if ((controller.Character.TeamSide != mOwner.Character.TeamSide) ^ mAuraInfo.IsBeneficial)
                {
                    StatusEffect auraEffect = StatusEffectFactory.CheckoutStatusEffect(mAuraInfo.AuraEffectType, mOwner.Character);
                    mInAura.Add(controller, auraEffect);
                    EncounterFlowControl.CharacterDirector.ApplyAura(controller.Character, auraEffect);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            CharacterActorController exited = other.gameObject.GetComponentInParent<CharacterActorController>();
            if (exited != null && exited != mOwner)
            {
                OnAuraExited(exited);
            }
        }

        private void OnAuraExited(CharacterActorController controller)
        {
            if (mInAura.ContainsKey(controller))
            { 
                EncounterFlowControl.CharacterDirector.RemoveAura(controller.Character, mInAura[controller]);
                mInAura.Remove(controller);
            }
        }
    }

}

