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

        protected override void Setup()
        {
            mEncounterModel = LevelManagementService.Get().GetLoadedLevel().GetActiveEncounter().EncounterModel;

            UIManager.Instance.PostContainer(UIContainerId.EncounterStatusView, this);

            AudioManager.Instance.PlayTrack(TrackId.Encounter);
        }

        protected override void Cleanup()
        {
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterStatusView);

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

                case "encounterComplete":
                {
                    mEncounterModel.IsEncounterActive = false;
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
    }
}


