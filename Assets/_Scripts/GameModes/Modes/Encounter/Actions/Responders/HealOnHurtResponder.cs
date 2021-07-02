using MAGE.GameModes.Encounter;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Combat
{
    class HealOnHurtResponder : ActionResponderBase
    {
        public HealOnHurtResponder(CombatEntity responder) : base(responder)
        {
            PercentChance = 25;
        }

        protected override List<ActionResponseBase> GetResponsesToResult(ActionResult actionResult)
        {
            List<ActionResponseBase> responses = new List<ActionResponseBase>();

            foreach (var targetResultPair in actionResult.TargetResults)
            {
                CombatTarget target = targetResultPair.Key;
                CombatEntity targetEntity = target.GetComponent<CombatEntity>();
                foreach (InteractionResult result in targetResultPair.Value)
                {
                    if (!IsResponder(targetEntity) // responder wasn't the one who got hurt
                    && IsAlly(targetEntity) // don't heal enemies
                    && WasHurt(result)
                    && IsAlive(targetEntity)
                    && InRange(target.transform, Range))
                    {
                        ActionComposerBase actionComposerBase = ActionComposerFactory.CheckoutAction(mResponder, ActionId.SpotHeal);
                        actionComposerBase.ActionInfo.EffectRange.AreaType = AreaType.Point;
                        actionComposerBase.ActionInfo.Effectiveness *= .5f;

                        ActionProposal healProposal = new ActionProposal(
                            mResponder
                            , new Target(target)
                            , actionComposerBase);

                        responses.Add(new ActionProposalResponse(healProposal));
                        break;
                    }
                }
            }
            
            return responses;
        }
    }
}


