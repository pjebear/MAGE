using MAGE.GameModes.Combat;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Items;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    abstract class TargetingSolverBase : IDeferredVar<Target>
    {
        public IDeferredVar<CombatEntity> Targeting;
        public TargetSelection BeingTargeted;
        public List<Target> Targets = new List<Target>();
        public int TargetIdx = 0;

        public Target Get()
        {
            if (TargetIdx < Targets.Count)
            {
                return Targets[TargetIdx];
            }
            else
            {
                return Target.Empty;
            }
        }

        public bool HasTarget()
        {
            return TargetIdx < Targets.Count;
        }

        public bool Next()
        {
            ++TargetIdx;
            return HasTarget();
        }

        public abstract void Solve();
    }

    class TargetingSolver : TargetingSolverBase
    {
        public override void Solve()
        {
            if (BeingTargeted.SelectionRange.AreaType == AreaType.Point)
            {
                Debug.Assert(BeingTargeted.FocalTarget.TargetType == TargetSelectionType.Focal);
                Targets = new List<Target>() { BeingTargeted.FocalTarget };
            }
            else
            {
                Targets = LevelManagementService.Get().GetLoadedLevel().GetTargetsInRange(Targeting.Get(), BeingTargeted).Select( x=> new Target(x)).ToList();
            }
        }
    }

    class EmptySpaceSolver : TargetingSolverBase
    {
        public int SpaceCount = 1;

        public override void Solve()
        {
            Level level = LevelManagementService.Get().GetLoadedLevel();

            switch (BeingTargeted.SelectionRange.AreaType)
            {
                case AreaType.Point:
                {
                    if (!level.QueryObjectExistsAtPoint(BeingTargeted.FocalTarget.GetTargetPoint(), 1.25f))
                    {
                        Targets.Add(BeingTargeted.FocalTarget);
                    }
                }
                break;
                case AreaType.Circle:
                {
                    List<Vector3> pointsInCircle = level.GetPointsAroundCircle(BeingTargeted.FocalTarget.GetTargetPoint(), 1.25f, SpaceCount);
                    Targets = level.FilterEmptyPoints(pointsInCircle, .25f).Select(x => new Target(x)).ToList();
                }
                break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }
    }

    abstract class InteractionSolverBase
    {
        public IDeferredVar<StateChange> StateChange;
        public DeferredInteractionResult InteractionResult = new DeferredInteractionResult();

        public Dictionary<CombatTarget, InteractionResult> Results = new Dictionary<CombatTarget, InteractionResult>();

        public abstract void Solve(CombatEntity interacting, Target target);
    }

    class WeaponInteractionSolver : InteractionSolverBase
    {
        public override void Solve(CombatEntity interacting, Target target)
        {
            if (target.TargetType == TargetSelectionType.Focal)
            {
                CombatTarget combatTarget = target.FocalTarget;

                InteractionResult result = InteractionResolver.GetWeaponInteractionResult(interacting, combatTarget, StateChange.Get());
                InteractionUtil.GetOwnerResultTypeFromResults(new List<InteractionResult>() { result });
                Results.Add(combatTarget, result);
                InteractionResult.Set(result);
            }
        }
    }

    class SpellInteractionSolver : InteractionSolverBase
    {
        public override void Solve(CombatEntity interacting, Target target)
        {
            if (target.TargetType == TargetSelectionType.Focal)
            {
                CombatTarget combatTarget = target.FocalTarget;

                InteractionResult result = new InteractionResult(InteractionResultType.Hit, StateChange.Get());
                Results.Add(combatTarget, result);
                InteractionResult.Set(result);
            }
        }
    }

    class SummonInteractionSolver : InteractionSolverBase
    {
        public override void Solve(CombatEntity interacting, Target target)
        {
            // empty
        }
    }

    class SpellEffectivenessCalculator : IDeferredVar<int>
    {
        public bool IsBeneficial = false;
        public float BaseEffectiveness;
        public IDeferredVar<StatsControl> DeferredCaster;

        public int Get()
        {
            float effectiveness = BaseEffectiveness;
            if (DeferredCaster.Get() != null)
            {
                Attributes attributes = DeferredCaster.Get().Attributes;
                effectiveness += attributes[PrimaryStat.Magic] / 8;
                effectiveness *= 1 + (attributes[SecondaryStat.Attunement] / 100);
            }
            if (!IsBeneficial)
            {
                effectiveness *= -1;
            }
            if (IsBeneficial) effectiveness = Mathf.Max(effectiveness, 0);
            else effectiveness = Mathf.Min(effectiveness, 0);

            return (int)effectiveness;
        }
    }

    class WeaponEffectivenessCalculator : IDeferredVar<int>
    {
        public IDeferredVar<CombatEntity> DeferredCombatEntity;
        private Optional<int> mCachedEffectiveness;
        private float mBaseEffectiveness = 0;
        private float mMultiplier = 0;
        private Equipment.Slot mEquipmentSlot = Equipment.Slot.RightHand;

        public WeaponEffectivenessCalculator(Equipment.Slot equipmentSlot, float baseEffectiveness = 0, float multiplier = 1)
        {
            mBaseEffectiveness = baseEffectiveness;
            mEquipmentSlot = equipmentSlot;
            mMultiplier = multiplier;
        }

        public int Get()
        {
            if (!mCachedEffectiveness.HasValue)
            {
                CombatEntity combatEntity = DeferredCombatEntity.Get();
                UnityEngine.Debug.Assert(combatEntity != null);
                if (combatEntity != null)
                {
                    EquipmentControl attackerEquipment = combatEntity.GetComponent<EquipmentControl>();
                    StatsControl attackerStats = combatEntity.GetComponent<StatsControl>();

                    HeldEquippable heldEquippable = (attackerEquipment.Equipment[mEquipmentSlot] as HeldEquippable);

                    float damage = mBaseEffectiveness;
                    foreach (AttributeScalar scalar in heldEquippable.EffectivenessScalars)
                    {
                        damage += scalar.GetScalar(attackerStats.Attributes);
                    }
                    damage *= mMultiplier;

                    mCachedEffectiveness = -(int)damage;
                }
            }

            return mCachedEffectiveness.Value;
        }
    }
}
