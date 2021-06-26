using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class SpotHealComposer : HealComposer
    {
        public SpotHealComposer(CombatEntity owner) : base (owner)
        {
            
        }

        protected override ActionInfo PopulateActionInfo()
        {
            ActionInfo actionInfo = base.PopulateActionInfo();

            actionInfo.Effectiveness *= .5f;
            
            actionInfo.ActionCost = new StateChange(StateChangeType.ActionCost, 0, 0);
            
            actionInfo.EffectInfo.EffectId = EffectType.AOE_Heal;

            actionInfo.EffectRange = new RangeInfo()
            {
                AreaType = AreaType.Point,
                TargetingType = TargetingType.Allies
            };

            return actionInfo;
        }
    }
}
