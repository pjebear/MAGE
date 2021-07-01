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

                case "resolveStatusEffect":
                {
                    Debug.Assert(ArStatusEffectsPending());

                    EncounterModel.StatusEffectInfo toDisplay = null;
                    if (ArStatusEffectsPending())
                    {
                        while (ArStatusEffectsPending())
                        {
                            EncounterModel.StatusEffectInfo nextEffectInfo = mEncounterModel.mStatusEffectQueue.Peek();
                            mEncounterModel.mStatusEffectQueue.Dequeue();

                            if (nextEffectInfo.ControllableEntity.GetComponent<ResourcesControl>().IsAlive())
                            {
                                toDisplay = nextEffectInfo;
                                break;
                            }
                        }
                    }

                    if (toDisplay != null)
                    {
                        if (toDisplay.Expired)
                        {
                            toDisplay.ControllableEntity.GetComponent<StatusEffectControl>().RemoveStatusEffects(new List<StatusEffect>() { toDisplay.StatusEffect });
                        }
                        else
                        {
                            toDisplay.ControllableEntity.GetComponent<CombatTarget>().ApplyStateChange(toDisplay.StatusEffect.GetTurnEndStateChange());
                        }

                        Camera.main.GetComponent<Cameras.CameraController>().SetTarget(toDisplay.ControllableEntity.transform, Cameras.CameraType.TopDown);

                        Invoke("NotifyStatusEffectResolved", 2);
                    }
                    else
                    {
                        NotifyStatusEffectResolved();
                    }
                    handled = true;
                }
                break;
            }

            return handled;
        }

        void NotifyStatusEffectResolved()
        {
            SendFlowMessage("statusEffectResolved");
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

                            if (ArStatusEffectsPending())
                            {
                                result = "statusEffectResolution";
                            }
                            else if (AreActionsPending())
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

        private bool ArStatusEffectsPending()
        {
            return mEncounterModel.mStatusEffectQueue.Count > 0;
        }

        private void ProgressClock()
        {
            List<TemporaryEntity> toUpdate = new List<TemporaryEntity>(mEncounterModel.mTemporaryEntities);
            foreach (TemporaryEntity temporary in toUpdate)
            {
                temporary.OnTurnTick();
            }

            // Status Effects
            {
                foreach (ControllableEntity controllableEntity in mEncounterModel.AlivePlayers)
                {
                    StatusEffectControl statusEffectControl = controllableEntity.GetComponent<StatusEffectControl>();
                    foreach (StatusEffect statusEffect in statusEffectControl.mStatusEffectLookup.Values)
                    {
                        statusEffect.ProgressDuration();

                        if (statusEffect.HasExpired())
                        {
                            EncounterModel.StatusEffectInfo statusEffectInfo = new EncounterModel.StatusEffectInfo();
                            statusEffectInfo.ControllableEntity = controllableEntity;
                            statusEffectInfo.StatusEffect = statusEffect;
                            statusEffectInfo.Expired = true;

                            mEncounterModel.mStatusEffectQueue.Enqueue(statusEffectInfo);
                        }
                    }
                }
            }

            // Charging Actions
            {
                List<CombatEntity> chargedActions = new List<CombatEntity>();
                foreach (var chargingActionPair in mEncounterModel.mChargingActions)
                {
                    EncounterModel.ChargingActionInfo chargingActionInfo = chargingActionPair.Value;

                    --chargingActionInfo.TicksRemaining;
                    if (chargingActionInfo.TicksRemaining <= 0)
                    {
                        mEncounterModel.mActionQueue.Enqueue(chargingActionInfo.Action);
                        chargedActions.Add(chargingActionPair.Key);
                    }
                }

                foreach (CombatEntity combatEntity in chargedActions)
                {
                    mEncounterModel.mChargingActions.Remove(combatEntity);
                }
            }

            foreach (ControllableEntity combatCharacter in mEncounterModel.AlivePlayers)
            {
                combatCharacter.OnTurnTick();
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
                    || mEncounterModel.TurnComplete)
                {

                    foreach (StatusEffect statusEffect in currentTurn.GetComponent<StatusEffectControl>().mStatusEffectLookup.Values)
                    {
                        if (statusEffect.GetTurnEndStateChange() != null)
                        {
                            EncounterModel.StatusEffectInfo statusEffectInfo = new EncounterModel.StatusEffectInfo();
                            statusEffectInfo.ControllableEntity = (ControllableEntity)currentTurn;
                            statusEffectInfo.StatusEffect = statusEffect;
                            statusEffectInfo.Expired = false;

                            mEncounterModel.mStatusEffectQueue.Enqueue(statusEffectInfo);
                        }
                    }

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

        class TurnOrderEntry
        {
            public TurnOrderCharacterPanel.DataProvider Entry;
            public int TicksTilTurn = 0;
        }

        IDataProvider PublishTurnOrderView()
        {
            mTurnOptions.Clear();

            EncounterTurnOrderView.DataProvider dp = new EncounterTurnOrderView.DataProvider();

            List<TurnOrderEntry> entriesToPutInQueue = new List<TurnOrderEntry>();

            List<ControllableEntity> entitiesToPutInTurnQueue = new List<ControllableEntity>(mEncounterModel.AlivePlayers);

            bool enemyFoundInTurnOrder = false;
            if (mEncounterModel.CurrentTurn != null)
            {
                TurnOrderCharacterPanel.DataProvider turnOrderDP = GetTurnFlowDPForControllable(mEncounterModel.CurrentTurn);
                turnOrderDP.IsCurrentTurn = true;
                enemyFoundInTurnOrder = mEncounterModel.CurrentTurn.TeamSide == TeamSide.EnemyAI;
                entriesToPutInQueue.Add(new TurnOrderEntry() { Entry = turnOrderDP, TicksTilTurn = -20 });
                entitiesToPutInTurnQueue.Remove(mEncounterModel.CurrentTurn);
                mTurnOptions.Add(mEncounterModel.CurrentTurn);
            }

            foreach (ControllableEntity controllableEntity in mEncounterModel.TurnQueue)
            {
                TurnOrderCharacterPanel.DataProvider turnOrderDP = GetTurnFlowDPForControllable(controllableEntity);
                enemyFoundInTurnOrder |= controllableEntity.TeamSide == TeamSide.EnemyAI;
                turnOrderDP.IsCurrentTurn = (!enemyFoundInTurnOrder && controllableEntity.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Clock].Ratio == 1);

                if (!enemyFoundInTurnOrder)
                {
                    mTurnOptions.Add(controllableEntity);
                }

                entriesToPutInQueue.Add(new TurnOrderEntry() { Entry = turnOrderDP, TicksTilTurn = 0 });
                entitiesToPutInTurnQueue.Remove(controllableEntity);
            }

            foreach (ActionProposal action in mEncounterModel.mActionQueue)
            {
                entriesToPutInQueue.Add(new TurnOrderEntry() { Entry = GetTurnFlowDPForAction(action), TicksTilTurn = -10 });
            }

            foreach (ControllableEntity controllableEntity in entitiesToPutInTurnQueue)
            {
                float currentClock = controllableEntity.GetComponent<ResourcesControl>().Resources[GameSystems.Stats.ResourceType.Clock].mCurrent;
                float speed = controllableEntity.GetComponent<StatsControl>().Attributes[TertiaryStat.Speed];

                int ticksTilTurn = (int)((100 - currentClock) / speed);
                entriesToPutInQueue.Add(new TurnOrderEntry() { Entry = GetTurnFlowDPForControllable(controllableEntity), TicksTilTurn = ticksTilTurn * 10 });
            }

            foreach (EncounterModel.ChargingActionInfo chargingAction in mEncounterModel.mChargingActions.Values)
            {
                entriesToPutInQueue.Add(new TurnOrderEntry() { Entry = GetTurnFlowDPForAction(chargingAction.Action), TicksTilTurn = chargingAction.TicksRemaining * 10 - 5 });
            }

            entriesToPutInQueue.Sort((x, y) => x.TicksTilTurn.CompareTo(y.TicksTilTurn));

            
            foreach (TurnOrderEntry turn in entriesToPutInQueue)
            {
                dp.TurnOrder.Add(turn.Entry);
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

        private TurnOrderCharacterPanel.DataProvider GetTurnFlowDPForAction(ActionProposal chargingAction)
        {
            TurnOrderCharacterPanel.DataProvider turnOrderDP = new TurnOrderCharacterPanel.DataProvider();

            turnOrderDP.PortraitAsset = chargingAction.Action.ActionInfo.ActionId.ToString();
            turnOrderDP.IsAlly = chargingAction.Proposer.TeamSide == TeamSide.AllyHuman;

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


