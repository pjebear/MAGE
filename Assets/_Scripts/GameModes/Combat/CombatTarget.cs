using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Combat
{
    [RequireComponent(typeof(CombatDisplayable))]
    [RequireComponent(typeof(ResourcesControl))]
    class CombatTarget : MonoBehaviour
    {
        public void ApplyStateChange(StateChange stateChange)
        {
            // StatusEffects first
            if (GetComponent<StatusEffectControl>() != null)
            {
                if (stateChange.Type == StateChangeType.ActionCost)
                {
                    GetComponent<StatusEffectControl>().RemoveStatusEffects(stateChange.statusEffects);
                }
                else
                {
                    GetComponent<StatusEffectControl>().ApplyStatusEffects(stateChange.statusEffects);
                }
            }

            GetComponent<ResourcesControl>().ApplyStateChange(stateChange);
        }

        public void ApplyInteractionResult(InteractionResult result)
        {

            string resultString = "";
            switch (result.InteractionResultType)
            {
                case InteractionResultType.Block: resultString = "Block"; break;
                case InteractionResultType.Parry: resultString = "Parry"; break;
                case InteractionResultType.Dodge: resultString = "Dodge"; break;
                case InteractionResultType.Resist: resultString = "Resist"; break;
            }

            if (resultString != "")
            {
                Billboard.Params param = new Billboard.Params();
                param.anchor = transform;
                param.offset = new Vector3(1f, 2f, 0);
                param.text = resultString;
                GetComponent<BillboardEmitter>().Emitt(param, 2f);
            }

            ApplyStateChange(result.StateChange);
        }
    }
}
