using MAGE.GameModes.Combat;
using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Actions
{
    static class InteractionResolver
    {
        public static InteractionResult GetWeaponInteractionResult(CombatEntity attacker, CombatTarget target, StateChange weaponStateChange)
        {
            StateChange stateChange = StateChange.Empty;
            
            InteractionResultType interactionResultType = InteractionResultType.NUM;

            RelativeOrientation relativeOrientation = InteractionUtil.GetRelativeOrientation(attacker.transform, target.transform);

            float blockChance = 0;
            float dodgeChance = 0;
            float parryChance = 0;

            InteractionUtil.GetAvoidanceAttributesForEntity(target.GetComponent<CombatEntity>(), out dodgeChance, out blockChance, out parryChance, relativeOrientation);

            int diceRoll = UnityEngine.Random.Range(0, 100);
            if (diceRoll < parryChance)
            {
                interactionResultType = InteractionResultType.Parry;
            }
            else if (diceRoll < parryChance + blockChance)
            {
                interactionResultType = InteractionResultType.Block;
            }
            else if (diceRoll < parryChance + blockChance + dodgeChance)
            {
                interactionResultType = InteractionResultType.Dodge;
            }
            else
            {
                interactionResultType = InteractionResultType.Hit;

                stateChange = weaponStateChange.Copy();
                float physicalReductionMultiplier = 1;
                if (target.GetComponent<StatsControl>() != null)
                {
                    physicalReductionMultiplier = 1 - target.GetComponent<StatsControl>().Attributes[TertiaryStat.PhysicalResistance];
                    if (physicalReductionMultiplier < 0)
                    {
                        Debug.LogWarning("Target has greater than 100 percent physical reduction. defaulting to 90 percent");
                        physicalReductionMultiplier = .1f;
                    }
                    
                }

                stateChange.healthChange = (int)(stateChange.healthChange * physicalReductionMultiplier);
            }

            return new InteractionResult(interactionResultType, stateChange);
        }
    }


}
