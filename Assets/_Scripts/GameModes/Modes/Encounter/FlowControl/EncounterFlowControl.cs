﻿using MAGE.GameModes.Combat;
using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounters;
using MAGE.GameModes.SceneElements.Navigation;
using MAGE.GameSystems;
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
    class EncounterFlowControl 
        : FlowControlBase
        , UIContainerControl
    {
        private string TAG = "EncounterFlowControl";

        private EncounterModel mEncounterModel;
        private List<EncounterCondition> mWinConditions = new List<EncounterCondition>();
        private List<EncounterCondition> mLoseConditions = new List<EncounterCondition>();

        private AudioSource mAmbientSoundSource;

        enum FlowState
        {
            Intro,
            //UnitPlacement,
            //Init,

            CoreLoopBegin,
            StatusIncrement = CoreLoopBegin,
            StatusCheck,
            ActionIncrement,
            ActionResolution,
            TurnIncrement,
            TurnResolution,
            CoreLoopEnd,

            Outro,

            NUM
        }

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Encounter;
        }

        protected override void Setup()
        {
            
        }

        private void PrepareScenarioEncounter(EncounterContainer activeContainer)
        {
            Level level = LevelManagementService.Get().GetLoadedLevel();

            {
                List<CharacterPickerControl> allies = activeContainer.Allys.GetComponentsInChildren<CharacterPickerControl>(true).ToList();
                foreach (CharacterPickerControl character in allies)
                {
                    CombatCharacter combatCharacter = level.CreateCombatCharacter(character.transform.position, character.transform.rotation, activeContainer.Allys);
                    combatCharacter.GetComponent<CharacterPickerControl>().CharacterPicker.SetRootCharacterId(character.CharacterPicker.GetCharacterId());
                    character.gameObject.SetActive(false);
                }
            }

            {
                List<CharacterPickerControl> enemies = activeContainer.Enemies.GetComponentsInChildren<CharacterPickerControl>(true).ToList();
                foreach (CharacterPickerControl character in enemies)
                {
                    CombatCharacter combatCharacter = level.CreateCombatCharacter(character.transform.position, character.transform.rotation, activeContainer.Enemies);
                    combatCharacter.GetComponent<CharacterPickerControl>().CharacterPicker.SetRootCharacterId(character.CharacterPicker.GetCharacterId());
                    character.gameObject.SetActive(false);
                }
            }
        }

        private void PrepareEncounter()
        {
            Level level = LevelManagementService.Get().GetLoadedLevel();
            EncounterContainer activeEncounter = level.GetActiveEncounter();
            if (activeEncounter.EncounterScenarioId != EncounterScenarioId.Random)
            {
                activeEncounter.StartEncounter();
                PrepareScenarioEncounter(activeEncounter);
            }

            mWinConditions = activeEncounter.WinConditions.GetComponents<EncounterCondition>().ToList();
            Debug.Assert(mWinConditions.Count > 0);
            mLoseConditions = activeEncounter.LoseConditions.GetComponents<EncounterCondition>().ToList();
            Debug.Assert(mLoseConditions.Count > 0);

            mEncounterModel = new EncounterModel();
            GameModel.Encounter = mEncounterModel;

            mEncounterModel.Teams.Add(TeamSide.AllyHuman, activeEncounter.Allys.GetComponentsInChildren<CombatCharacter>().ToList());
            foreach (CombatCharacter character in mEncounterModel.Teams[TeamSide.AllyHuman])
            {
                mEncounterModel.Players.Add(character.GetComponent<CharacterPickerControl>().CharacterPicker.GetCharacterId(), character);
                character.GetComponent<ActorMotor>().Enable(false);
                character.GetComponent<CombatEntity>().TeamSide = TeamSide.AllyHuman;
            }

            mEncounterModel.Teams.Add(TeamSide.EnemyAI, activeEncounter.Enemies.GetComponentsInChildren<CombatCharacter>().ToList());
            foreach (CombatCharacter character in mEncounterModel.Teams[TeamSide.EnemyAI])
            {
                mEncounterModel.Players.Add(character.GetComponent<CharacterPickerControl>().CharacterPicker.GetCharacterId(), character);
                character.GetComponent<ActorMotor>().Enable(false);
                character.GetComponent<CombatEntity>().TeamSide = TeamSide.EnemyAI;

                character.GetComponentInChildren<ActorOutfitter>().SetOutfitColorization(GameSystems.Appearances.OutfitColorization.Enemy);
            }

            UIManager.Instance.PostContainer(UIContainerId.EncounterStatusView, this);

            mAmbientSoundSource = gameObject.AddComponent<AudioSource>();
            mAmbientSoundSource.clip = AudioManager.Instance.GetTrack(TrackId.Encounter);
            mAmbientSoundSource.loop = true;
            mAmbientSoundSource.spatialBlend = 0; // global volume
                                                  //mAmbientSoundSource.Play();
            AudioManager.Instance.FadeInTrack(mAmbientSoundSource, 5, .5f);
        }

        protected override void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterStatusView);

            Level level = LevelManagementService.Get().GetLoadedLevel();
            EncounterContainer activeEncounter = level.GetActiveEncounter();
            Destroy(activeEncounter.gameObject);

            EncounterResultInfo result = new EncounterResultInfo();
            result.EncounterScenarioId = activeEncounter.EncounterScenarioId;
            result.PlayersInEncounter = mEncounterModel.Teams[TeamSide.AllyHuman]
                .Select(x=> x.Character.Id)
                .ToList();

            result.DidUserWin = IsEncounterWon();

            WorldService.Get().UpdateOnEncounterEnd(result);
        }

        public override bool Notify(string notifyEvent)
        {
            bool handled = false;

            switch (notifyEvent)
            {
                case "prepareEncounter":
                {
                    PrepareEncounter();
                    SendFlowMessage("encounterPrepared");
                    handled = true;
                }
                break;

                case "progressTurnFlow":
                {
                    ProgressTurnFlow();
                    handled = true;
                }
                break;

            }

            return handled;
        }

        public override string Query(string queryParam)
        {
            string result = "";

            switch (queryParam)
            {
                case "encounterFlowState":
                {
                    if (IsEncounterOver())
                    {
                        result = "encounterComplete";
                    }
                    else if (AreActionsPending())
                    {
                        result = "actionResolution";
                    }
                    else
                    {
                        result = "playerTurnFlow";
                    }
                }
                break;
            }

            return result;
        }

        public override string Condition(string condition)
        {
            string result = FlowConstants.CONDITION_EMPTY;

            switch (condition)
            {
                case "havePendingActions":
                {
                    result = mEncounterModel.mActionQueue.Count > 0
                        ? FlowConstants.CONDITION_TRUE
                        : FlowConstants.CONDITION_FALSE;
                }
                break;
                case "isCurrentTurnPlayControlled":
                {
                    result = mEncounterModel.CurrentTurn.GetComponent<CombatEntity>().TeamSide == TeamSide.AllyHuman
                        ? FlowConstants.CONDITION_TRUE
                        : FlowConstants.CONDITION_FALSE;
                }
                break;
            }

            return result;
        }

        private bool IsEncounterLost()
        {
            bool isLost = true;

            isLost = mLoseConditions.Where(x => x.IsConditionMet(mEncounterModel)).Count() > 0;

            return isLost;
        }

        private bool IsEncounterWon()
        {
            bool isWon = mWinConditions.Where(x => x.IsConditionMet(mEncounterModel)).Count() > 0;

            return isWon;
        }

        private bool IsEncounterOver()
        {
            return IsEncounterLost() || IsEncounterWon();
        }

        private bool AreActionsPending()
        {
            return mEncounterModel.mActionQueue.Count > 0;
        }

        private void ProgressTurnFlow()
        {
            if (mEncounterModel.CurrentTurn != null)
            {
                CombatCharacter currentTurn = mEncounterModel.CurrentTurn;
                if (!currentTurn.GetComponent<ResourcesControl>().IsAlive() 
                    || mEncounterModel.TurnComplete
                    || (currentTurn.GetComponent<ResourcesControl>().GetAvailableMovementRange() == 0 
                        && currentTurn.GetComponent<ResourcesControl>().GetNumAvailableActions() == 0))
                {
                    mEncounterModel.TurnComplete = false;

                    mEncounterModel.CurrentTurn.GetComponent<ActorMotor>().Enable(false);

                    mEncounterModel.CurrentTurn.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Clock].Current = 0;

                    mEncounterModel.CurrentTurn = null;
                }
            }

            // Clean up existing turn queue
            if (mEncounterModel.CurrentTurn == null)
            {
                if (mEncounterModel.TurnQueue.Count > 0)
                {
                    mEncounterModel.TurnQueue = mEncounterModel.TurnQueue.Where(x => x.GetComponent<ResourcesControl>().IsAlive()).ToList();
                }

                if (mEncounterModel.TurnQueue.Count == 0)
                {
                    PopulateTurnQueue();
                }

                mEncounterModel.CurrentTurn = mEncounterModel.TurnQueue[0];
                mEncounterModel.CurrentTurn.OnTurnStart();
                mEncounterModel.CurrentTurn.GetComponent<ActorMotor>().Enable(true);
                mEncounterModel.TurnQueue.RemoveAt(0);
            }
        }

        public IDataProvider Publish(int containerId)
        {
            EncounterStatus.DataProvider dataProvider = new EncounterStatus.DataProvider();

            return dataProvider;
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            if (containerId == (int)UIContainerId.EncounterStatusView
                && interactionInfo.InteractionType == UIInteractionType.Click)
            {
                if (interactionInfo.ComponentId == (int)EncounterStatus.ComponentId.WinBtn)
                {
                    SendFlowMessage("forceWin");
                }
                else if (interactionInfo.ComponentId == (int)EncounterStatus.ComponentId.LoseBtn)
                {
                    SendFlowMessage("forceLoss");
                }
            }
        }

        public string Name()
        {
            return "EncounterFlowControl";
        }

        void PopulateTurnQueue()
        {
            Debug.Assert(!IsEncounterOver());
            if (!IsEncounterOver())
            {
                while (mEncounterModel.TurnQueue.Count == 0)
                {
                    foreach (CombatCharacter combatCharacter in mEncounterModel.AlivePlayers)
                    {
                        ResourcesControl resourcesControl = combatCharacter.GetComponent<ResourcesControl>();
                        StatsControl statsControl = combatCharacter.GetComponent<StatsControl>();
                        
                        resourcesControl.Resources[GameSystems.Stats.ResourceType.Clock].Modify((int)statsControl.Attributes[GameSystems.Stats.TertiaryStat.Speed]);
                        if (resourcesControl.Resources[GameSystems.Stats.ResourceType.Clock].Ratio == 1)
                        {
                            mEncounterModel.TurnQueue.Add(combatCharacter);
                        }
                    }
                }
            }
        }
    }
}


