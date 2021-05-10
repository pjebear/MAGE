using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Actions
{
    interface IActionModifier : IModifier<ActionInfoBase>
    {
        ActionId ActionId { get; }
    }

    class SwordActionModifier : IActionModifier
    {
        public ActionId ActionId { get { return ActionId.WeaponAttack; } }

        public void Modify(ActionInfoBase info)
        {
            WeaponActionInfoBase meleeAttackInfo = (WeaponActionInfoBase)info;

            meleeAttackInfo.Effectiveness += .5f;
        }
    }

    class HealModifier : IActionModifier
    {
        public ActionId ActionId { get { return ActionId.Heal; } }
        public float HealAmp;

        public HealModifier(float healAmp)
        {
            HealAmp = healAmp;
        }

        public void Modify(ActionInfoBase info)
        {
            info.Effectiveness += HealAmp;
        }
    }

}
