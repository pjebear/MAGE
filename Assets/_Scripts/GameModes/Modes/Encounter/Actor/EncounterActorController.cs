using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class EncounterActorController : MonoBehaviour
{
    private float mBillboardDuration = 1f;
    private float mBillboardHeight = 1.2f;
    public BillboardEmitter BillboardEmitter;
    public Actor Actor;
    public EncounterCharacter EncounterCharacter;

    private void Awake()
    {

    }

    public void DisplayStateChange(StateChange stateChange)
    {
        if (stateChange.Type == StateChangeType.ActionTarget || stateChange.Type == StateChangeType.StatusEffect)
        {
            if (stateChange.healthChange != 0)
            {
                Billboard.Params data = new Billboard.Params(stateChange.healthChange.ToString(), transform, Vector3.up * mBillboardHeight);
                BillboardEmitter.Emitt(data, mBillboardDuration);
            }
            if (stateChange.resourceChange != 0)
            {
                Billboard.Params data = new Billboard.Params(stateChange.resourceChange.ToString(), transform, Vector3.up * mBillboardHeight);
                BillboardEmitter.Emitt(data, mBillboardDuration);
            }
            foreach(StatusEffect effect in stateChange.statusEffects)
            {
                DisplayStatusApplication(effect);
            }
        }
    }

    public void DisplayStatusApplication(StatusEffect effect)
    {
        Billboard.Params data = new Billboard.Params("+" + effect.ToString().ToString(), transform, Vector3.up * mBillboardHeight);
        BillboardEmitter.Emitt(data, mBillboardDuration);
    }

    public void DisplayStatusRemoval(StatusEffect statusEffect)
    {
        Billboard.Params data = new Billboard.Params("-" + statusEffect.ToString().ToString(), transform, Vector3.up * mBillboardHeight);
        BillboardEmitter.Emitt(data, mBillboardDuration);
    }
}

