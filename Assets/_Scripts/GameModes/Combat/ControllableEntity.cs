﻿using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Combat
{
    [RequireComponent(typeof(ActionsControl))]
    [RequireComponent(typeof(AurasControl))]
    [RequireComponent(typeof(CharacterPickerControl))]
    [RequireComponent(typeof(CombatDisplayable))]
    [RequireComponent(typeof(CombatTarget))]
    [RequireComponent(typeof(EquipmentControl))]
    [RequireComponent(typeof(ResourcesControl))]
    [RequireComponent(typeof(StatsControl))]
    [RequireComponent(typeof(StatusEffectControl))]
    [RequireComponent(typeof(SummonHeirarchy))]
    class ControllableEntity : CombatEntity
    {
        public int Id = -1;
        public Character Character = null;

        void OnCharacterChanged()
        {
            Debug.Assert(Id == -1);
            if (Id == -1)
            {
                Id = GetComponent<CharacterPickerControl>().CharacterId;

                ICharacterService characterService = CharacterService.Get();
                if (characterService != null)
                {
                    Character = characterService.GetCharacter(Id);

                    GetComponent<StatsControl>().SetAttributes(Character.CurrentAttributes);
                    GetComponent<ResourcesControl>().InitResourcesFromAttributes(Character.CurrentAttributes);
                    GetComponent<ActionsControl>().Actions = Character.GetActionIds();
                    GetComponent<EquipmentControl>().SetEquipment(Character.Equipment);

                    // Responders
                    List<ActionResponseId> responseIds = new List<ActionResponseId>()
                {
                    ActionResponseId.Riptose
                };
                    foreach (ActionResponseId actionResponseId in responseIds)
                    {
                        GetComponent<Combat.ActionsControl>().ActionResponders.Add(
                            ActionResponderFactory.CheckoutResponder(GetComponent<CombatEntity>(), actionResponseId));
                    }

                    // Auras
                    foreach (AuraType auraType in Character.GetAuras())
                    {
                        GetComponent<AurasControl>().ApplyAura(auraType);
                    }
                }

                GetComponent<Actor>().SetInCombat(true);

                EncounterContainer container = GetComponentInParent<EncounterContainer>();
                Debug.Assert(container != null);
                if (container)
                {
                    Transform teamTransform = transform.parent;
                    TeamSide = container.Allys == teamTransform ? TeamSide.AllyHuman : TeamSide.EnemyAI;
                    container.EncounterModel.AddPlayer(this);
                }
            }
        }

        public void OnTurnStart()
        {
            StatusEffectControl statusEffectControl = GetComponent<StatusEffectControl>();
            statusEffectControl.RemoveStatusEffects(statusEffectControl.GetSingleTurnStatusEffects());

            if (GetComponent<CombatTarget>() != null)
            {
                List<StateChange> turnStateStateChanges = statusEffectControl.GetTurnStartStateChanges();
                foreach (StateChange stateChange in turnStateStateChanges)
                {
                    GetComponent<CombatTarget>().ApplyStateChange(stateChange);
                }
            }
            
            GetComponent<ResourcesControl>().Resources[ResourceType.Actions].SetCurrentToMax();
            GetComponent<ResourcesControl>().Resources[ResourceType.MovementRange].SetCurrentToMax();
            
        }

        public override void OnTurnTick()
        {
            float maxMana = GetComponent<ResourcesControl>().Resources[ResourceType.Mana].Max;
            float attunementModifier = GetComponent<StatsControl>().Attributes[SecondaryStat.Attunement] / 100f;
            float resourceRecoverModifier = Mathf.Max(GetComponent<StatsControl>().Attributes[TertiaryStat.ResourceRecovery], 0);
            float tuningModifer = .04f;
            GetComponent<ResourcesControl>().Resources[ResourceType.Mana]
                .Modify(maxMana * attunementModifier * resourceRecoverModifier * tuningModifer);

            GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Clock].Modify(
                GetComponent<StatsControl>().Attributes[GameSystems.Stats.TertiaryStat.Speed]);
        }

        public override void OnDeath()
        {
            if (GetComponent<SummonHeirarchy>().Owner != null)
            {
                GameModel.Encounter.RemovePlayer(Id);
                Destroy(gameObject);
            }
            else
            {
                GetComponent<AudioSource>().PlayOneShot(AudioManager.Instance.GetSFXClip(SFXId.MaleDeath));
                GetComponent<ActorAnimator>()?.Trigger("die");
                GetComponent<CapsuleCollider>().enabled = false;
            }
        }
    }
}