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
            int characterId = GetComponent<CharacterPickerControl>().CharacterPicker.GetCharacterId();

            ICharacterService characterService = CharacterService.Get();
            if (characterService != null)
            {
                Character = characterService.GetCharacter(characterId);

                GetComponent<StatsControl>().SetAttributes(Character.CurrentAttributes);
                GetComponent<ResourcesControl>().InitResourcesFromAttributes();
                GetComponent<ActionsControl>().Actions = Character.GetActionIds();
                GetComponent<EquipmentControl>().Equipment = Character.Equipment;

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

        public void OnTurnStart()
        {
            List<StateChange> turnStateStateChanges = GetComponent<StatusEffectControl>().GetTurnStartStateChanges();
            foreach (StateChange stateChange in turnStateStateChanges)
            {
                GetComponent<CombatTarget>().ApplyStateChange(stateChange);
            }

            GetComponent<ResourcesControl>().Resources[ResourceType.Mana]
                .Modify((int)(GetComponent<ResourcesControl>().Resources[ResourceType.Mana].Max 
                * GetComponent<StatsControl>().Attributes[SecondaryStat.Attunement] / 100
                / 2f));
            GetComponent<ResourcesControl>().Resources[ResourceType.Actions].SetCurrentToMax();
            GetComponent<ResourcesControl>().Resources[ResourceType.MovementRange].SetCurrentToMax();
        }
    }
}
