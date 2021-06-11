using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Stats
{
    static class StatusEffectFactory
    {
        public static StatusEffect CheckoutStatusEffect(StatusEffectId type, int initialStacks = 1)
        {
            StatusEffect effect = null;
            StatusEffectInfo info = new StatusEffectInfo();
            info.Type = type;

            switch (type)
            {
                case StatusEffectId.Avenger:
                    info.Duration = StatusEffectConstants.PERMANENT_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Avenger;
                    info.MaxStackCount = 100;
                    effect = new AvengerEffect();
                    break;

                case StatusEffectId.Daze:
                    info.Duration = StatusEffectConstants.MEDIUM_DURATION;
                    info.Beneficial = false;
                    info.SpriteId = UI.StatusIconSpriteId.Disarm;
                    info.MaxStackCount = 1;
                    effect = new DazeEffect();
                    break;

                case StatusEffectId.Disarm:
                    info.Duration = StatusEffectConstants.MEDIUM_DURATION;
                    info.Beneficial = false;
                    info.SpriteId = UI.StatusIconSpriteId.Disarm;
                    info.MaxStackCount = 1;
                    effect = new DisarmEffect();
                    break;

                case StatusEffectId.Defend:
                    info.Duration = StatusEffectConstants.UNTIL_NEXT_TURN;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Protection;
                    info.MaxStackCount = 1;
                    effect = new DefendEffect();
                    break;

                case StatusEffectId.Hamstring:
                    info.Duration = StatusEffectConstants.MEDIUM_DURATION;
                    info.Beneficial = false;
                    info.SpriteId = UI.StatusIconSpriteId.Disarm;
                    info.MaxStackCount = 1;
                    effect = new HamstringEffect();
                    break;

                case StatusEffectId.DoubleTime:
                    info.Duration = StatusEffectConstants.UNTIL_NEXT_TURN;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Protection;
                    info.MaxStackCount = 1;
                    effect = new DoubleTimeEffect();
                    break;

                case StatusEffectId.Poison:
                    info.Duration = StatusEffectConstants.MEDIUM_DURATION;
                    info.Beneficial = false;
                    info.SpriteId = UI.StatusIconSpriteId.Poison;
                    effect = new PoisonEffect();
                    break;

                case StatusEffectId.ScorchedEarth:
                    info.Duration = StatusEffectConstants.PERMANENT_DURATION;
                    info.Beneficial = false;
                    info.SpriteId = UI.StatusIconSpriteId.ScorchedEarth;
                    effect = new ScorchedEarth();
                    break;

                case StatusEffectId.Protection:
                    info.Duration = StatusEffectConstants.MEDIUM_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Protection;
                    effect = new ProtectionEffect();
                    break;

                case StatusEffectId.Aura_Protection:
                    info.Duration = StatusEffectConstants.PERMANENT_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Protection;
                    effect = new ProtectionEffect();
                    break;

                case StatusEffectId.Regen:
                    info.Duration = StatusEffectConstants.MEDIUM_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Protection;
                    effect = new RegenEffect();
                    break;

                case StatusEffectId.Aura_Regen:
                    info.Duration = StatusEffectConstants.PERMANENT_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Protection;
                    effect = new RegenEffect();
                    break;

                case StatusEffectId.Root:
                    info.Duration = StatusEffectConstants.MEDIUM_DURATION;
                    info.Beneficial = false;
                    info.SpriteId = UI.StatusIconSpriteId.Disarm;
                    info.MaxStackCount = 1;
                    effect = new RootedEffect();
                    break;

                case StatusEffectId.Aura_RighteousGlory:
                    info.Duration = StatusEffectConstants.PERMANENT_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.RighteousGlory;
                    effect = new RighteousGloryEffect();
                    break;

                case StatusEffectId.BloodScent:
                    info.Duration = StatusEffectConstants.PERMANENT_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.BloodScent;
                    effect = new BloodScentEffect();
                    break;

                default:
                    Debug.Assert(false);
                    break;

            }

            effect.SetInfo(info);
            effect.StackCount = initialStacks;
            
            return effect;
        }
    }

}

