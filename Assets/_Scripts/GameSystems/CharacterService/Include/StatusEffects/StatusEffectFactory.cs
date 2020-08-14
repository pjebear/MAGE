using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameServices.Character
{
    static class StatusEffectFactory
    {
        public static StatusEffect CheckoutStatusEffect(StatusEffectType type, GameModes.Encounter.EncounterCharacter owner, int initialStacks = 1)
        {
            StatusEffect effect = null;
            StatusEffectInfo info = new StatusEffectInfo();
            info.Type = type;

            switch (type)
            {
                case StatusEffectType.Avenger:
                    info.Duration = StatusEffectConstants.PERMANENT_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Avenger;
                    info.MaxStackCount = 100;
                    effect = new AvengerEffect(owner, info);
                    break;

                case StatusEffectType.Poison:
                    info.Duration = StatusEffectConstants.MEDIUM_DURATION;
                    info.Beneficial = false;
                    info.SpriteId = UI.StatusIconSpriteId.Poison;
                    effect = new PoisonEffect(owner, info);
                    break;

                case StatusEffectType.Protection:
                    info.Duration = StatusEffectConstants.MEDIUM_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Protection;
                    effect = new ProtectionEffect(owner, info);
                    break;

                case StatusEffectType.Aura_Protection:
                    info.Duration = StatusEffectConstants.PERMANENT_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Protection;
                    effect = new ProtectionEffect(owner, info);
                    break;

                case StatusEffectType.Regen:
                    info.Duration = StatusEffectConstants.MEDIUM_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Protection;
                    effect = new RegenEffect(owner, info);
                    break;

                case StatusEffectType.Aura_Regen:
                    info.Duration = StatusEffectConstants.PERMANENT_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.Protection;
                    effect = new RegenEffect(owner, info);
                    break;

                case StatusEffectType.Shackle:
                    info.Duration = StatusEffectConstants.SHORT_DURATION;
                    info.Beneficial = false;
                    info.SpriteId = UI.StatusIconSpriteId.Disarm;
                    effect = new ShackleEffect(owner, info);
                    break;

                case StatusEffectType.Aura_RighteousGlory:
                    info.Duration = StatusEffectConstants.PERMANENT_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.RighteousGlory;
                    effect = new RighteousGloryEffect(owner, info);
                    break;

                case StatusEffectType.BloodScent:
                    info.Duration = StatusEffectConstants.PERMANENT_DURATION;
                    info.Beneficial = true;
                    info.SpriteId = UI.StatusIconSpriteId.BloodScent;
                    effect = new BloodScentEffect(owner, info);
                    break;

                default:
                    Debug.Assert(false);
                    break;

            }

            effect.StackCount = initialStacks;

            return effect;
        }
    }

}

