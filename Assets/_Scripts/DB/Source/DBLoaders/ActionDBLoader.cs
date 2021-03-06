﻿using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    static class ActionDBLoader
    {
        private static DB.DBRangeInfo SpellCastRange = new DB.DBRangeInfo() { AreaType = (int)AreaType.Circle, Elevation = 3, Min = 0, Max = 4 };
        private static DB.DBRangeInfo MeleeCastRange = new DB.DBRangeInfo() { AreaType = (int)AreaType.Circle, Elevation = 1, Min = 1, Max = 1 };
        private static DB.DBRangeInfo SmallAreaEffectRange = new DB.DBRangeInfo() { AreaType = (int)AreaType.Circle, Elevation = 1, Min = 0, Max = 1 };
        private static DB.DBRangeInfo MediumAreaEffectRange = new DB.DBRangeInfo() { AreaType = (int)AreaType.Circle, Elevation = 1, Min = 0, Max = 2 };
        private static DB.DBRangeInfo UnitEffectRage = new DB.DBRangeInfo() { AreaType = (int)AreaType.Circle, Elevation = 0, Min = 0, Max = 0 };
        private static DB.DBRangeInfo ConeEffectRange = new DB.DBRangeInfo() { AreaType = (int)AreaType.Cone, Elevation = 1, Min = 0, Max = 1 };
        private static DB.DBRangeInfo CrossEffectRange = new DB.DBRangeInfo() { AreaType = (int)AreaType.Cross, Elevation = 2, Min = 1, Max = 5 };

        public static void LoadDB()
        {
            { // Anvil
                DB.DBAction entry = new DB.DBAction();

                ActionId actionId = ActionId.Anvil;

                entry.Id = (int)actionId;
                entry.Name = actionId.ToString();
                entry.ActionRange = (int)ActionRange.AOE;
                entry.ActionSource = (int)ActionSource.Weapon;
                entry.CastSpeed = (int)CastSpeed.Instant;
                entry.AnimationId = (int)AnimationId.SwordSwing;
                entry.EffectId = (int)EffectType.Fire;

                entry.CastRange = MeleeCastRange;
                entry.EffectRange = ConeEffectRange;

                DBService.Get().WriteAction((int)actionId, entry);
            }

            { // Fire Ball
                DB.DBAction entry = new DB.DBAction();

                ActionId actionId = ActionId.FireBall;

                entry.Id = (int)actionId;
                entry.Name = actionId.ToString();
                entry.ActionRange = (int)ActionRange.Projectile;
                entry.ActionSource = (int)ActionSource.Cast;
                entry.CastSpeed = (int)CastSpeed.Fast;
                entry.AnimationId = (int)AnimationId.Cast;
                entry.ProjectileId = (int)ProjectileId.FireBall;
                entry.EffectId = (int)EffectType.Fire;

                entry.CastRange = SpellCastRange;
                entry.EffectRange = UnitEffectRage;

                DBService.Get().WriteAction((int)actionId, entry);
            }

            { // Heal
                DB.DBAction entry = new DB.DBAction();

                ActionId actionId = ActionId.Heal;

                entry.Id = (int)actionId;
                entry.Name = actionId.ToString();
                entry.ActionRange = (int)ActionRange.AOE;
                entry.ActionSource = (int)ActionSource.Cast;
                entry.CastSpeed = (int)CastSpeed.Fast;
                entry.AnimationId = (int)AnimationId.Cast;
                entry.EffectId = (int)EffectType.AOE_Heal;
                entry.SFXId = (int)SFXId.Heal;

                entry.CastRange = SpellCastRange;
                entry.EffectRange = SmallAreaEffectRange;

                DBService.Get().WriteAction((int)actionId, entry);
            }

            { // HolyLight
                DB.DBAction entry = new DB.DBAction();

                ActionId actionId = ActionId.HolyLight;

                entry.Id = (int)actionId;
                entry.Name = actionId.ToString();
                entry.ActionRange = (int)ActionRange.AOE;
                entry.ActionSource = (int)ActionSource.Cast;
                entry.CastSpeed = (int)CastSpeed.Fast;
                entry.AnimationId = (int)AnimationId.Cast;
                entry.EffectId = (int)EffectType.AOE_Heal;
                entry.IsSelfCast = true;

                entry.EffectRange = MediumAreaEffectRange;

                DBService.Get().WriteAction((int)actionId, entry);
            }

            { // MightyBlow
                DB.DBAction entry = new DB.DBAction();

                ActionId actionId = ActionId.MightyBlow;

                entry.Id = (int)actionId;
                entry.Name = actionId.ToString();
                entry.ActionRange = (int)ActionRange.Meele;
                entry.ActionSource = (int)ActionSource.Weapon;
                entry.CastSpeed = (int)CastSpeed.Instant;
                entry.AnimationId = (int)AnimationId.SwordSwing;

                entry.CastRange = MeleeCastRange;
                entry.EffectRange = UnitEffectRage;

                DBService.Get().WriteAction((int)actionId, entry);
            }

            { // Protection
                DB.DBAction entry = new DB.DBAction();

                ActionId actionId = ActionId.Protection;

                entry.Id = (int)actionId;
                entry.Name = actionId.ToString();
                entry.ActionRange = (int)ActionRange.AOE;
                entry.ActionSource = (int)ActionSource.Cast;
                entry.CastSpeed = (int)CastSpeed.Fast;
                entry.AnimationId = (int)AnimationId.Cast;
                entry.EffectId = (int)EffectType.AOE_Heal;

                entry.CastRange = SpellCastRange;
                entry.EffectRange = SmallAreaEffectRange;

                DBService.Get().WriteAction((int)actionId, entry);
            }

            { // Shackle
                DB.DBAction entry = new DB.DBAction();

                ActionId actionId = ActionId.Shackle;

                entry.Id = (int)actionId;
                entry.Name = actionId.ToString();
                entry.ActionRange = (int)ActionRange.AOE;
                entry.ActionSource = (int)ActionSource.Cast;
                entry.CastSpeed = (int)CastSpeed.Instant;
                entry.AnimationId = (int)AnimationId.Cast;
                entry.EffectId = (int)EffectType.Fire;

                entry.CastRange = SpellCastRange;
                entry.EffectRange = UnitEffectRage;

                DBService.Get().WriteAction((int)actionId, entry);
            }

            { // Smite
                DB.DBAction entry = new DB.DBAction();

                ActionId actionId = ActionId.Smite;

                entry.Id = (int)actionId;
                entry.Name = actionId.ToString();
                entry.ActionRange = (int)ActionRange.AOE;
                entry.ActionSource = (int)ActionSource.Cast;
                entry.CastSpeed = (int)CastSpeed.Instant;
                entry.AnimationId = (int)AnimationId.Cast;
                entry.EffectId = (int)EffectType.Fire;

                entry.CastRange = SpellCastRange;
                entry.EffectRange = UnitEffectRage;

                DBService.Get().WriteAction((int)actionId, entry);
            }

            { // Summon Bear
                DB.DBAction entry = new DB.DBAction();

                ActionId actionId = ActionId.SummonBear;

                entry.Id = (int)actionId;
                entry.Name = actionId.ToString();
                entry.ActionRange = (int)ActionRange.Meele;
                entry.ActionSource = (int)ActionSource.Cast;
                entry.CastSpeed = (int)CastSpeed.Instant;
                entry.AnimationId = (int)AnimationId.Cast;
                entry.SummonType = (int)SpecializationType.Bear;
                entry.SummonCount = 1;
                entry.MaxSummonCount = 1;

                MeleeCastRange.CopyTo(entry.CastRange);
                entry.CastRange.TargetingType = (int)TargetingType.Empty;

                entry.EffectRange = UnitEffectRage;

                DBService.Get().WriteAction((int)actionId, entry);
            }

            { // Weapon Attack
                DB.DBAction entry = new DB.DBAction();

                ActionId actionId = ActionId.MeleeAttack;

                entry.Id = (int)actionId;
                entry.Name = actionId.ToString();
                entry.CastSpeed = (int)CastSpeed.Instant;
                entry.EffectRange = UnitEffectRage;
                entry.ActionSource = (int)ActionSource.Weapon;

                DBService.Get().WriteAction((int)actionId, entry);
            }

            DBService.Get().UpdateActionDB();
        }
    }
}



