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

namespace MAGE.GameModes.Combat
{
    [RequireComponent(typeof(CombatEntity))]
    [RequireComponent(typeof(ActionsControl))]
    [RequireComponent(typeof(AurasControl))]
    [RequireComponent(typeof(EquipmentControl))]
    [RequireComponent(typeof(ResourcesControl))]
    [RequireComponent(typeof(StatsControl))]
    [RequireComponent(typeof(StatusEffectControl))]
    [RequireComponent(typeof(CharacterPickerControl))]
    class CombatCharacter : MonoBehaviour
    {
        public Character Character = null;
        private void Start()
        {
            int characterId = GetComponent<CharacterPickerControl>().GetCharacterId();

            ICharacterService characterService = CharacterService.Get();
            if (characterService != null)
            {
                Character = characterService.GetCharacter(characterId);

                GetComponent<StatsControl>().SetAttributes(Character.CurrentAttributes);
                GetComponent<ResourcesControl>().InitResourcesFromAttributes();
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
        }

        public void OnTurnTick()
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

        public void OnTurnStart()
        {
            StatusEffectControl statusEffectControl = GetComponent<StatusEffectControl>();
            statusEffectControl.RemoveStatusEffects(statusEffectControl.GetSingleTurnStatusEffects());

            List<StateChange> turnStateStateChanges = statusEffectControl.GetTurnStartStateChanges();
            foreach (StateChange stateChange in turnStateStateChanges)
            {
                GetComponent<CombatTarget>().ApplyStateChange(stateChange);
            }

            GetComponent<ResourcesControl>().Resources[ResourceType.Actions].SetCurrentToMax();
            GetComponent<ResourcesControl>().Resources[ResourceType.MovementRange].SetCurrentToMax();
        }
    }
}
