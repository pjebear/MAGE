using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Common.StatusTypes;
namespace EncounterSystem.StatusEffects
{
    using Character;
    class Aura : MonoBehaviour
    {
        private StatusEffect toApply;
        private CharacterManager mOwner;
        private bool mIsBeneficial;
        private List<CharacterManager> mInAura;
        public void Initialize(CharacterManager owner, StatusEffect effect, float radius, bool isBeneficial)
        {
            mOwner = owner;
            toApply = effect;
            mIsBeneficial = isBeneficial;
            // Apply to self if beneficial
            if (mIsBeneficial)
            {
                mOwner.AttemptStatusApplication(effect);
            }
            gameObject.GetComponent<SphereCollider>().radius = radius;
            mInAura = new List<CharacterManager>();
        }

        public void SetIsActive(bool isActive)
        {
            foreach (CharacterManager character in mInAura)
            {
                if (isActive)
                {
                    character.AttemptStatusApplication(toApply);
                }
                else
                {
                    character.AttemptRemoveStatusEffect(toApply.Index, 1);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CharacterManager target = other.GetComponent<CharacterManager>();
                if (target == mOwner)
                {
                    return;
                }
                if (!(!(target.IsPlayerControlled ^ mOwner.IsPlayerControlled) ^ mIsBeneficial))
                {
                    target.AttemptStatusApplication(toApply);
                    mInAura.Add(target);
                }
            }
           
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CharacterManager target = other.GetComponent<CharacterManager>();
                if (target == mOwner)
                {
                    return;
                }
                if (!(!(target.IsPlayerControlled ^ mOwner.IsPlayerControlled) ^ mIsBeneficial))
                {
                    target.AttemptRemoveStatusEffect(toApply.Index, 1);
                    mInAura.Remove(target);
                }
            }
        }
    }
}
