using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EncounterSystem.Action
{
    using Common.ActionEnums;
    using Common.ActionTypes;
    using Common.AttributeEnums;
    using Common.AttributeTypes;
    using Common.StatusEnums;
    using Common.EquipmentEnums;
    using Common.EquipmentTypes;

    using Animation;
    using Character;
    using MapEnums;
    using MapTypes;
    using global::ActionUtil;

    class ActionFactory : MonoBehaviour
    {
        public static ActionFactory Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public List<ParticleSystem> mParticleSystemPrefabs;
        private static readonly Dictionary<ActionIndex, AnimationIndex> mAnimationLookUp = new Dictionary<ActionIndex, AnimationIndex>()
            {
                {ActionIndex.FIRE, AnimationIndex.FIRE1 },
                {ActionIndex.FIRE_BLAST, AnimationIndex.FIRE1 },
                {ActionIndex.FLAME_THROWER, AnimationIndex.FIRE1 },
                {ActionIndex.HEAL, AnimationIndex.FIRE1 },
                {ActionIndex.BERSERKER_BATTLECRY, AnimationIndex.FIRE1 },
                {ActionIndex.SPELL_WEAPON, AnimationIndex.FIRE2 },
            };

        public ActionBase CheckoutAttackAction(List<WeaponBase> weapons)
        {
            int weaponIndex = -1;

            for (int i = 0; i < 2; i++)
            {
                if (weapons[i] != null && weapons[i].WeaponType != WeaponType.Unarmed)
                    weaponIndex = i;
            }

            if (weaponIndex == -1)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (weapons[i] != null)
                        weaponIndex = i;
                }
            }
            Debug.Assert(weaponIndex != -1);

            GameObject actionObject = new GameObject("Attack");
            actionObject.transform.SetParent(gameObject.transform);
            _GetActionPriority actionPriority  = (CharacterManager caster, List<CharacterManager> targets) =>
            {
                float priority = 0f;
                foreach (CharacterManager target in targets)
                {
                    if (caster.IsPlayerControlled ^ target.IsPlayerControlled && target.IsAlive)
                    {
                        priority += .5f;
                    }
                }
                return priority;
            };
            ResourceChange actionCost = new ResourceChange(Resource.INVALID, 0);
            ActionResourceChangeInformation resourceChangeInfo = new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Physical, ActionEffectType.Meele, false, 0, 1.5f, AllignmentType.Unalligned, weapons[weaponIndex].DamageComposition);
            List<ActionModifier> actionModifiers = new List<ActionModifier>();
            ActionBase toCheckOut = null;

            switch (weapons[weaponIndex].ActionIndex)
            {
                case (ActionIndex.MELEE_WEAPON):
                    toCheckOut = actionObject.AddComponent<ActionMeleeWeaponBase>();
                    break;

                case (ActionIndex.RANGED_WEAPON):
                    toCheckOut = actionObject.AddComponent<ActionRangedWeaponBase>();
                    break;
                case (ActionIndex.SPELL_WEAPON):
                    actionCost.Resource = Resource.Mana;
                    actionCost.Value = 10f; // return mana
                    resourceChangeInfo = new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Magic, ActionEffectType.AOE, false, 0, 1.5f);
                    actionModifiers.AddRange(weapons[weaponIndex].DamageModifiers);
                    toCheckOut = actionObject.AddComponent<SpellBase>();
                    break;
            }



            toCheckOut.Initialize(new ActionInfo(weapons[weaponIndex].ActionIndex, "Attack", ActionConstants.InstantChargeTime, actionModifiers, actionCost,
                new List<Prerequisite>() { new EquipmentPrerequisite(new List<KeyValuePair<EquipmentCategory, int>>() { new KeyValuePair<EquipmentCategory, int>(EquipmentCategory.Weapon, -1) }) },
                resourceChangeInfo,
                new List<StatusEffectIndex>(),
                ActionRootEffect.Harmful),
                weapons[weaponIndex].AttackAreaInfo,
                new AnimationInfo("", "", ""),
                mAnimationLookUp.ContainsKey(weapons[weaponIndex].ActionIndex) ?
                new ActionAnimationSystem(mParticleSystemPrefabs[(int)mAnimationLookUp[weapons[weaponIndex].ActionIndex]], actionObject.transform) : null,
                actionPriority);

            return toCheckOut;
        }

        public ActionBase CheckoutAction(ActionIndex index)
        {
            GameObject actionObject = new GameObject(index.ToString());
            actionObject.transform.SetParent(gameObject.transform);

            int
            /* MapInteractionInfo*/
            maxRange, minRange, maxRangeElevation, maxAoe, minAoe, maxElevationAoe,
            /*ChargeTime*/
            chargeTime = ActionConstants.InstantChargeTime;
            float
            /* ResourceChangeInfo*/
            baseCritMultiplier = 1.5f
            ;
            _GetActionPriority actionPriority = (CharacterManager caster, List<CharacterManager> targets) =>
            {
                float priority = 0f;
                foreach(CharacterManager target in targets)
                {
                    if (caster.IsPlayerControlled ^ target.IsPlayerControlled && target.IsAlive)
                    {
                        priority += .5f;
                    }
                }
                return priority; };
            TargetSelectionType targetType;
            TileAreaType
                actionAreaType = TileAreaType.Circle,
                aoeAreaType = TileAreaType.Circle;
            string
            actionName = "",
            /* AnimationInfo */
            chargeAnimationId = "",
            preActionAnimationId = "",
            actionAnimationId = "";

            ResourceChange actionCost;
            ActionResourceChangeInformation resourceChangeInformation;
            List<ActionModifier> actionModifiers = new List<ActionModifier>();
            List<StatusEffectIndex> statusEffects = new List<StatusEffectIndex>();
            List<Prerequisite> prerequisites = new List<Prerequisite>();
            ActionRootEffect rootEffect = ActionRootEffect.Beneficial;
            ActionBase toCheckOut = null;
            switch (index)
            {

                case (ActionIndex.SHIELD_BASH):

                    maxRange = 1; minRange = 1; maxRangeElevation = 1; maxAoe = 0; minAoe = 0; maxElevationAoe = 0;
                    targetType = TargetSelectionType.Targeted;
                    actionName = "Advance";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Endurance, -40);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Physical, ActionEffectType.Meele, false, 0, baseCritMultiplier);
                    prerequisites.Add(new EquipmentPrerequisite(new List<KeyValuePair<EquipmentCategory, int>>() { new KeyValuePair<EquipmentCategory, int>(EquipmentCategory.Shield, -1) }));

                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return container[AttributeType.Stat][(int)PrimaryStat.Might] * 0.5f; }, ModifierType.Additive));
                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return 10 * container[AttributeType.Stat][(int)TertiaryStat.FrontalBlock]; }, ModifierType.Multiplicative));

                    statusEffects.Add(StatusEffectIndex.CAST_SLOW);
                    rootEffect = ActionRootEffect.Harmful;
                    actionPriority = (CharacterManager caster, List<CharacterManager> bashTargets) =>
                    {
                        const float enemy = 1f;
                        const float ally = -1f;
                        float priority = 0f;
                        foreach (var target in bashTargets)
                        {
                            if (caster.IsPlayerControlled ^ target.IsPlayerControlled)
                            {
                                priority += 0.4f; // + is casting
                            }
                        }
                        return priority;
                    };
                    toCheckOut = actionObject.AddComponent<SpellBase>();
                    break;

                case (ActionIndex.SHIELDWALL_ADVANCE):
                    maxRange = 1; minRange = 1; maxRangeElevation = 0; maxAoe = 0; minAoe = 0; maxElevationAoe = 0;
                    targetType = TargetSelectionType.Targeted;
                    actionName = "Advance";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Endurance, -25);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Physical, ActionEffectType.Meele, false, 0, baseCritMultiplier);
                    prerequisites.Add(new EquipmentPrerequisite(new List<KeyValuePair<EquipmentCategory, int>>() { new KeyValuePair<EquipmentCategory, int>(EquipmentCategory.Shield, -1) }));

                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return container[AttributeType.Stat][(int)PrimaryStat.Might] * 0.5f; }, ModifierType.Additive));
                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return 1 + container[AttributeType.Stat][(int)TertiaryStat.FrontalBlock]; }, ModifierType.Multiplicative));
                    rootEffect = ActionRootEffect.Harmful;

                    toCheckOut = actionObject.AddComponent<ShieldWallAdvance>();
                    break;

                case (ActionIndex.OPPORTUNITY_STRIKE):
                    maxRange = 1; minRange = 1; maxRangeElevation = 1; maxAoe = 0; minAoe = 0; maxElevationAoe = 0;
                    targetType = TargetSelectionType.Targeted;
                    actionName = "Opportunity Strike";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.INVALID, -1);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Magic, ActionEffectType.Meele, false, 0, baseCritMultiplier, AllignmentType.Dark);
                    prerequisites.Add(new EquipmentPrerequisite(new List<KeyValuePair<EquipmentCategory, int>>() { new KeyValuePair<EquipmentCategory, int>(EquipmentCategory.Shield, -1) }));

                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return container[AttributeType.Stat][(int)PrimaryStat.Finese] * 1.5f; }, ModifierType.Additive));
                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return container[AttributeType.Stat][(int)SecondaryStat.Fortitude] * 0.01f; }, ModifierType.Multiplicative));

                    statusEffects.Add(StatusEffectIndex.INTERPUPT);
                    rootEffect = ActionRootEffect.Harmful;

                    toCheckOut = actionObject.AddComponent<SpellBase>();
                    break;

                case (ActionIndex.FIRE):
                    maxRange = 3; minRange = 0; maxRangeElevation = 2; maxAoe = 1; minAoe = 0; maxElevationAoe = 1;
                    aoeAreaType = TileAreaType.Circle;
                    actionAreaType = TileAreaType.Circle;

                    targetType = TargetSelectionType.Targeted;
                    chargeTime = 34;
                    actionName = "Fire";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Mana, -25);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Magic, ActionEffectType.AOE, false, 0, baseCritMultiplier, AllignmentType.Fire);

                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return container[AttributeType.Stat][(int)PrimaryStat.Magic]; }, ModifierType.Additive));
                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return 1 + container[AttributeType.Stat][(int)SecondaryStat.Attunement] * 0.01f; }, ModifierType.Multiplicative));
                    
                    rootEffect = ActionRootEffect.Harmful;
                    actionPriority = (CharacterManager caster, List<CharacterManager> fireTargets) =>
                    {
                        const float enemy = .5f;
                        const float ally = -.4f;
                        float priority = 0f;
                        foreach (var target in fireTargets)
                        {
                            float currentHealthPercent = target.Resources[(int)Resource.Health] / target.Resources[(int)Resource.MaxHealth];
                            priority += (!(caster.IsPlayerControlled ^ target.IsPlayerControlled) ? ally : enemy) * (1 + (currentHealthPercent == 0 ? 0 : 1 - currentHealthPercent));
                        }
                        return priority;
                    };
                    toCheckOut = actionObject.AddComponent<SpellBase>();
                    break;

                case (ActionIndex.FLAME_THROWER):
                    maxRange = 5; minRange = 1; maxRangeElevation = 3; maxAoe = maxRange; minAoe = minRange; maxElevationAoe = 1;
                    aoeAreaType = TileAreaType.Line;
                    actionAreaType = TileAreaType.Cross;
                    targetType = TargetSelectionType.Targeted;
                    chargeTime = 34;
                    actionName = "Flame Thrower";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Mana, -25);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Magic, ActionEffectType.AOE, false, 0, baseCritMultiplier, AllignmentType.Fire);

                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return 2 * container[AttributeType.Stat][(int)PrimaryStat.Magic]; }, ModifierType.Additive));
                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return container[AttributeType.Stat][(int)SecondaryStat.Attunement] * 0.01f; }, ModifierType.Multiplicative));
                    
                    rootEffect = ActionRootEffect.Harmful;
                    actionPriority = (CharacterManager caster, List<CharacterManager> fireTargets) =>
                    {
                        const float enemy = .5f;
                        const float ally = -.4f;
                        float priority = 0f;
                        foreach (var target in fireTargets)
                        {
                            float currentHealthPercent = target.Resources[(int)Resource.Health] / target.Resources[(int)Resource.MaxHealth];
                            priority += (!(caster.IsPlayerControlled ^ target.IsPlayerControlled) ? ally : enemy) * (1 + (currentHealthPercent == 0 ? 0 : 1 - currentHealthPercent));
                        }
                        return priority;
                    };
                    toCheckOut = actionObject.AddComponent<SpellBase>();
                    break;

                case (ActionIndex.FIRE_BLAST):
                    maxRange = 0; minRange = 0; maxRangeElevation = 0; maxAoe = 3; minAoe = 1; maxElevationAoe = 1;
                    aoeAreaType = TileAreaType.Circle;
                    actionAreaType = TileAreaType.Circle;
                    targetType = TargetSelectionType.Auto;
                    chargeTime = 34;
                    actionName = "Fire Blast";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Mana, -25);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Magic, ActionEffectType.AOE, false, 0, baseCritMultiplier, AllignmentType.Fire);

                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return 1.5f * container[AttributeType.Stat][(int)PrimaryStat.Magic]; }, ModifierType.Additive));
                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return 1.5f + container[AttributeType.Stat][(int)SecondaryStat.Attunement] * 0.01f; }, ModifierType.Multiplicative));
                    
                    rootEffect = ActionRootEffect.Harmful;

                    actionPriority = (CharacterManager caster, List<CharacterManager> fireTargets) =>
                    {
                        const float enemy = .5f;
                        const float ally = -.4f;
                        float priority = 0f;
                        foreach (var target in fireTargets)
                        {
                            float currentHealthPercent = target.Resources[(int)Resource.Health] / target.Resources[(int)Resource.MaxHealth];
                            priority += (!(caster.IsPlayerControlled ^ target.IsPlayerControlled) ? ally : enemy) * (1 + (currentHealthPercent == 0 ? 0 : 1 - currentHealthPercent));
                        }
                        return priority;
                    };

                    toCheckOut = actionObject.AddComponent<AOESpellBase>();
                    break;

                case (ActionIndex.HEAL):
                    maxRange = 3; minRange = 0; maxRangeElevation = 2; maxAoe = 1; minAoe = 0; maxElevationAoe = 1;
                    targetType = TargetSelectionType.Targeted;
                    chargeTime = 34;
                    actionName = "Heal";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Mana, -8);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 20, ActionBaseType.Magic, ActionEffectType.AOE, true, 0, baseCritMultiplier, AllignmentType.Light);

                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return container[AttributeType.Stat][(int)PrimaryStat.Magic]; }, ModifierType.Additive));
                    actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return container[AttributeType.Stat][(int)SecondaryStat.Attunement] * 0.01f; }, ModifierType.Multiplicative));
                    

                    actionPriority = (CharacterManager caster, List<CharacterManager> healTargets) =>
                    {
                        const float ally = 1f;
                        const float enemy = -.5f;
                        float priority = 0f;
                        foreach (var target in healTargets)
                        {
                            float currentHealthPercent = target.Resources[(int)Resource.Health] / target.Resources[(int)Resource.MaxHealth];
                            priority += (!(caster.IsPlayerControlled ^ target.IsPlayerControlled) ? ally : enemy) * (currentHealthPercent == 0 ? 0 : 1 - currentHealthPercent);
                        }
                        return priority;
                    };

                    toCheckOut = actionObject.AddComponent<SpellBase>();
                    break;

                case (ActionIndex.PROTECT):
                    maxRange = 3; minRange = 0; maxRangeElevation = 2; maxAoe = 1; minAoe = 0; maxElevationAoe = 1;
                    targetType = TargetSelectionType.Targeted;
                    chargeTime = 24;
                    actionName = "Protect";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Mana, -12);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Magic, ActionEffectType.AOE, true, 0, baseCritMultiplier, AllignmentType.Light);
                    statusEffects.Add(StatusEffectIndex.PROTECT_1);

                    actionPriority = (CharacterManager caster, List<CharacterManager> protectTargets) =>
                    {
                        const float ally = .5f;
                        const float enemy = -.5f;
                        float priority = 0f;
                        foreach (var target in protectTargets)
                        {
                            if (target.QueryStatusEffect(StatusEffectIndex.PROTECT_1) == null)
                            {
                                float characterWeakness = (1 - target.Stats[(int)TertiaryStat.PhysicalResistance]) / 2f;
                                priority += (!(caster.IsPlayerControlled ^ target.IsPlayerControlled) ? ally : enemy) * characterWeakness;
                            }
                        }
                        return priority;
                    };

                    toCheckOut = actionObject.AddComponent<SpellBase>();
                    break;

                case (ActionIndex.SHELL):
                    maxRange = 3; minRange = 0; maxRangeElevation = 2; maxAoe = 1; minAoe = 0; maxElevationAoe = 1;
                    targetType = TargetSelectionType.Targeted;
                    chargeTime = 24;
                    actionName = "Shell";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Mana, -12);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Magic, ActionEffectType.AOE, true, 0, baseCritMultiplier, AllignmentType.Light);
                    statusEffects.Add(StatusEffectIndex.SHELL_1);

                    actionPriority = (CharacterManager caster, List<CharacterManager> healTargets) =>
                    {
                        const float ally = .5f;
                        const float enemy = -.5f;
                        float priority = 0f;
                        foreach (var target in healTargets)
                        {
                            if (target.QueryStatusEffect(StatusEffectIndex.PROTECT_1) == null)
                            {
                                float characterWeakness = (1 - target.Stats[(int)TertiaryStat.MagicalResistance]) / 2f;
                                priority += (!(caster.IsPlayerControlled ^ target.IsPlayerControlled) ? ally : enemy) * characterWeakness;
                            }
                        }
                        return priority;
                    };

                    toCheckOut = actionObject.AddComponent<SpellBase>();
                    break;

                case (ActionIndex.CLEANSE):
                    maxRange = 3; minRange = 0; maxRangeElevation = 2; maxAoe = 0; minAoe = 0; maxElevationAoe = 1;
                    targetType = TargetSelectionType.Targeted;
                    chargeTime = 34;
                    actionName = "Cleanse";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Mana, -12);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Magic, ActionEffectType.AOE, true, 0, baseCritMultiplier, AllignmentType.Light);

                    actionPriority = (CharacterManager caster, List<CharacterManager> cleanseTargets) =>
                    {
                        const float ally = .5f;
                        const float enemy = -.35f;
                        float priority = 0f;
                        foreach (var target in cleanseTargets)
                        {
                            // if target has status effects
                            priority += (!(caster.IsPlayerControlled ^ target.IsPlayerControlled) ? ally : enemy);
                        }
                        //return priority;
                        return 0f;
                    };

                    toCheckOut = actionObject.AddComponent<Cleanse>();
                    break;

                case (ActionIndex.REVIVE):
                    maxRange = 3; minRange = 0; maxRangeElevation = 2; maxAoe = 0; minAoe = 0; maxElevationAoe = 1;
                    targetType = TargetSelectionType.Targeted;
                    chargeTime = 34;
                    actionName = "Revive";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Mana, -16);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Magic, ActionEffectType.AOE, true, 0, baseCritMultiplier, AllignmentType.Light);

                    actionPriority = (CharacterManager caster, List<CharacterManager> reviveTargets) =>
                    {
                        const float ally = 1f;
                        const float enemy = -1f;
                        float priority = 0f;
                        foreach (var target in reviveTargets)
                        {
                            priority += (!(caster.IsPlayerControlled ^ target.IsPlayerControlled) ? ally : enemy) * (target.IsAlive ? 0 : 1);
                        }
                        return priority;
                    };

                    toCheckOut = actionObject.AddComponent<Revive>();
                    break;

                case (ActionIndex.DEFEND):
                    maxRange = 0; minRange = 0; maxRangeElevation = 0; maxAoe = 0; minAoe = 0; maxElevationAoe = 0;
                    targetType = TargetSelectionType.Auto;
                    actionName = "Defend";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Endurance, -25);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Physical, ActionEffectType.Meele, true, 0, 0);
                    prerequisites.Add(new EquipmentPrerequisite(new List<KeyValuePair<EquipmentCategory, int>>() { new KeyValuePair<EquipmentCategory, int>(EquipmentCategory.Shield, -1) }));

                    statusEffects.Add(StatusEffectIndex.FRONTAL_DEFEND);
                    statusEffects.Add(StatusEffectIndex.PERIPHERAL_DEFEND);

                    actionPriority = (CharacterManager caster, List<CharacterManager> self) =>
                    {
                        return 0.2f;
                    };

                    toCheckOut = actionObject.AddComponent<SpellBase>();
                    break;

                case (ActionIndex.BERSERKER_BATTLECRY):
                    maxRange = 0; minRange = 0; maxRangeElevation = 0; maxAoe = 2; minAoe = 0; maxElevationAoe = 1;
                    targetType = TargetSelectionType.Auto;
                    actionName = "Battle Cry";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Endurance, -35);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Magic, ActionEffectType.Meele, true, 0, 0);
                    prerequisites.Add(new StatusPrerequisite(new List<KeyValuePair<StatusEffectIndex, int>>() { new KeyValuePair<StatusEffectIndex, int>(StatusEffectIndex.BERSERKER_BLOODSCENT, 0) }));
                    statusEffects.Add(StatusEffectIndex.PHYSICAL_DAMAGE_PERCENT);

                    actionPriority = (CharacterManager caster, List<CharacterManager> battlecryTargets) =>
                    {
                        var bloodScent = caster.QueryStatusEffect(StatusEffectIndex.BERSERKER_BLOODSCENT);
                        float multiplier = 0;
                        if (bloodScent != null) // should be moot as not having the status effect would mean not being able to use the ability
                        {
                            multiplier = bloodScent.StackCount;
                        }

                        const float ally = .1f;
                        const float enemy = -.1f;
                        float priority = 0f;
                        foreach (var target in battlecryTargets)
                        {
                            priority += (!(caster.IsPlayerControlled ^ target.IsPlayerControlled) ? ally : enemy) * multiplier;
                        }
                        return priority;
                    };

                    toCheckOut = actionObject.AddComponent<Berserker_BattleCry>();
                    break;

                case (ActionIndex.BERSERKER_CLEAVE):
                    maxRange = 1; minRange = 1; maxRangeElevation = 0; maxAoe = 0; minAoe = -1; maxElevationAoe = 1;
                    targetType = TargetSelectionType.Targeted;
                    aoeAreaType = TileAreaType.Cone;
                    actionName = "Cleave";
                    actionAnimationId = "Attack3Trigger";
                    actionCost = new ResourceChange(Resource.Endurance, -35);
                    resourceChangeInformation =
                        new ActionResourceChangeInformation(Resource.Health, 0, ActionBaseType.Magic, ActionEffectType.Meele, false, 0, baseCritMultiplier);
                    prerequisites.Add(new EquipmentPrerequisite(new List<KeyValuePair<EquipmentCategory, int>>() { new KeyValuePair<EquipmentCategory, int>(EquipmentCategory.Weapon, (int)WeaponType.MeleeWeapon) }));
                    // Get Weapon Modifiers and add this modifier to it
                    // Hard coding until there's a better way of determining ordering
                    //actionModifiers.Add(new ActionModifier((AttributeContainer container) => { return 0.7f; }, ModifierType.Multiplicative));

                    actionPriority = (CharacterManager caster, List<CharacterManager> battlecryTargets) =>
                    {
                        var bloodScent = caster.QueryStatusEffect(StatusEffectIndex.BERSERKER_BLOODSCENT);
                        float multiplier = 0;
                        if (bloodScent != null) // should be moot as not having the status effect would mean not being able to use the ability
                        {
                            multiplier = bloodScent.StackCount;
                        }

                        const float ally = -.5f;
                        const float enemy = .5f;
                        float priority = 0f;
                        foreach (var target in battlecryTargets)
                        {
                            priority += (!(caster.IsPlayerControlled ^ target.IsPlayerControlled) ? ally : enemy) * multiplier;
                        }
                        return priority;
                    };
                    rootEffect = ActionRootEffect.Harmful;

                    toCheckOut = actionObject.AddComponent<Berserker_Cleave>();
                    break;
                default:
                    Debug.LogError(index.ToString() + " is not hooked up in Action Factory yet!");
                    return null;
            }

            if (toCheckOut != null)
            {
                toCheckOut.Initialize(
                           new ActionInfo(index, actionName, chargeTime,
                               actionModifiers,
                               actionCost,
                               prerequisites,
                               resourceChangeInformation,
                               statusEffects,
                               rootEffect
                           ),
                           new MapInteractionInfo(targetType, actionAreaType, aoeAreaType, maxRange, minRange, maxRangeElevation, maxAoe, minAoe, maxElevationAoe),
                           new AnimationInfo(chargeAnimationId, preActionAnimationId, actionAnimationId),
                           mAnimationLookUp.ContainsKey(index) ?
                           new ActionAnimationSystem(mParticleSystemPrefabs[(int)mAnimationLookUp[index]], actionObject.transform) : null,
                           actionPriority
                           );
            }
            else
            {
                Debug.LogError("No Ability created for " + index.ToString());
            }

            return toCheckOut;
        }

    }

}

