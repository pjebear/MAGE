using MAGE.GameModes.Combat;
using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounters;
using MAGE.GameModes.SceneElements.Navigation;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Loot;
using MAGE.GameSystems.Stats;
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

        private AudioSource mAmbientSoundSource;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Encounter;
        }

        private bool mIsTurnSwapEnabled = false;
        private List<ControllableEntity> mTurnOptions = new List<ControllableEntity>();

        protected override void Setup()
        {
            mEncounterModel = LevelManagementService.Get().GetLoadedLevel().GetActiveEncounter().EncounterModel;

            UIManager.Instance.PostContainer(UIContainerId.EncounterStatusView, this);
            UIManager.Instance.PostContainer(UIContainerId.EncounterTurnOrderView, this);

            AudioManager.Instance.PlayTrack(TrackId.Encounter);
        }

        protected override void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterStatusView);
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterTurnOrderView);

            AudioManager.Instance.StopTrack(TrackId.Encounter);

            Level level = LevelManagementService.Get().GetLoadedLevel();
            EncounterContainer activeEncounter = level.GetActiveEncounter();

            Messaging.MessageRouter.Instance.NotifyMessage(new LevelManagement.LevelMessage(LevelManagement.MessageType.EncounterComplete, activeEncounter));
        }

        public override bool Notify(string notifyEvent)
        {
            bool handled = false;

            switch (notifyEvent)
            {
                case "encounterStarted":
                {
                    mEncounterModel.IsEncounterActive = true;
                    handled = true;
                }
                break;

                case "enableTurnSwap":
                {
                    mIsTurnSwapEnabled = true;
                    handled = true;
                }
                break;

                case "disableTurnSwap":
                {
                    mIsTurnSwapEnabled = false;
                    handled = true;
                }
                break;

                case "encounterComplete":
                {
                    mEncounterModel.IsEncounterActive = false;
                    handled = true;
                }
                break;

                case "displayHoverInspectors":
                {
                    UIManager.Instance.PostContainer(UIContainerId.EncounterCharacterHoverView, this);
                    handled = true;
                }
                break;

                case "hideHoverInspectors":
                {
                    UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterHoverView);
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
                    if (mEncounterModel.IsEncounterOver())
                    {
                        result = "encounterComplete";
                    }
                    else
                    {
                        ProgressTurnFlow();

                        int safetyCatch = 100;
                        while(result == "" && (--safetyCatch > 0))
                        {
                            if (AreActionsPending())
                            {
                                result = "actionResolution";
                            }
                            else if (mEncounterModel.CurrentTurn != null)
                            {
                                result = "playerTurnFlow";
                            }
                            else if (mEncounterModel.TurnQueue.Count > 0)
                            {
                                mEncounterModel.CurrentTurn = mEncounterModel.TurnQueue[0];
                                mEncounterModel.CurrentTurn.OnTurnStart();
                                mEncounterModel.CurrentTurn.GetComponent<ActorMotor>().Enable(true);
                                mEncounterModel.TurnQueue.RemoveAt(0);

                                result = "playerTurnFlow";
                            }
                            else
                            {
                                ProgressClock();
                            }
                        }
                        Debug.Assert(safetyCatch > 0);

                        UIManager.Instance.Publish(UIContainerId.EncounterTurnOrderView);
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

        private bool AreActionsPending()
        {
            return mEncounterModel.mActionQueue.Count > 0;
        }

        private void ProgressClock()
        {
            List<TemporaryEntity> toUpdate = new List<TemporaryEntity>(mEncounterModel.mTemporaryEntities);
            foreach (TemporaryEntity temporary in toUpdate)
            {
                temporary.OnTurnTick();
            }

            foreach (ControllableEntity combatCharacter in mEncounterModel.AlivePlayers)
            {
                combatCharacter.OnTurnTick();

                if (mEncounterModel.mChargingActions.ContainsKey(combatCharacter))
                {
                    ActionProposal actionProposal = mEncounterModel.mChargingActions[combatCharacter];

                    if (!actionProposal.Action.AreActionRequirementsMet())
                    {
                        mEncounterModel.mChargingActions.Remove(combatCharacter);
                    }
                    else if (actionProposal.Action.AreResourceRequirementsMet())
                    {
                        mEncounterModel.mActionQueue.Enqueue(actionProposal);
                        mEncounterModel.mChargingActions.Remove(combatCharacter);
                    }
                }
            }
            
            mEncounterModel.TurnQueue = mEncounterModel.AlivePlayers.Where(x =>
                x.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Clock].Ratio == 1).ToList();
            foreach (ControllableEntity controllableEntity in mEncounterModel.TurnQueue)
            {
                controllableEntity.OnTurnAvailable();
            }
        }

        private void ProgressTurnFlow()
        {
            if (mEncounterModel.CurrentTurn != null)
            {
                CombatEntity currentTurn = mEncounterModel.CurrentTurn;
                if (!currentTurn.GetComponent<ResourcesControl>().IsAlive() 
                    || mEncounterModel.TurnComplete
                    || (currentTurn.GetComponent<ActionsControl>().GetAvailableMovementRange() == 0 
                        && currentTurn.GetComponent<ActionsControl>().GetNumAvailableActions() == 0))
                {
                    mEncounterModel.TurnComplete = false;

                    mEncounterModel.CurrentTurn.GetComponent<ActorMotor>().Enable(false);

                    mEncounterModel.CurrentTurn.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Clock].SetCurrentToZero();

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
            }

            // Clean up action queue
            List<CombatEntity> charactersCharing = mEncounterModel.mChargingActions.Keys.ToList();
            foreach (CombatEntity character in charactersCharing)
            {
                if (!character.GetComponent<ResourcesControl>().IsAlive())
                {
                    mEncounterModel.mChargingActions.Remove(character);
                }
            }
        }

        public IDataProvider Publish(int containerId)
        {
            IDataProvider dp = null;
            switch (containerId)
            {
                case (int)UIContainerId.EncounterCharacterHoverView:
                {
                    dp = PublishCharacterHoverView();
                }
                break;
                case (int)UIContainerId.EncounterTurnOrderView:
                {
                    dp = PublishTurnOrderView();
                }
                break;
                case (int)UIContainerId.EncounterStatusView:
                {
                    dp = new EncounterStatus.DataProvider();
                }
                break;
                default:
                {
                    Debug.Assert(false);
                }
                break;
            }

            return dp;
        }

        IDataProvider PublishCharacterHoverView()
        {
            EncounterCharacterHoverView.DataProvider dp = new EncounterCharacterHoverView.DataProvider();

            foreach (ControllableEntity controllableEntity in mEncounterModel.AlivePlayers)
            {
                CharacterHoverInspector.DataProvider inspectorDp = new CharacterHoverInspector.DataProvider();
                inspectorDp.PortraitAsset = controllableEntity.Character.Appearance.PortraitSpriteId.ToString();
                inspectorDp.Specialization = controllableEntity.Character.CurrentSpecializationType.ToString();
                inspectorDp.Name = controllableEntity.Character.Name.ToString();
                inspectorDp.Level = controllableEntity.Character.Level;
                inspectorDp.IsAlly = controllableEntity.TeamSide == TeamSide.AllyHuman;
                inspectorDp.CurrentHP = (int)controllableEntity.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Health].mCurrent;
                inspectorDp.MaxHP = (int)controllableEntity.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Health].mMax;

                foreach (StatusEffect statusEffect in controllableEntity.GetComponent<StatusEffectControl>().mStatusEffectLookup.Values)
                {
                    StatusIcon.DataProvider statusDp = new StatusIcon.DataProvider();
                    statusDp.Count = statusEffect.StackCount;
                    statusDp.IsBeneficial = statusEffect.Beneficial;
                    statusDp.AssetName = statusEffect.SpriteId.ToString();
                    inspectorDp.StatusEffects.Add(statusDp);
                }

                dp.CharacterDPs.Add(controllableEntity.transform, inspectorDp);
            }

            return dp;
        }

        IDataProvider PublishTurnOrderView()
        {
            mTurnOptions.Clear();

            EncounterTurnOrderView.DataProvider dp = new EncounterTurnOrderView.DataProvider();

            List<ControllableEntity> entitiesToPutInTurnQueue = new List<ControllableEntity>(mEncounterModel.AlivePlayers);

            List<ControllableEntity> toPublish = new List<ControllableEntity>();
            if (mEncounterModel.CurrentTurn != null)
            {
                toPublish.Add(mEncounterModel.CurrentTurn);
                entitiesToPutInTurnQueue.Remove(mEncounterModel.CurrentTurn);
            }

            foreach (ControllableEntity controllableEntity in mEncounterModel.TurnQueue)
            {
                toPublish.Add(controllableEntity);
                entitiesToPutInTurnQueue.Remove(controllableEntity);
            }

            entitiesToPutInTurnQueue.Sort((x, y) => y.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Clock].mCurrent.CompareTo(x.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Clock].mCurrent));
            toPublish.AddRange(entitiesToPutInTurnQueue);

            bool enemyFoundInTurnOrder = false;
            foreach (ControllableEntity controllableEntity in toPublish)
            {
                TurnOrderCharacterPanel.DataProvider turnOrderDP = GetTurnFlowDPForControllable(controllableEntity);

                enemyFoundInTurnOrder |= controllableEntity.TeamSide != TeamSide.AllyHuman;
                turnOrderDP.IsCurrentTurn = 
                    dp.TurnOrder.Count == 0 
                    || (!enemyFoundInTurnOrder && controllableEntity.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Clock].Ratio == 1);

                if (!enemyFoundInTurnOrder)
                {
                    mTurnOptions.Add(controllableEntity);
                }

                entitiesToPutInTurnQueue.Remove(controllableEntity);

                dp.TurnOrder.Add(turnOrderDP);
            }

            return dp;
        }

        private TurnOrderCharacterPanel.DataProvider GetTurnFlowDPForControllable(ControllableEntity controllableEntity)
        {
            TurnOrderCharacterPanel.DataProvider turnOrderDP = new TurnOrderCharacterPanel.DataProvider();

            turnOrderDP.PortraitAsset = controllableEntity.Character.Appearance.PortraitSpriteId.ToString();
            turnOrderDP.IsAlly = controllableEntity.TeamSide == TeamSide.AllyHuman;
            turnOrderDP.CurrentHP = (int)controllableEntity.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Health].mCurrent;
            turnOrderDP.MaxHP = (int)controllableEntity.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Health].mMax;

            return turnOrderDP;
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
            else if (containerId == (int)UIContainerId.EncounterTurnOrderView 
                && interactionInfo.InteractionType == UIInteractionType.Click
                && mIsTurnSwapEnabled)
            {
                ListInteractionInfo listInfo = interactionInfo as ListInteractionInfo;

                if (listInfo.ListIdx > 0 && listInfo.ListIdx < mTurnOptions.Count)
                {
                    ControllableEntity currentTurn = mEncounterModel.CurrentTurn;
                    ControllableEntity selectedUnit = mTurnOptions[listInfo.ListIdx];

                    int indexInTurnQueueOfSelected = mEncounterModel.TurnQueue.IndexOf(selectedUnit);
                    Debug.Assert(indexInTurnQueueOfSelected != -1);
                    if (indexInTurnQueueOfSelected != -1)
                    {
                        mEncounterModel.CurrentTurn = selectedUnit;
                        mEncounterModel.TurnQueue.Insert(indexInTurnQueueOfSelected, currentTurn);
                        mEncounterModel.TurnQueue.Remove(selectedUnit);

                        UIManager.Instance.Publish(UIContainerId.EncounterTurnOrderView);
                        SendFlowMessage("focusedCharacterChanged");
                    }
                }
            }
        }

        public string Name()
        {
            return "EncounterFlowControl";
        }
    }
}


