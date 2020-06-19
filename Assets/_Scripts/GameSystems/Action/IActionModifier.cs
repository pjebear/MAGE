using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


interface IActionModifier : IModifier<ActionInfo>
{
    ActionId ActionId { get; }
}

class SwordActionModifier : IActionModifier
{
    public ActionId ActionId { get { return ActionId.SwordAttack; } }

    public void Modify(ActionInfo info)
    {
        WeaponActionInfoBase meleeAttackInfo = (WeaponActionInfoBase)info;

        meleeAttackInfo.DamageAmp += .5f;
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

    public void Modify(ActionInfo info)
    {
        HealInfo healInfo = (HealInfo)info;

        healInfo.HealAmp += HealAmp;
    }
}
