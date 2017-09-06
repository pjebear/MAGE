using Common.ActionEnums;
using Common.StatusEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Character;

namespace WorldSystem.Talents
{
    public enum TalentIndex
    {
        // Footman
        Footman_Status_FrontalBlockIncrease,
        Footman_Status_SpeedIncrease,
        Footman_Aura_PhysicalResistanceIncrease,
        Footman_Action_ShieldBash,
        Footman_Action_Defend,

        // Shieldwall
        ShieldWall_Status_MovementIncrease,
        Shieldwall_Status_PeripheralBlockChance,
        Shieldwall_Action_Advance,
        // Duelist

        // Berserker
        Berserker_Status_PhysicalDamagePercentIncrease,
    }
    namespace TalentFactory
    {
        class TalentFactory
        {
            public static TalentBase CheckoutTalent(TalentIndex index, CharacterBase character)
            {
                TalentBase toCheckOut = null;

                switch (index)
                {
                    case (TalentIndex.Footman_Status_FrontalBlockIncrease):
                        toCheckOut = new StatusTalentBase(StatusEffectIndex.FOOTMAN_FRONTAL_BLOCK, character, index, 3);
                        break;
                    case (TalentIndex.Footman_Status_SpeedIncrease):
                        toCheckOut = new StatusTalentBase(StatusEffectIndex.FOOTMAN_SPEED_INCREASE, character, index, 3);
                        break;
                    case (TalentIndex.Footman_Action_Defend):
                        toCheckOut = new ActionTalentBase(ActionIndex.DEFEND, character, index);
                        break;
                    case (TalentIndex.Footman_Action_ShieldBash):
                        toCheckOut = new ActionTalentBase(ActionIndex.SHIELD_BASH, character, index);
                        break;
                    case (TalentIndex.Footman_Aura_PhysicalResistanceIncrease):
                        toCheckOut = new AuraTalentBase(AuraIndex.FOOTMAN_PHYSICAL_DAMAGE_RESISTANCE, character, index);
                        break;
                    default:
                        UnityEngine.Debug.LogError("No talent defined for " + index.ToString() + " in talent factory");
                        break;
                }

                return toCheckOut;
            }
        }
    }
   
}
