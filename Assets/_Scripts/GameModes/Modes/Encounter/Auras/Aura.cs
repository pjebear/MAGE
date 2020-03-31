using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Aura : MonoBehaviour 
{
    public Collider TriggerVolume;

    private Dictionary<EncounterActorController, StatusEffect> mInAura = new Dictionary<EncounterActorController, StatusEffect>();
    private Vector3 UnitRangeAuraScale;

    private AuraInfo mAuraInfo;
    private EncounterActorController mOwner;

    private void Awake()
    {
        UnitRangeAuraScale = Vector3.one;

        SetActive(false);
    }

    public void Initialize(AuraInfo info, EncounterActorController owner)
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
                actorStatusPair.Key.EncounterCharacter.OnAuraExited(actorStatusPair.Value);
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
        EncounterActorController entered = other.gameObject.GetComponentInParent<EncounterActorController>();
        if (entered != null && entered != mOwner)
        {
            OnAuraEntered(entered);
        }
    }

    private void OnAuraEntered(EncounterActorController controller)
    {
        if (!mInAura.ContainsKey(controller))
        {
            if ((controller.EncounterCharacter.Team != mOwner.EncounterCharacter.Team) ^ mAuraInfo.IsBeneficial)
            {
                StatusEffect auraEffect = StatusEffectFactory.CheckoutStatusEffect(mAuraInfo.AuraEffectType, mOwner.EncounterCharacter);
                mInAura.Add(controller, auraEffect);
                controller.DisplayStatusApplication(auraEffect);
                controller.EncounterCharacter.OnAuraEntered(auraEffect);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EncounterActorController exited = other.gameObject.GetComponentInParent<EncounterActorController>();
        if (exited != null && exited != mOwner)
        {
            OnAuraExited(exited);
        }
    }

    private void OnAuraExited(EncounterActorController controller)
    {
        if (mInAura.ContainsKey(controller))
        {
            if ((controller.EncounterCharacter.Team != mOwner.EncounterCharacter.Team) ^ mAuraInfo.IsBeneficial)
            {
                StatusEffect auraEffect = mInAura[controller];
                mInAura.Remove(controller);
                controller.DisplayStatusRemoval(auraEffect);
                controller.EncounterCharacter.OnAuraExited(auraEffect);
            }
        }
    }
}

