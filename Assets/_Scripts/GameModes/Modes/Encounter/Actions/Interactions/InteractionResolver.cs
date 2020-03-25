using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class InteractionResolver
{
    public static List<InteractionResult> ResolveInteraction(EncounterCharacter caster, ActionInfo info, List<EncounterCharacter> targets)
    {
        List<InteractionResult> results = new List<InteractionResult>();

        foreach (EncounterCharacter target in targets)
        {
            StateChange stateChange = StateChange.Empty;

            InteractionResultType interactionResultType = InteractionResultType.NUM;

            if (UnityEngine.Random.Range(0,100) < target.Attributes[AttributeCategory.Stat][(int)TertiaryStat.FrontalParry].Value)
            {
                interactionResultType = InteractionResultType.Parry;
            }
            else if (UnityEngine.Random.Range(0, 100) < target.Attributes[AttributeCategory.Stat][(int)TertiaryStat.FrontalBlock].Value)
            {
                interactionResultType = InteractionResultType.Block;
            }
            else if (UnityEngine.Random.Range(0, 100) < target.Attributes[AttributeCategory.Stat][(int)TertiaryStat.Dodge].Value)
            {
                interactionResultType = InteractionResultType.Dodge;
            }
            else
            {
                stateChange = info.GetStateChange(caster, target);

                if (info.ActionMedium == ActionMedium.Physical)
                {
                    float physicalReductionMultiplier = 1 - target.Attributes[AttributeCategory.Stat][(int)TertiaryStat.PhysicalResistance].Value;
                    float modifiedHealthChange = stateChange.healthChange * physicalReductionMultiplier;
                    stateChange.healthChange = (int)modifiedHealthChange;
                }
                
                interactionResultType = InteractionResultType.Hit;
            }

            results.Add(new InteractionResult(interactionResultType, stateChange));
        }

        return results;
    }

    // TODO: Spells
}

