using MAGE.GameModes.Combat;
using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounters;
using MAGE.GameModes.SceneElements.Navigation;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Loot;
using MAGE.GameSystems.World;
using MAGE.UI;
using MAGE.UI.Views;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace MAGE.GameModes.Encounter
{
    class EncounterLoadFlowControl : FlowControlBase
    {
        private string TAG = "EncounterLoadFlowControl";

        private EncounterContainer mEncounterToLoad;
        private IEnumerable<ControllableEntity> mInitialEntities = null;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.EncounterLoadFlowControl;
        }

        protected override void Setup()
        {
            PrepareEncounter();
        }

        private void ConvertPlaceholderCharactersToCombatCharacters()
        {
            Level level = LevelManagementService.Get().GetLoadedLevel();

            {
                List<CharacterPickerControl> enemies = mEncounterToLoad.Allys
                    .GetComponentsInChildren<CharacterPickerControl>(true)
                    .Where(x => x.GetComponent<CombatEntity>() == null)
                    .ToList();
                foreach (CharacterPickerControl character in enemies)
                {
                    ControllableEntity combatCharacter = level.CreateCombatCharacter(character.transform.position, character.transform.rotation, mEncounterToLoad.Allys);

                    // Triggers the addition to EncounterModel
                    combatCharacter.GetComponent<CharacterPickerControl>().CharacterId = character.CharacterId;

                    character.gameObject.SetActive(false);
                }
            }

            {
                List<CharacterPickerControl> enemies = mEncounterToLoad.Enemies
                    .GetComponentsInChildren<CharacterPickerControl>(true)
                    .Where(x => x.GetComponent<CombatEntity>() == null)
                    .ToList();
                foreach (CharacterPickerControl character in enemies)
                {
                    ControllableEntity combatCharacter = level.CreateCombatCharacter(character.transform.position, character.transform.rotation, mEncounterToLoad.Enemies);

                    // Triggers the addition to EncounterModel
                    combatCharacter.GetComponent<CharacterPickerControl>().CharacterId = character.CharacterId;

                    character.gameObject.SetActive(false);
                }
            }
        }

        private void PrepareEncounter()
        {
            Level level = LevelManagementService.Get().GetLoadedLevel();
            mEncounterToLoad = level.GetActiveEncounter();
            GameModel.Encounter = mEncounterToLoad.EncounterModel;

            // Win Loss Conditions
            mEncounterToLoad.EncounterModel.mWinConditions = mEncounterToLoad.WinConditions.GetComponents<EncounterCondition>().ToList();
            Debug.Assert(mEncounterToLoad.EncounterModel.mWinConditions.Count > 0);

            mEncounterToLoad.EncounterModel.mLoseConditions = mEncounterToLoad.LoseConditions.GetComponents<EncounterCondition>().ToList();
            Debug.Assert(mEncounterToLoad.EncounterModel.mLoseConditions.Count > 0);

            mEncounterToLoad.StartEncounter();
            ConvertPlaceholderCharactersToCombatCharacters();

            mInitialEntities = mEncounterToLoad.GetComponentsInChildren<ControllableEntity>();
        }

        void Update()
        {
            bool isLoaded = true;
            foreach (ControllableEntity controllableEntity in mInitialEntities)
            {
                if (!mEncounterToLoad.EncounterModel.Players.ContainsKey(controllableEntity.Id))
                {
                    isLoaded = false;
                    break;
                }
            }

            if (isLoaded)
            {
                SendFlowMessage("encounterLoaded");
            }
        }

        protected override void Cleanup()
        {
            
        }
    }
}


