using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Aura : MonoBehaviour 
{
    public Collider TriggerVolume;

    private Dictionary<ActorController, StatusEffect> mInAura = new Dictionary<ActorController, StatusEffect>();
    private Vector3 UnitRangeAuraScale;

    private AuraInfo mAuraInfo;
    private ActorController mOwner;

    private void Awake()
    {
        UnitRangeAuraScale = Vector3.one;

        SetActive(false);
    }

    public void Initialize(AuraInfo info, ActorController owner)
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
                actorStatusPair.Key.mActor.OnAuraExited(actorStatusPair.Value);
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
        ActorController entered = other.gameObject.GetComponentInParent<ActorController>();
        if (entered != null && entered != mOwner)
        {
            OnAuraEntered(entered);
        }
    }

    private void OnAuraEntered(ActorController controller)
    {
        if (!mInAura.ContainsKey(controller))
        {
            if ((controller.mActor.Team != mOwner.mActor.Team) ^ mAuraInfo.IsBeneficial)
            {
                StatusEffect auraEffect = StatusEffectFactory.CheckoutStatusEffect(mAuraInfo.AuraEffectType, mOwner.mActor);
                mInAura.Add(controller, auraEffect);
                controller.DisplayStatusApplication(auraEffect);
                controller.mActor.OnAuraEntered(auraEffect);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ActorController exited = other.gameObject.GetComponentInParent<ActorController>();
        if (exited != null && exited != mOwner)
        {
            OnAuraExited(exited);
        }
    }

    private void OnAuraExited(ActorController controller)
    {
        if (mInAura.ContainsKey(controller))
        {
            if ((controller.mActor.Team != mOwner.mActor.Team) ^ mAuraInfo.IsBeneficial)
            {
                StatusEffect auraEffect = mInAura[controller];
                mInAura.Remove(controller);
                controller.DisplayStatusRemoval(auraEffect);
                controller.mActor.OnAuraExited(auraEffect);
            }
        }
    }
}

