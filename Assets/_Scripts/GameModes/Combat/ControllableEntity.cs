using MAGE.GameModes.SceneElements;
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
using UnityEngine.AI;

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
                    foreach (ActionResponseId actionResponseId in Character.ActionResponders)
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

        public void OnTurnAvailable()
        {
            GetComponent<ResourcesControl>().Resources[ResourceType.Actions].SetCurrentToMax();
            GetComponent<ResourcesControl>().Resources[ResourceType.MovementRange].SetCurrentToMax();
        }

        public void OnTurnStart()
        {
            StatusEffectControl statusEffectControl = GetComponent<StatusEffectControl>();
            statusEffectControl.RemoveStatusEffects(statusEffectControl.GetSingleTurnStatusEffects(), false);
        }

        public override void OnTurnTick()
        {
            float attunementModifier = GetComponent<StatsControl>().Attributes[SecondaryStat.Attunement] / 400f;
            float resourceRecoverModifier = Mathf.Max(GetComponent<StatsControl>().Attributes[TertiaryStat.ResourceRecovery], 0);
            
            GetComponent<ResourcesControl>().Resources[ResourceType.Mana]
                .Modify(attunementModifier * resourceRecoverModifier);

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

                GetComponentInChildren<NavMeshObstacle>(true).gameObject.SetActive(false);
            }
        }
    }
}
